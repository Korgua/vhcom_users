namespace VHCom_users
{
    public partial class vhcomUserSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(vhcomUserSettings));
            this.tabMenus = new System.Windows.Forms.TabControl();
            this.tabSpecialUsers = new System.Windows.Forms.TabPage();
            this.tabServices = new System.Windows.Forms.TabPage();
            this.tabWindowsUsers = new System.Windows.Forms.TabPage();
            this.tabOtherSettings = new System.Windows.Forms.TabPage();
            this.tabMenus.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabMenus
            // 
            this.tabMenus.Controls.Add(this.tabSpecialUsers);
            this.tabMenus.Controls.Add(this.tabServices);
            this.tabMenus.Controls.Add(this.tabWindowsUsers);
            this.tabMenus.Controls.Add(this.tabOtherSettings);
            this.tabMenus.Location = new System.Drawing.Point(3, 3);
            this.tabMenus.Name = "tabMenus";
            this.tabMenus.SelectedIndex = 0;
            this.tabMenus.Size = new System.Drawing.Size(540, 442);
            this.tabMenus.TabIndex = 8;
            // 
            // tabSpecialUsers
            // 
            this.tabSpecialUsers.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tabSpecialUsers.Location = new System.Drawing.Point(4, 22);
            this.tabSpecialUsers.Name = "tabSpecialUsers";
            this.tabSpecialUsers.Size = new System.Drawing.Size(532, 416);
            this.tabSpecialUsers.TabIndex = 0;
            this.tabSpecialUsers.Text = "VH Com Konfiguráció";
            // 
            // tabServices
            // 
            this.tabServices.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tabServices.Location = new System.Drawing.Point(4, 22);
            this.tabServices.Name = "tabServices";
            this.tabServices.Padding = new System.Windows.Forms.Padding(3);
            this.tabServices.Size = new System.Drawing.Size(532, 416);
            this.tabServices.TabIndex = 1;
            this.tabServices.Text = "Szolgáltatások";
            // 
            // tabWindowsUsers
            // 
            this.tabWindowsUsers.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tabWindowsUsers.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tabWindowsUsers.Location = new System.Drawing.Point(4, 22);
            this.tabWindowsUsers.Name = "tabWindowsUsers";
            this.tabWindowsUsers.Size = new System.Drawing.Size(532, 416);
            this.tabWindowsUsers.TabIndex = 2;
            this.tabWindowsUsers.Text = "Windows Felhasználók";
            // 
            // tabOtherSettings
            // 
            this.tabOtherSettings.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tabOtherSettings.Location = new System.Drawing.Point(4, 22);
            this.tabOtherSettings.Margin = new System.Windows.Forms.Padding(0);
            this.tabOtherSettings.Name = "tabOtherSettings";
            this.tabOtherSettings.Size = new System.Drawing.Size(532, 416);
            this.tabOtherSettings.TabIndex = 3;
            this.tabOtherSettings.Text = "Egyéb beállítások";
            // 
            // vhcomUserSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(539, 485);
            this.Controls.Add(this.tabMenus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "vhcomUserSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "VHCom Felhasználók";
            this.tabMenus.ResumeLayout(false);
            this.ResumeLayout(false);

        }

		#endregion
        public System.Windows.Forms.TabControl tabMenus;
        public System.Windows.Forms.TabPage tabSpecialUsers;
        public System.Windows.Forms.TabPage tabServices;
        public System.Windows.Forms.TabPage tabWindowsUsers;
        private System.Windows.Forms.TabPage tabOtherSettings;
    }
}

