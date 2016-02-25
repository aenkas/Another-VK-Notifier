using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AVKN
{
    public class Notifier
    {
        NotifyIcon ni;
        const string authErrorText = "Пользователь не аутентифицирован";
        const string defaultText = "Нет новых уведомлений";

        public bool ShowNotification(Notification n)
        {
            if (ni == null)
                return false;

            ni.Text = n.NotificationHeader;

            ni.ShowBalloonTip(9000, n.NotificationHeader, n.NotificationText, ToolTipIcon.Info);

            return true;
        }

        public bool ShowAuthError()
        {
            if (ni == null)
                return false;

            ni.Text = authErrorText;

            return false;
        }

        public bool ShowDefault()
        {
            if (ni == null)
                return false;

            ni.Text = defaultText;

            return true;
        }

        public bool InitNotifier()
        {
            ni = new NotifyIcon();

            ni.Text = defaultText;
            ni.Icon = SystemIcons.Application;
            ni.Visible = true;

            return true;
        }

        public bool DestroyNotifier()
        {
            ni.Dispose();
            ni = null;

            return true;
        }

        public Notifier()
        {
            
        }
    }
}
