using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AVKN
{
    public class Brain
    {
        bool notifyAboutPersonal;
        bool notifyAboutDialogs;
        bool notifyAboutGroups;
        string login;
        string password;
        MsgReceiver brainsMessageReceiver;
        Notifier brainsNotifier;
        bool isInit;
        List<long> lastIds;

        public bool NotifyAboutPersonal
        {
            get
            {
                return notifyAboutPersonal;
            }
            set
            {
                notifyAboutPersonal = value;
            }
        }

        public bool NotifyAboutDialogs
        {
            get
            {
                return notifyAboutDialogs;
            }
            set
            {
                notifyAboutDialogs = value;
            }
        }

        public bool NotifyAboutGroups
        {
            get
            {
                return notifyAboutGroups;
            }
            set
            {
                notifyAboutGroups = value;
            }
        }

        public string Login
        {
            get
            {
                return login;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                login = value;
            }
        }

        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                password = value;
            }
        }

        public bool InitBrain(MsgReceiver mr, Notifier notifier)
        {
            if(mr == null || notifier == null)
                return false;

            brainsMessageReceiver = mr;
            brainsNotifier = notifier;
            isInit = true;

            return true;
        }

        public bool IncreaseEntropy()
        {
            Notification notification = new Notification();
            bool hasNewMessages = false;

            if (!isInit)
                return false;

            if (!brainsMessageReceiver.IsLogged())
                return false;

            brainsMessageReceiver.RetrieveMessages();

            notification.ClearMsgQueue();

            while (brainsMessageReceiver.GetMessagesCount() > 0)
            {
                Message message = brainsMessageReceiver.PopFirstMsg();

                if (message == null)
                    break;

                if ((message.MsgType == MsgTypes.Personal) && (notifyAboutPersonal == false))
                    continue;

                if ((message.MsgType == MsgTypes.Dialog) && (notifyAboutDialogs == false))
                    continue;

                if ((message.MsgType == MsgTypes.Group) && (notifyAboutGroups == false))
                    continue;

                if (!lastIds.Contains(message.Id))
                {
                    lastIds.Add(message.Id);

                    hasNewMessages = true;
                }

                notification.AddMessage(message);
            }

            if (hasNewMessages == true)
                if (notification.BuildNotification())
                    brainsNotifier.ShowNotification(notification);

            return true;
        }

        public bool LoadSettings()
        {
            return false;
        }

        public bool SaveSettings()
        {
            return false;
        }

        public void BrainDrain()
        {
            lastIds.Clear();
        }

        public Brain()
        {
            notifyAboutPersonal = true;
            notifyAboutDialogs = true;
            notifyAboutGroups = true;
            login = "";
            password = "";
            isInit = false;
            lastIds = new List<long>();
        }
    }
}
