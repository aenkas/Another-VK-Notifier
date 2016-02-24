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
            return false;
        }

        public bool IncreaseEntropy()
        {
            return false;
        }

        public bool LoadSettings()
        {
            return false;
        }

        public bool SaveSettings()
        {
            return false;
        }

        public Brain()
        {
            notifyAboutPersonal = true;
            notifyAboutDialogs = true;
            notifyAboutGroups = true;
            login = "";
            password = "";
        }
    }
}
