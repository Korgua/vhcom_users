using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Drawing;
using System.IO;
using System.Management;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace VHCom_users {
	public partial class vhcomUserSettings : Form{
		/*public List<vhcomUser> vhUser = new List<vhcomUser>();
		public List<winUser> winUser = new List<winUser>();*/
		/*public UserController WUF = new UserController();
		public OuterFunctions OF = new OuterFunctions();
		*/
		public userView userView = new userView();
		public logging log = new logging();

		public vhcomUserSettings() {
			InitializeComponent();
            tabMenus.Selected += (sender, e) => {
                if(e.TabPageIndex == 0) {
                    userView.ConfigurationFilesListView(tabSpecialUsers);
                }
                else if(e.TabPageIndex == 1) {
                    userView.servicesListView(tabServices);
                }
                else if(e.TabPageIndex == 2) {
                    userView.listWindowsUsers(tabWindowsUsers);
                }
            };
            userView.ConfigurationFilesListView(tabSpecialUsers);
		}
	}
}