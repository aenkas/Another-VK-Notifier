using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AVKN
{
    class Notification
    {
        public string NotificationHeader { get; set; }

        public string NotificationText { get; set; }

        public string NotificationUrl { get; set; }

        public bool AddMessage(Message m)
        {
            return false;
        }

        public void ClearMsgQueue()
        {
            throw new NotImplementedException();
        }

        public bool BuildNotification()
        {
            return false;
        }

        public Notification()
        {

        }
    }
}
