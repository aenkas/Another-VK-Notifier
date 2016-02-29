using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Forms;

namespace AVKN
{
    class AppLogic
    {
        MsgReceiver receiver;
        Notifier notifier;
        Brain brain;
        ContextMenu contextMenu;
        System.Timers.Timer appTimer;

        public void RunApp()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            receiver = new MsgReceiver();
            notifier = new Notifier();
            brain = new Brain();

            if (!notifier.InitNotifier())
                throw new Exception();

            if (brain.LoadSettings())
                receiver.LogInVk(brain.Login, brain.Password);

            if (!receiver.IsLogged())
                ShowLoginWindow();

            SetupNotifyIconMenus();

            if (!notifier.SetContextMenu(contextMenu))
                throw new Exception();

            if (!notifier.SetLaunchCallback(LaunchCallback))
                throw new Exception();

            if (!brain.InitBrain(receiver, notifier))
                throw new Exception();

            appTimer = new System.Timers.Timer(5000);
            appTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            appTimer.Enabled = true;

            Application.ApplicationExit += AppExitEvent;
            Application.Run();
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            if (!receiver.IsLogged())
            {
                //MessageBox.Show("You must log in before receiving messages");

                return;
            }

            if (!brain.IncreaseEntropy())
                return;// MessageBox.Show("Cant receive new messages");
        }

        private void AppExitEvent(object sender, EventArgs e)
        {
            brain.SaveSettings();
            notifier.DestroyNotifier();
        }

        // Функция, вызываемая в notifier при нажатии левой кнопкой мыши перед открытием браузера.
        // Возвращает true, если требуется открытие браузера, иначе false
        private bool LaunchCallback()
        {
            if (!receiver.IsLogged())
            {
                ShowLoginWindow();

                return false;
            }

            return true;
        }

        private void SetupNotifyIconMenus()
        {
            MenuItem menuItemLogInVK = new MenuItem();
            MenuItem menuItemNotifyAboutDialogsCheckbox = new MenuItem();
            MenuItem menuItemNotifyAboutPersonalCheckbox = new MenuItem();
            MenuItem menuItemNotifyAboutGroupsCheckbox = new MenuItem();
            MenuItem menuItemExitApplication = new MenuItem();

            menuItemLogInVK.Text = "Вход";
            menuItemLogInVK.Click += LogInVK;

            menuItemNotifyAboutDialogsCheckbox.Text = "Показывать беседы";
            menuItemNotifyAboutDialogsCheckbox.Click += NotifyAboutDialogsCheckbox;
            menuItemNotifyAboutDialogsCheckbox.Checked = true;

            menuItemNotifyAboutPersonalCheckbox.Text = "Показывать личные сообщения";
            menuItemNotifyAboutPersonalCheckbox.Click += NotifyAboutPersonalCheckbox;
            menuItemNotifyAboutPersonalCheckbox.Checked = true;

            menuItemNotifyAboutGroupsCheckbox.Text = "Показывать группы";
            menuItemNotifyAboutGroupsCheckbox.Click += NotifyAboutGroupsCheckbox;
            menuItemNotifyAboutGroupsCheckbox.Checked = true;

            menuItemExitApplication.Text = "Выйти";
            menuItemExitApplication.Click += ExitApplication;

            contextMenu = new ContextMenu();

            contextMenu.MenuItems.Add(menuItemLogInVK);
            contextMenu.MenuItems.Add(menuItemNotifyAboutDialogsCheckbox);
            contextMenu.MenuItems.Add(menuItemNotifyAboutPersonalCheckbox);
            contextMenu.MenuItems.Add(menuItemNotifyAboutGroupsCheckbox);
            contextMenu.MenuItems.Add(menuItemExitApplication);
        }

        // Функции, отвечающие за обработку событий нажатия в меню.

        private void ShowLoginWindow()
        {
            LogInVKForm form = new LogInVKForm(brain);

            if (!notifier.SetContextMenu(null))
                throw new Exception();

            form.ShowDialog();

            if (receiver.LogInVk(brain.Login, brain.Password))
            {
                brain.SaveSettings();

                notifier.ShowDefault();
            }
            else
            {
                MessageBox.Show("Невозможно авторизоваться в ВКонтакте");

                notifier.ShowAuthError();

                brain.Login = "";
                brain.Password = "";
            }

            if (!notifier.SetContextMenu(contextMenu))
                throw new Exception();
        }

        private void LogInVK(object sender, EventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;

            ShowLoginWindow();

            /*if(receiver.IsLogged())
            {
                menuItem.Text = "Сменить аккаунт";
            } else
            {
                menuItem.Text = "Войти";
            }*/
        }

        private void NotifyAboutDialogsCheckbox(object sender, EventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;

            if (menuItem.Checked == true)
            {
                menuItem.Checked = false;
                brain.NotifyAboutDialogs = false;
            }
            else
            {
                menuItem.Checked = true;
                brain.NotifyAboutDialogs = true;
            }
        }

        private void NotifyAboutPersonalCheckbox(object sender, EventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;

            if (menuItem.Checked == true)
            {
                menuItem.Checked = false;
                brain.NotifyAboutPersonal = false;
            }
            else
            {
                menuItem.Checked = true;
                brain.NotifyAboutPersonal = true;
            }
        }

        private void NotifyAboutGroupsCheckbox(object sender, EventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;

            if (menuItem.Checked == true)
            {
                menuItem.Checked = false;
                brain.NotifyAboutGroups = false;
            }
            else
            {
                menuItem.Checked = true;
                brain.NotifyAboutGroups = true;
            }
        }

        private void ExitApplication(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
