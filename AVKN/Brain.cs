using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AVKN
{
    class Brain
    {
        public bool NotifyAboutPersonal { get; set; }

        public bool NotifyAboutDialogs { get; set; }

        public bool NotifyAboutGroups { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

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

        }
    }
}
