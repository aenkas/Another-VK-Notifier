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

        public Form1()
        {
            InitializeComponent();

            this.FormClosing += Form1_FormClosing;

            receiver = new MsgReceiver();
            notifier = new Notifier();
            brain = new Brain();

            if (!notifier.InitNotifier())
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
            if (brain.LoadSettings())
            {
                loginTextBox.Text = brain.Login;
                passwordTextBox.Text = brain.Password;

                if (receiver.LogInVk(brain.Login, brain.Password))
                {
                    loginButton.Text = "Сменить аккаунт";
                }
                else
                {
                    MessageBox.Show("Cant log in VK");

                    loginButton.Text = "Войти";
                }
            }
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            string login = loginTextBox.Text;
            string password = passwordTextBox.Text;

            if (receiver.LogInVk(login, password))
            {
                brain.Login = login;
                brain.Password = password;

                if (!brain.SaveSettings())
                    MessageBox.Show("Cant save new login/password");

                loginButton.Text = "Сменить аккаунт";
            }
            else
            {
                MessageBox.Show("Cant log in VK");

                if(!receiver.IsLogged())
                    loginButton.Text = "Войти";
            }
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
    }
}
