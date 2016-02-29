using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VkNet;
using VkNet.Categories;
using VkNet.Utils;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.Auth;
using VkNet.Model.RequestParams;
using VkNet.Enums;
using VkNet.Enums.Filters;
using VkNet.Exception;
using System.Windows.Forms;

namespace AVKN
{
    public class MsgReceiver
    {
        private bool isLogged;
        private Stack<Message> messageStack = new Stack<Message>();
        private VkApi vk = new VkApi();
        Dictionary<long, VkNet.Model.User> usersDict;

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
                vk.Authorize(auth);
            }
            catch (Exception)
            {
                return false;
            }
            return (isLogged = true);
        }

        public bool IsLogged()
        {
            return isLogged;
        }

        public bool RetrieveMessages()
        {
            /*try */{
                MessagesGetParams vkMsgParams = new MessagesGetParams();
                List<long> userIdsToGet = new List<long>(); 

                vkMsgParams.Count = 100;
                vkMsgParams.Offset = 0;
                vkMsgParams.TimeOffset = 0;
                vkMsgParams.Filters = MessagesFilter.All;

                MessagesGetObject vkMessages = vk.Messages.Get(vkMsgParams);//vk.Messages.Get(0, out offset, 100, 0, new TimeSpan(0), 0);

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

                    foreach(var vkUser in vkUsers)
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
                        if (vkMessage.ReadState.Value == MessageReadState.Readed)
                            continue;

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
                            else
                                msg.SenderName = vkMessage.Title;
                        }
                    } 

                    if(string.IsNullOrEmpty(msg.SenderName))
                        msg.SenderName = vkMessage.Title;

                    if(vkMessage.Id.HasValue)
                        msg.Id = vkMessage.Id.Value;

                    msg.MsgText = vkMessage.Body;

                    if (vkMessage.ChatId.HasValue)
                    {
                        msg.MsgType = MsgTypes.Dialog;
                        if (vkMessage.ChatId.HasValue)
                        {
                            msg.MsgUrl = "https://vk.com/im?msgid=" + msg.Id.ToString() + "&sel=c" + vkMessage.ChatId.Value;
                            msg.DomainUrl = "https://vk.com/im?sel=c" + vkMessageUserId.ToString() + vkMessage.ChatId.Value;
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
            /*catch(Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }*/
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
        }
    }
}
