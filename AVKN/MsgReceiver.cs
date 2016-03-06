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
        Dictionary<long, string> groupNamesDict;
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
            MessagesGetObject vkMessages = null;
            List<Post> vkPostsToProcess = new List<Post>();

            // Получение сообщений
            if (!ReceiveMessages(userIdsToGet, ref vkMessages))
                return false;

            // Получение постов в группах
            ReceiveGroupPosts(userIdsToGet, vkPostsToProcess);

            // Получение списка userIdsToGet
            ReceiveNewUsers(userIdsToGet);

            // Формирование сообщений
            FormMessagesFromVkMessages(vkMessages);

            // Формирование постов в группах
            FormMessagesFromVkPosts(vkPostsToProcess);

            return true;
        }

        // Получение сообщений и составление списка userIdsToGet
        bool ReceiveMessages(List<long> userIdsToGet, ref MessagesGetObject vkMessages)
        {
            MessagesGetParams vkMsgParams = new MessagesGetParams();

            vkMsgParams.Count = 100;
            vkMsgParams.Offset = 0;
            vkMsgParams.TimeOffset = 0;
            vkMsgParams.Filters = MessagesFilter.All;

            try
            {
                vkMessages = vk.Messages.Get(vkMsgParams);//vk.Messages.Get(0, out offset, 100, 0, new TimeSpan(0), 0);
            }
            catch (Exception)
            {
                return false;
            }

            // Составление списка userIdsToGet (id пользователей, которых надо получить с сервера) из сообщений
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

            return true;
        }

        // Получение постов в группах и составление списка userIdsToGet
        void ReceiveGroupPosts(List<long> userIdsToGet, List<Post> vkPostsToProcess)
        {
            for (int i = 0; i < 3; i++)
            {
                if (currentVkGroup == vkGroupsCount)
                {
                    try
                    {
                        GroupsGetParams vkGroupsParams = new GroupsGetParams();

                        vkGroupsParams.UserId = vk.UserId.Value;
                        vkGroupsParams.Extended = true;

                        var newVkGroups = vk.Groups.Get(vkGroupsParams);

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

                //Console.WriteLine("vkGroup.Name " + vkGroup.Name);

                if (!string.IsNullOrEmpty(vkGroup.Name))
                {
                    groupNamesDict[vkGroupId] = vkGroup.Name;
                }

                currentVkGroup++;

                if (lastReceivingDateForGroups.ContainsKey(vkGroupId) == false)
                    lastReceivingDateForGroups[vkGroupId] = DateTime.Now;

                WallGetParams vkWallParams = new WallGetParams();

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

                    // Составление списка userIdsToGet (id пользователей, которых надо получить с сервера) из постов
                    if (post.FromId.HasValue)
                    {
                        if (post.FromId.Value > 0)
                        {
                            long vkMessageUserId = post.FromId.Value;

                            if ((!usersDict.ContainsKey(vkMessageUserId)) && (!userIdsToGet.Contains(vkMessageUserId)))
                                userIdsToGet.Add(vkMessageUserId);
                        }
                    }

                    vkPostsToProcess.Add(post);
                }
            }
        }

        // Получение пользователей из списка userIdsToGet и добавление в usersDict
        void ReceiveNewUsers(List<long> userIdsToGet)
        {
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
        }

        // Формирование сообщений
        void FormMessagesFromVkMessages(MessagesGetObject vkMessages)
        {
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
        }

        // Формирование постов в группах
        void FormMessagesFromVkPosts(List<Post> vkPostsToProcess)
        {
            foreach (var vkPost in vkPostsToProcess)
            {
                Message msg = new Message();
                string groupOrUserName;
                long senderId;

                if (!vkPost.OwnerId.HasValue)
                    continue;

                if (vkPost.OwnerId.Value < 0)
                {
                    groupOrUserName = "club" + (-vkPost.OwnerId.Value);
                }
                else
                {
                    groupOrUserName = "id" + vkPost.OwnerId.Value;
                }

                msg.MsgType = MsgTypes.Group;
                msg.MsgText = vkPost.Text;
                msg.MsgUrl = "https://vk.com/" + groupOrUserName + "?w=wall-" + Math.Abs(vkPost.OwnerId.Value) + "_" + vkPost.Id;
                msg.DomainUrl = "https://vk.com/" + groupOrUserName;
                msg.Id = -receivedPostsCounter;
                receivedPostsCounter--;

                if (vkPost.FromId.HasValue)
                    senderId = vkPost.FromId.Value;
                else
                    senderId = vkPost.OwnerId.Value;

                if (senderId > 0)
                {
                    if (usersDict.ContainsKey(senderId))
                    {
                        if ((usersDict[senderId].FirstName != null) && (usersDict[senderId].LastName != null))
                            msg.SenderName = usersDict[senderId].FirstName + " " + usersDict[senderId].LastName;
                        else if (usersDict[senderId].Nickname != null)
                            msg.SenderName = usersDict[senderId].Nickname;
                    }
                }
                else
                {
                    if (groupNamesDict.ContainsKey(-senderId))
                    {
                        msg.SenderName = groupNamesDict[-senderId];
                    }
                }

                //Console.WriteLine("Sender: " + msg.SenderName);
                //Console.WriteLine("Text: " + msg.MsgText);

                messageStack.Push(msg);
            }
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
            groupNamesDict = new Dictionary<long, string>();
            lastReceivingDateForGroups = new Dictionary<long,DateTime>();
        }
    }
}
