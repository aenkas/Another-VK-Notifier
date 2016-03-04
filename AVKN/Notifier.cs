using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AVKN
{
    public interface INotifier : IDisposable
    {
        bool ShowNotification(Notification n);

        bool ShowAuthError();

        bool ShowDefault();

        bool InitNotifier();

        bool SetContextMenu(ContextMenu niContextMenu);

        bool SetLaunchCallback(Func<bool> callback);
    }

    public class Notifier : INotifier
    {
        NotifyIcon ni;
        const string authErrorText = "Пользователь не авторизован";
        const string defaultText = "Нет новых уведомлений";
        const string havenewText = "Есть новые уведомления";
        string launchUrl;
        Func<bool> launchCallback;
        bool disposed = false;

        public bool ShowNotification(Notification n)
        {
            if (ni == null)
                return false;

            ni.Text = havenewText;
            launchUrl = n.NotificationUrl;

            if (string.IsNullOrEmpty(n.NotificationText)) {
                if (string.IsNullOrEmpty(n.NotificationHeader))
                    ni.ShowBalloonTip(9000, "", defaultText, ToolTipIcon.Info);
                else
                    ni.ShowBalloonTip(9000, "", n.NotificationHeader, ToolTipIcon.Info);
            }
            else
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(this.disposed == false)
            {
                if(disposing == true)
                {
                    if(ni != null)
                    {
                        ni.Dispose();
                        ni = null;
                    }
                }

                disposed = true;
            }
        }

        ~Notifier()
        {
            Dispose(false);
        }
    }
}
