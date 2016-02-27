using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VkNet;
using VkNet.Categories;
using VkNet.Utils;
using VkNet.Model;
using VkNet.Enums;
using VkNet.Exception;
using VkNet.Enums.Filters;
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
                MessageBox.Show(e.Message);
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
            try {
                int offset = 0;
                var messages = vk.Messages.Get(0, out offset, 100, 1, new TimeSpan(0), 0);
                foreach (VkNet.Model.Message m in messages)
                {
                    Message msg = new Message();
                    msg.MsgText = m.Body;
                    //msgtypes - хз
                    //msg.MsgUrl = m.
                    messageStack.Push(msg);
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
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
