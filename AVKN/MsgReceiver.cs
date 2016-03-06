using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using VkNet;
using VkNet.Categories;
using VkNet.Utils;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;
using VkNet.Enums;
using VkNet.Enums.Filters;
using VkNet.Exception;
using System.Windows.Forms;

namespace AVKN
{
    public interface IMsgReceiver
    {
        bool LogInVk(string login, string password);

        bool IsLogged();

        bool RetrieveMessages();

        int GetMessagesCount();

        Message PopFirstMsg();

        void ClearMsgStack();
    }

    public class MsgReceiver : IMsgReceiver
    {
        private bool isLogged;
        private Stack<Message> messageStack = new Stack<Message>();
        private VkApi vk = new VkApi();
        Dictionary<long, VkNet.Model.User> usersDict;
        //private HashSet<long> viewedGroups = new HashSet<long>();
        int receivedPostsCounter = -1;
        System.Collections.ObjectModel.ReadOnlyCollection<Group> vkGroups;
        int vkGroupsCount = 0;
        int currentVkGroup = 0;
        Dictionary<long, DateTime> lastReceivingDateForGroups;

        public bool LogInVk(string login, string password)
        {
            try
            {
                var auth = new ApiAuthParams();

                Settings scope = Settings.All;
                auth.Login = login;
                auth.Password = password;
                auth.ApplicationId = 5322484;
                auth.Settings = scope;

                // Первое, что пришло в голову. Есть примеры лучше? Правим код!
                try
                {
                    vk.Authorize(auth);
                }
                catch (Exception)
                {
                    using (TwoFactorAuthForm authForm = new TwoFactorAuthForm())
                    {
                        auth.TwoFactorAuthorization = authForm.ShowDialogAndReturnKey;

                        vk.Authorize(auth);
                    }
                }

            }
            catch (Exception)
            {
                return false;
            }

            //lastReceivingDate = DateTime.Now;

            return (isLogged = true);
        }

        public bool IsLogged()
        {
            return isLogged;
        }

