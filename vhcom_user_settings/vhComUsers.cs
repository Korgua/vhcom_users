using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Drawing;
using System.IO;
using System.Management;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace VHCom_users {
	public partial class vhcomUserSettings : Form {
        userView userView = new userView();
		public vhcomUserSettings() {
			InitializeComponent();
            tabMenus.Selected += (sender, e) => {
                if(e.TabPageIndex == 0) {
                    userView.ConfigurationFilesListView();
                }
                else if(e.TabPageIndex == 1) {
                    userView.servicesListView();
                }
                else if(e.TabPageIndex == 2) {
                    userView.listWindowsUsers();
                }
            };
            userView.ConfigurationFilesListView();
		}
	}
}