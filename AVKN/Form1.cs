using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AVKN
{
    public partial class Form1 : Form
    {
        MsgReceiver receiver;
        Notifier notifier;
        Brain brain;
        ContextMenu contextMenu;

        public Form1()
        {
            InitializeComponent();

            this.FormClosing += Form1_FormClosing;

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

            if (!brain.InitBrain(receiver, notifier))
                throw new Exception();
        }

        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            brain.SaveSettings();
            notifier.DestroyNotifier();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void receiveButton_Click(object sender, EventArgs e)
        {
            if (!receiver.IsLogged())
            {
                MessageBox.Show("You must log in before receiving messages");

                return;
            }

            if (!brain.IncreaseEntropy())
                MessageBox.Show("Cant receive new messages");
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

            form.ShowDialog();

            if (receiver.LogInVk(brain.Login, brain.Password))
            {
                if (!brain.SaveSettings())
                    MessageBox.Show("Невозможно сохранить логин и пароль");
            }
            else
            {
                MessageBox.Show("Невозможно авторизоваться в ВКонтакте");

                notifier.ShowAuthError();

                brain.Login = "";
                brain.Password = "";
            }
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

            if(menuItem.Checked == true)
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
