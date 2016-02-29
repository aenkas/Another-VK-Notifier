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
        const string authErrorText = "Пользователь не авторизован";
        const string defaultText = "Нет новых уведомлений";
        const string havenewText = "Есть новые уведомления";
        string launchUrl;
        Func<bool> launchCallback;

        public bool ShowNotification(Notification n)
        {
            if (ni == null)
                return false;

            ni.Text = havenewText;
            launchUrl = n.NotificationUrl;

            ni.ShowBalloonTip(9000, n.NotificationHeader, n.NotificationText, ToolTipIcon.Info);

            return true;
        }

        public bool ShowAuthError()
        {
            if (ni == null)
                return false;

            ni.Text = authErrorText;
            launchUrl = "";

            return false;
        }

        public bool ShowDefault()
        {
            if (ni == null)
                return false;

            ni.Text = defaultText;
            launchUrl = "https://vk.com/";

            return true;
        }

        public bool InitNotifier()
        {
            ni = new NotifyIcon();

            ni.Text = defaultText;
            ni.Icon = SystemIcons.Application;
            ni.DoubleClick += ProcessNILMBClicks;
            ni.BalloonTipClicked += ProcessNILMBClicks;
            ni.Visible = true;

            return true;
        }

        public bool DestroyNotifier()
        {
            ni.Dispose();
            ni = null;

            return true;
        }

        public bool SetContextMenu(ContextMenu niContextMenu)
        {
            if (ni == null)
                return false;

            ni.ContextMenu = niContextMenu;

            return true;
        }

        public bool SetLaunchCallback(Func<bool> callback)
        {
            launchCallback = callback;

            return true;
        }

        public Notifier()
        {
            launchUrl = "";
            launchCallback = null;
        }

        private void ProcessNILMBClicks(object sender, EventArgs e)
        {
            ni.DoubleClick -= ProcessNILMBClicks;
            ni.BalloonTipClicked -= ProcessNILMBClicks;

            try {
                if (launchCallback != null)
                    if (!launchCallback())
                        return;

                if (!string.IsNullOrEmpty(launchUrl))
                    System.Diagnostics.Process.Start(launchUrl);

                ShowDefault();
            }
            finally
            {
                ni.DoubleClick += ProcessNILMBClicks;
                ni.BalloonTipClicked += ProcessNILMBClicks;
            }
        }
    }
}
