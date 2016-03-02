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
    public partial class TwoFactorAuthForm : Form
    {
        public TwoFactorAuthForm()
        {
            InitializeComponent();
        }

        public string ShowDialogAndReturnKey()
        {
            this.ShowDialog();

            return this.authKeyTextBox.Text;
        }

        private void TwoFactorAuthForm_Load(object sender, EventArgs e)
        {

        }

        private void authButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
