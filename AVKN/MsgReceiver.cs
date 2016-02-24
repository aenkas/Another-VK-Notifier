using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AVKN
{
    public class MsgReceiver
    {
        public bool LogInVk(string login, string password)
        {
            return false;
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
