using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AVKN
{
    public class Notification
    {
        string notificationHeader;
        string notificationText;
        string notificationUrl;

        List<Message> messages;

        public string NotificationHeader
        {
            get
            {
                return notificationHeader;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                notificationHeader = value;
            }
        }

        public string NotificationText
        {
            get
            {
                return notificationText;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                notificationText = value;
            }
        }

        public string NotificationUrl
        {
            get
            {
                return notificationUrl;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                notificationUrl = value;
            }
        }

        public bool AddMessage(Message m)
        {
            try
            {
                messages.Add(m);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public void ClearMsgQueue()
        {
            messages.Clear();
        }

        public bool BuildNotification()
        {
            return false;
        }

        public Notification()
        {
            notificationHeader = "";
            notificationText = "";
            notificationUrl = "";
        }
    }
}