        public bool RetrieveMessages()
        {
            if (!isLogged)
                return false;

            List<long> userIdsToGet = new List<long>();

            // Чтение сообщений
            MessagesGetParams vkMsgParams = new MessagesGetParams();

            vkMsgParams.Count = 100;
            vkMsgParams.Offset = 0;
            vkMsgParams.TimeOffset = 0;
            vkMsgParams.Filters = MessagesFilter.All;

            MessagesGetObject vkMessages;
            try
            {
                vkMessages = vk.Messages.Get(vkMsgParams);//vk.Messages.Get(0, out offset, 100, 0, new TimeSpan(0), 0);
            }
            catch (Exception)
            {
                return false;
            }

            // Чтение постов в группах
            /*Group vkGroups = vk.Groups.Get(vk.UserId.Value);

            foreach (var vkGroup in vkGroups)
            {
                WallGetParams vkWallParams = new WallGetParams();

                WallGetObject vkWalls = vk.Wall.Get(vkWallParams);
            }*/

            // Получение списка userIdsToGet - id пользователей, которых надо получить с сервера
            foreach (var vkMessage in vkMessages.Messages)
            {
                long vkMessageUserId;

                if (vkMessage.UserId.HasValue)
                {
                    vkMessageUserId = vkMessage.UserId.Value;

                    if ((!usersDict.ContainsKey(vkMessageUserId)) && (!userIdsToGet.Contains(vkMessageUserId)))
                        userIdsToGet.Add(vkMessageUserId);
                }
            }

            try
            {
                var vkUsers = vk.Users.Get(userIdsToGet);

                foreach (var vkUser in vkUsers)
                {
                    usersDict[vkUser.Id] = vkUser;
                }
            }
            catch (Exception)
            {
            }

            foreach (var vkMessage in vkMessages.Messages)
            {
                long vkMessageUserId = 0;

                if (vkMessage.ReadState.HasValue)
                {
                    if (vkMessage.ReadState.Value == MessageReadState.Readed)
                        continue;
                }
                else continue;

                Message msg = new Message();

                if (vkMessage.UserId.HasValue)
                {
                    vkMessageUserId = vkMessage.UserId.Value;

                    if (usersDict.ContainsKey(vkMessageUserId))
                    {
                        if ((usersDict[vkMessageUserId].FirstName != null) && (usersDict[vkMessageUserId].LastName != null))
                            msg.SenderName = usersDict[vkMessageUserId].FirstName + " " + usersDict[vkMessageUserId].LastName;
                        else if (usersDict[vkMessageUserId].Nickname != null)
                            msg.SenderName = usersDict[vkMessageUserId].Nickname;
                    }
                }

                if (string.IsNullOrEmpty(msg.SenderName) && !string.IsNullOrEmpty(vkMessage.Title))
                    msg.SenderName = vkMessage.Title;

                if (vkMessage.Id.HasValue)
                    msg.Id = vkMessage.Id.Value;

                msg.MsgText = vkMessage.Body;

                if (vkMessage.ChatId.HasValue)
                {
                    msg.MsgType = MsgTypes.Dialog;
                    if (vkMessage.ChatId.HasValue)
                    {
                        msg.MsgUrl = "https://vk.com/im?msgid=" + msg.Id.ToString() + "&sel=c" + vkMessage.ChatId.Value;
                        msg.DomainUrl = "https://vk.com/im?sel=c" + vkMessage.ChatId.Value;
                    }
                    else
                    {
                        msg.MsgUrl = "https://vk.com/im";
                        msg.DomainUrl = "https://vk.com/im";
                    }

                }
                else
                {
                    msg.MsgType = MsgTypes.Personal;
                    msg.MsgUrl = "https://vk.com/im?msgid=" + msg.Id.ToString() + "&sel=" + vkMessageUserId.ToString();
                    msg.DomainUrl = "https://vk.com/im?sel=" + vkMessageUserId.ToString();
                }


                messageStack.Push(msg);
                //break;
            }

            //Group vkGroups = vk.Groups.Get(vk.UserId.Value);
            //if (viewedGroups.Count == vkGroups.Count) viewedGroups.Clear();
            //int count = 0;
            //foreach (var vkGroup in vkGroups)
            for (int i = 0; i < 3; i++)
            {
                if (currentVkGroup == vkGroupsCount)
                {
                    try
                    {
                        var newVkGroups = vk.Groups.Get(vk.UserId.Value);

                        vkGroups = newVkGroups;
                        vkGroupsCount = vkGroups.Count;
                    }
                    catch
                    {
                    }
                    
                    currentVkGroup = 0;

                    continue;
                }

                Group vkGroup = vkGroups[currentVkGroup];
                long vkGroupId = vkGroup.Id;

                currentVkGroup++;

                if (lastReceivingDateForGroups.ContainsKey(vkGroupId) == false)
                    lastReceivingDateForGroups[vkGroupId] = DateTime.Now;

                //if (count == 3) break;
                //if (viewedGroups.Contains(vkGroup.Id)) continue;
                //viewedGroups.Add(vkGroup.Id);
                WallGetParams vkWallParams = new WallGetParams();
                //count++;
                vkWallParams.Count = 50;
                vkWallParams.Offset = 0;
                vkWallParams.OwnerId = -vkGroup.Id; // needs to be negative
                WallGetObject vkWalls = null;
                try // useless, but it would throw an exception if the user is not a member (was deleted) or the group id is wrong.
                {
                    vkWalls = vk.Wall.Get(vkWallParams);
                }
                catch (Exception)
                {
                    continue;
                }

                var posts = vkWalls.WallPosts;
                foreach (var post in posts)
                {
                    if (post.Date.HasValue)
                    {
                        //Console.WriteLine("post.Date " + post.Date.Value.ToString() + " lastReceivingDateForGroups[vkGroupId] " + lastReceivingDateForGroups[vkGroupId].ToString());
                        
                        if (post.Date.Value <= lastReceivingDateForGroups[vkGroupId])
                            continue;

                        if (post.Date.Value > lastReceivingDateForGroups[vkGroupId])
                            lastReceivingDateForGroups[vkGroupId] = post.Date.Value;
                    }
                    else continue;

                    Message msg = new Message();

                    msg.MsgType = MsgTypes.Group;
                    msg.MsgText = post.Text;
                    msg.MsgUrl = //"https://vk.com/club" + vkGroupId + "?w=wall" + vkGroup.Id + "_" + post.Id + "%2Fall";
                    msg.DomainUrl = "https://vk.com/club" + vkGroupId;
                    msg.Id = -receivedPostsCounter;
                    receivedPostsCounter--;

                    if (post.FromId.HasValue)
                    {
                        if (post.FromId.Value > 0)
                        {
                            long vkMessageUserId = post.FromId.Value;

                            if (usersDict.ContainsKey(vkMessageUserId))
                            {
                                if ((usersDict[vkMessageUserId].FirstName != null) && (usersDict[vkMessageUserId].LastName != null))
                                    msg.SenderName = usersDict[vkMessageUserId].FirstName + " " + usersDict[vkMessageUserId].LastName;
                                else if (usersDict[vkMessageUserId].Nickname != null)
                                    msg.SenderName = usersDict[vkMessageUserId].Nickname;
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(msg.SenderName) && !string.IsNullOrEmpty(vkGroup.Name))
                        msg.SenderName = vkGroup.Name;

                    //Console.WriteLine("Sender: " + msg.SenderName);
                    //Console.WriteLine("Text: " + msg.MsgText);

                    messageStack.Push(msg);
                }
            }

            return true;
        }

        public int GetMessagesCount()
        {
            return messageStack.Count;
        }

        public Message PopFirstMsg()
        {
            if (messageStack.Count == 0)
                return new Message();
            else
                return messageStack.Pop();
        }

        public void ClearMsgStack()
        {
            messageStack.Clear();
        }

        public MsgReceiver()
        {
            usersDict = new Dictionary<long, VkNet.Model.User>();
            lastReceivingDateForGroups = new Dictionary<long,DateTime>();
        }
    }
}
