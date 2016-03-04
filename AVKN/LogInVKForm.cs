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
    public partial class LogInVKForm : Form
    {
        Brain brainRef;

        public LogInVKForm(Brain brain)
        {
            InitializeComponent();

            brainRef = brain;
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            brainRef.Login = loginTextBox.Text;
            brainRef.Password = passwordTextBox.Text;

            this.DialogResult = DialogResult.OK;
        }

        private void LogInVKForm_Load(object sender, EventArgs e)
        {

        }
    }
}
