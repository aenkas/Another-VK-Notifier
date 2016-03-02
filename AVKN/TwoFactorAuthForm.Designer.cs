namespace AVKN
{
    partial class TwoFactorAuthForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.authKeyTextBox = new System.Windows.Forms.TextBox();
            this.authKeyLabel = new System.Windows.Forms.Label();
            this.authButton = new System.Windows.Forms.Button();
            this.infoGroupBox = new System.Windows.Forms.GroupBox();
            this.infoTextBox = new System.Windows.Forms.TextBox();
            this.infoGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // authKeyTextBox
            // 
            this.authKeyTextBox.Location = new System.Drawing.Point(79, 65);
            this.authKeyTextBox.Name = "authKeyTextBox";
            this.authKeyTextBox.PasswordChar = '*';
            this.authKeyTextBox.Size = new System.Drawing.Size(145, 20);
            this.authKeyTextBox.TabIndex = 0;
            // 
            // authKeyLabel
            // 
            this.authKeyLabel.AutoSize = true;
            this.authKeyLabel.Location = new System.Drawing.Point(79, 48);
            this.authKeyLabel.Name = "authKeyLabel";
            this.authKeyLabel.Size = new System.Drawing.Size(90, 13);
            this.authKeyLabel.TabIndex = 1;
            this.authKeyLabel.Text = "Полученный код";
            // 
            // authButton
            // 
            this.authButton.Location = new System.Drawing.Point(79, 106);
            this.authButton.Name = "authButton";
            this.authButton.Size = new System.Drawing.Size(144, 19);
            this.authButton.TabIndex = 2;
            this.authButton.Text = "Войти";
            this.authButton.UseVisualStyleBackColor = true;
            this.authButton.Click += new System.EventHandler(this.authButton_Click);
            // 
            // infoGroupBox
            // 
            this.infoGroupBox.Controls.Add(this.infoTextBox);
            this.infoGroupBox.Location = new System.Drawing.Point(12, 173);
            this.infoGroupBox.Name = "infoGroupBox";
            this.infoGroupBox.Size = new System.Drawing.Size(278, 103);
            this.infoGroupBox.TabIndex = 3;
            this.infoGroupBox.TabStop = false;
            // 
            // infoTextBox
            // 
            this.infoTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.infoTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.infoTextBox.Location = new System.Drawing.Point(10, 16);
            this.infoTextBox.Multiline = true;
            this.infoTextBox.Name = "infoTextBox";
            this.infoTextBox.ReadOnly = true;
            this.infoTextBox.Size = new System.Drawing.Size(258, 76);
            this.infoTextBox.TabIndex = 0;
            this.infoTextBox.Text = "Если логин и пароль указаны правильно, введите код двухфакторной авторизации и на" +
    "жмите \"Войти\"";
            // 
            // TwoFactorAuthForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(302, 288);
            this.Controls.Add(this.infoGroupBox);
            this.Controls.Add(this.authButton);
            this.Controls.Add(this.authKeyLabel);
            this.Controls.Add(this.authKeyTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "TwoFactorAuthForm";
            this.Text = "Двухфакторная авторизация";
            this.Load += new System.EventHandler(this.TwoFactorAuthForm_Load);
            this.infoGroupBox.ResumeLayout(false);
            this.infoGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox authKeyTextBox;
        private System.Windows.Forms.Label authKeyLabel;
        private System.Windows.Forms.Button authButton;
        private System.Windows.Forms.GroupBox infoGroupBox;
        private System.Windows.Forms.TextBox infoTextBox;
    }
}