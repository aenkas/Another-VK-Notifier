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
            catch (Exception e)
            {
                //MessageBox.Show(e.Message);
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
                int offset = 0;
                MessagesGetParams vkMsgParams = new MessagesGetParams();
                var usersDict = new Dictionary<long, VkNet.Model.User>();

                vkMsgParams.Count = 100;
                vkMsgParams.Offset = 0;
                vkMsgParams.TimeOffset = 0;
                vkMsgParams.Filters = MessagesFilter.All;

                MessagesGetObject vkMessages = vk.Messages.Get(vkMsgParams);//vk.Messages.Get(0, out offset, 100, 0, new TimeSpan(0), 0);
                
                foreach (var vkMessage in vkMessages.Messages)
                {
                    Message msg = new Message();

                    if(vkMessage.UserId.HasValue)
                    {
                        long vkMessageUserId = vkMessage.UserId.Value;

                        if (!usersDict.ContainsKey(vkMessageUserId))
                        {
                            User vkUser = vk.Users.Get(vkMessageUserId);

                            usersDict[vkMessageUserId] = vkUser;
                        }

                        if((usersDict[vkMessageUserId].FirstName != null) && (usersDict[vkMessageUserId].LastName != null))
                            msg.SenderName = usersDict[vkMessageUserId].FirstName + " " + usersDict[vkMessageUserId].LastName;
                        else if(usersDict[vkMessageUserId].Nickname != null)
                            msg.SenderName = usersDict[vkMessageUserId].Nickname;
                        else
                            msg.SenderName = vkMessage.Title;
                    } else
                    {
                        msg.SenderName = vkMessage.Title;
                    }

                    msg.MsgText = vkMessage.Body;
                    //msgtypes - хз
                    //msg.MsgUrl = m.
                    
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
            // if messageStack.Count == 0, then it will throw it's own exception when popping, so, do not handle this case?
            return messageStack.Pop();
        }

        public void ClearMsgStack()
        {
            messageStack.Clear();
        }

        public MsgReceiver()
        {

        }
    }
}
