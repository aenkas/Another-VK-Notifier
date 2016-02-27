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
            notificationHeader = "";
            notificationText = "";
            notificationUrl = "";
        }

        public bool BuildNotification()
        {
            if (messages.Count < 1)
                return false;

            if (messages.Count == 1)
            {
                Message firstMessage = messages[0];

                notificationHeader = firstMessage.SenderName;
                notificationText = firstMessage.MsgText;
                notificationUrl = firstMessage.MsgUrl;
            }
            else
            {
                int typesOfMessages = 0;
                string domainUrl = "";

                notificationHeader = "";
                notificationText = "У вас " + messages.Count + " непрочитанных сообщений";

                for (int i = 0; i < messages.Count; i++)
                {
                    typesOfMessages = typesOfMessages | (int)messages[i].MsgType;

                    if (string.Equals(domainUrl, ""))
                    {
                        domainUrl = messages[i].DomainUrl;
                    }
                    else if (!string.Equals(domainUrl, messages[i].DomainUrl))
                    {
                        switch (typesOfMessages)
                        {
                            case (int)MsgTypes.Personal:
                            case (int)MsgTypes.Dialog:
                                domainUrl = "https://vk.com/im";
                                break;
                            case (int)MsgTypes.Group:
                                domainUrl = "https://vk.com/groups";
                                break;
                            default:
                                domainUrl = "https://vk.com/";
                                break;
                        }
                    }
                }
                notificationUrl = domainUrl;
            }
            return true;
        }

        public Notification()
        {
            notificationHeader = "";
            notificationText = "";
            notificationUrl = "";
            messages = new List<Message>();
        }
    }
}
