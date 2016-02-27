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
        public bool LogInVk(string login, string password)
        {
            try
            {
                var auth = new ApiAuthParams();

                Settings scope = Settings.All;
                VkApi vk = new VkApi();
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
            return true;
        }

        public bool IsLogged()
        {
            return false;
        }

        public bool RetrieveMessages()
        {
            return false;
        }

        public int GetMessagesCount()
        {
            return 0;
        }

        public Message PopFirstMsg()
        {
            return new Message();
        }

        public void ClearMsgStack()
        {
            throw new NotImplementedException();
        }

        public MsgReceiver()
        {

        }
    }
}
