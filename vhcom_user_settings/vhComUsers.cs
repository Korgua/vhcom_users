using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Drawing;
using System.IO;
using System.Management;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace vhcom_user_settings {
	public partial class vhcomUserSettings : Form {
		public int vhcomUserInstance = 0;

		logging log = new logging();

		List<vhcomUser> vhUser = new ConfigFiles().getVhComUsers();
		List<winUser> winUser = new List<winUser>();
		List<Exception> ex = new List<Exception>();
		winUserFunctions WUF = new winUserFunctions();
		OuterFunctions OF = new OuterFunctions();
		
		zipNr zipNr = new zipNr();
		private List<string> error = new List<string>();
		public vhcomUserSettings() {
			InitializeComponent();

			log.writeToLog(null, "EnumSettings Start");
			//createHeadersInListView();

			//enumSettings();
			log.writeToLog(null, "createHeadersInListView Start");
			log.writeToLog(null, "showErrors Start");
			showColorDescription();
			log.writeToLog(null, "servicesListView Start");
			//servicesListView();
			log.writeToLog(null, "enumUsers Start");
			enumUsers();
			log.writeToLog(null, "End Start");

			//listWindowsUsers();
			/*winUser alma;
            alma = new winUser();
            alma.name = "alma");
            //alma.password = "alma");
            alma.group = "Rendszergazdák");
            alma.group = "Asztal távoli felhasználói");
            WUF.addWinUser(alma);
            WUF.RenameWinUser(alma, "sanyi");

            alma = new winUser();
            alma.name = "Pénztár");
            WUF.addWinUser(alma);*/

			/*
             * WUF.removeWinUser(alma);
            winUser.Remove(alma);
            
            */
			tabMenus.Selected += TabControl1_Selected;
			createHeadersInListView();
		}

		private void TabControl1_Selected(Object sender, TabControlEventArgs e) {
			if( e.TabPageIndex == 0 ) {
				createHeadersInListView();
			}
			else if( e.TabPageIndex == 1 ) {
				servicesListView();
			}
			else if( e.TabPageIndex == 2 ) {
				listWindowsUsers();
			}
		}


		public ListView createListView(Dictionary<string, int> header) {
			ListView lv = new ListView();
			lv.FullRowSelect = true;
			lv.View = View.Details;
			lv.AllowColumnReorder = false;
			lv.AllowDrop = false;
			lv.Top = 0;
			lv.Left = 0;
			lv.BackColor = Color.WhiteSmoke;
			lv.GridLines = true;
			lv.MultiSelect = false;

			// Prevent the column resize
			lv.ColumnWidthChanging += (e, sender) => {
				ColumnWidthChangingEventArgs arg = (ColumnWidthChangingEventArgs)sender;
				arg.Cancel = true;
				arg.NewWidth = lv.Columns[arg.ColumnIndex].Width;
			};
			bool first = true;
			foreach( KeyValuePair<string, int> kvp in header ) {
				lv.Columns.Add(kvp.Key.ToString(), kvp.Value, first == true ? HorizontalAlignment.Left : HorizontalAlignment.Center);
				if( first )
					first = false;

			}
			int width = 5;
			foreach( ColumnHeader ch in lv.Columns ) {
				width += ch.Width;
			}
			lv.Width = width;
			lv.Sorting = SortOrder.Ascending;

			return lv;
		}

		public void selectedWindowsUsersFromListView(winUser winUser, int Top) {
			foreach( Control item in tabWindowsUsers.Controls ) {
				if( item.GetType().ToString() == "System.Windows.Forms.Button" ) {
					tabWindowsUsers.Controls.Remove(item);
				}
			}

			Button btn_delete = new Button();
			Button btn_modify = new Button();
			int btn_width = 140;

			btn_delete.Text = "Felhasználó törlése";
			btn_delete.Width = btn_width;
			btn_delete.Left = tabWindowsUsers.Width - btn_width - 5;
			btn_delete.Top = Top + 5;
			btn_delete.Name = "Delete";

			btn_delete.MouseClick += (e, sender) => {
				MouseEventArgs arg = (MouseEventArgs)sender;
				if( WUF.deleteWindowsUserConfirmation(winUser) ) {
					listWindowsUsers();
				}
			};

			btn_modify.Text = "Felhasználó módosítása";
			btn_modify.Width = btn_width;
			btn_modify.Left = tabWindowsUsers.Width - ((btn_width+5) * 2);
			btn_modify.Top = Top + 5;
			btn_modify.Name = "Modify";

			btn_modify.MouseClick += (e, sender) => {
				MouseEventArgs arg = (MouseEventArgs)sender;
					editWindowsUserForm(winUser);
			};
			tabWindowsUsers.Controls.Add(btn_delete);
			tabWindowsUsers.Controls.Add(btn_modify);
		}

		public void editWindowsUserForm(winUser winUser) {
			WindowsFormUser WindowsFormUser = new WindowsFormUser();
			bool allSet = false;
			string newUserName, password;
			bool rdp , admin, users, enabled;
			WindowsFormUser.btn_winuser_confirm.MouseClick += (e, sender) => {
				if(WindowsFormUser.txt_newusername.Text.Length == 0) {
					WindowsFormUser.txt_newusername.BackColor = Color.LightPink;
					newUserName = null;
					allSet = false;
				}
				else {
					WindowsFormUser.txt_newusername.BackColor = Color.WhiteSmoke;
					newUserName = WindowsFormUser.txt_newusername.Text;
					allSet = true;
				}
				if(WindowsFormUser.txt_password.Text.Length == 0) {
					WindowsFormUser.txt_password.BackColor = Color.LightPink;
					password = null;
					allSet = false;
				}
				else {
					WindowsFormUser.txt_password.BackColor = Color.WhiteSmoke;
					password = WindowsFormUser.txt_password.Text;
					allSet = true;
				}
				if(WindowsFormUser.chk_grpusers.Checked) {
					users = true;
				}
				else {
					users = false;
				}
				if(WindowsFormUser.chk_grpadmin.Checked) {
					admin = true;
				}
				else {
					admin = false;
				}
				if(WindowsFormUser.chk_grprdp.Checked) {
					rdp = true;
				}
				else {
					rdp = false;
				}
				if(WindowsFormUser.chk_enabled.Checked) {
					enabled = true;
				}
				else {
					enabled = false;
				}

				if(allSet && (admin || rdp || users)) {
					DialogResult choice = MessageBox.Show(winUser.name.ToUpper() + " módosításra kerül!\nValóban módosítani akarod?", "Megerősítés", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
					if(choice == DialogResult.Yes) {
						WindowsFormUser.Dispose();
						listWindowsUsers();
					}
				}
				else {
					MessageBox.Show("Legealább 1 csoport kötelező!\nAmi piros, hiányos...", "Figyelmeztetés", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			};
				WindowsFormUser.ShowDialog();
		}

		public void listWindowsUsers() {
			enumSettings();
			log.writeToLog(null, "[ListWindowsUsers] Begin");
			Dictionary<string, int> headers = new Dictionary<string, int>();
			headers.Add("Felhasználónév", 160);
			headers.Add("Rendszergazda?", 120);
			headers.Add("Távoli?", 120);
			headers.Add("Aktív?", 120);
			ListView lv = createListView(headers);

			log.writeToLog(null, "[ListWindowsUsers] Listview created");
			foreach( winUser winUser in winUser ) {
				ListViewItem lvi = new ListViewItem(winUser.name);
				lvi.SubItems.Add(winUser.admin ? "igen" : "nem");
				lvi.SubItems.Add(winUser.rdp ? "igen" : "nem");
				lvi.SubItems.Add(winUser.enabled ? "igen" : "nem");
				if( winUser.enabled ) {
					lvi.BackColor = Color.LightGreen;
				}
				else {
					lvi.BackColor = Color.LightPink;
				}
				lv.Items.Add(lvi);
				lvi = null;
			}
			ListViewItem lastItm = lv.Items[lv.Items.Count - 1];
			lv.ClientSize = new Size(lv.ClientSize.Width, lv.Top + lastItm.Bounds.Bottom);


			lv.MouseDoubleClick += (e, sender) => {
				MouseEventArgs arg = (MouseEventArgs)sender;
				ListViewHitTestInfo info = lv.HitTest(arg.X, arg.Y);
				ListViewItem lvi = info.Item;
				foreach(winUser winUser in winUser) {
					if(winUser.name == lvi.Text) {
						editWindowsUserForm(winUser);
						break;
					}
				}
			};


				lv.KeyDown += (e, sender) => {
				KeyEventArgs arg = (KeyEventArgs)sender;
				bool found = false;
				if(arg.KeyCode.ToString() == "Delete") {
					foreach(ListViewItem lvi in lv.SelectedItems) {
						foreach(winUser winUser in winUser) {
							if(winUser.name == lvi.Text) {
								if(WUF.deleteWindowsUserConfirmation(winUser)) {
									listWindowsUsers();
								}
								found = true;
								break;
							}
						}
						if(found) {
							break;
						}
					}
				}
			};

			lv.ItemSelectionChanged += (e, sender) => {
				ListViewItemSelectionChangedEventArgs arg = (ListViewItemSelectionChangedEventArgs)sender;
				ListViewItem item = arg.Item;
				if( item != null ) {
					foreach( winUser winUser in winUser ) {
						if( winUser.name == item.Text ) {
							selectedWindowsUsersFromListView(winUser, lv.Height);
							break;
						}
					}
				}
				else {
					lv.SelectedItems.Clear();
					MessageBox.Show("Nincs kiválasztva sor, próbáld újra", "Hiba", MessageBoxButtons.OK);
				}

			};

			tabWindowsUsers.Controls.Clear();
			tabWindowsUsers.Controls.Add(lv);
			log.writeToLog(null, "[ListWindowsUsers] End");
		}




		//  Shows the existing VH COM services
		//  Like PumpService, OFSync, etc...
		public void servicesListView() {
			Dictionary<string, string> serviceList = new Dictionary<string, string>();
			serviceList.Add("pumpsrvc", "PumpService");
			serviceList.Add("wuauserv", "windows update");
			serviceList.Add("Service5", "InfoReporterClient");
			serviceList.Add("Service4", "InfoReporterServer");
			serviceList.Add("OfSync", "OfSync");
			serviceList.Add("OfSyncUpdater", "OfSyncUpdater");
			serviceList.Add("msiserver", "Windows Installer");
			serviceList.Add("MpsSvc", "Windows Tűzfal");

			Dictionary<string, string> ServiceStartType = new Dictionary<string, string>();
			ServiceStartType.Add("Auto", "Automatikus");
			ServiceStartType.Add("Manual", "Kézi");
			ServiceStartType.Add("Disabled", "Letiltva");
			ServiceStartType.Add("Stopped", "Leállítva");
			ServiceStartType.Add("Running", "Fut");

			// Prevent the column resize

			Dictionary<string, int> windowsServices = new Dictionary<string, int>();
			windowsServices.Add("Szolgáltatás", 140);
			windowsServices.Add("Futattó", 160);
			windowsServices.Add("Indítás", 110);
			windowsServices.Add("Állapot", 110);

			Font stdfont = new Font("Arial", 8.5f, FontStyle.Regular);

			ListView lv = createListView(windowsServices);
			lv.Font = stdfont;
			ListViewItem serviceName = null;
			ManagementObjectCollection queryCollection = OF.wqlQuery("select Name, StartName, StartMode, State from Win32_Service");// where name='"+kv.Key+"'");
			foreach( ManagementObject service in queryCollection ) {
				string dictKeyToDelete = null;
				foreach( KeyValuePair<string, string> kv in serviceList ) {
					if( service["Name"].ToString().ToLower() == kv.Key.ToLower() ) {
						dictKeyToDelete = kv.Key;
						serviceName = new ListViewItem(kv.Value);
						serviceName.SubItems.Add(service["StartName"].ToString());
						serviceName.SubItems.Add(ServiceStartType[service["StartMode"].ToString()]);
						serviceName.SubItems.Add(ServiceStartType[service["State"].ToString()]);
						if( service["StartMode"].ToString().ToLower() == "disabled" || service["State"].ToString().ToLower() == "stopped" ) {
							serviceName.BackColor = Color.LightPink;
						}
						else {
							serviceName.BackColor = Color.LightGreen;
						}
						Console.WriteLine("Font size: {0}", serviceName.Font.Size);
						serviceName.Font = stdfont;
						lv.Items.Add(serviceName);
						Console.WriteLine("Position Y: {0}", serviceName.Position.Y);
						break;
					}
				}
				//It is forbidden to delete a key from Dictionary in a loop
				if( dictKeyToDelete != null ) {
					serviceList.Remove(dictKeyToDelete);
					dictKeyToDelete = null;
				}
			}
			foreach( KeyValuePair<string, string> kv in serviceList ) {
				log.writeToLog(null, string.Format("Unknown services: {0}", kv.Key));
				serviceName = new ListViewItem(kv.Value);
				serviceName.SubItems.Add("Nincs adat");
				serviceName.SubItems.Add("Nincs adat");
				serviceName.SubItems.Add("Nincs adat");
				serviceName.BackColor = Color.FromArgb(1, 255, 184, 41);
				lv.Items.Add(serviceName);
			}
			ListViewItem lastItm = lv.Items[lv.Items.Count - 1];
			lv.ClientSize = new Size(lv.ClientSize.Width, lv.Top + lastItm.Bounds.Bottom);
			//lv.Height = lv.Items.Count * 22;
			tabServices.Controls.Clear();
			tabServices.Controls.Add(lv);
		}

		public void enumUsers() {
			/*if (vhUser.Count != 0 && error.Count != 0) {
                usersGroup.Top = colorDescription.Height + special_users.Height + 5;
                irsz_group.Top = usersGroup.Top + usersGroup.Height + 5;
            }
            else if (error.Count != 0 && vhUser.Count == 0) {
                usersGroup.Top = colorDescription.Height + 30;
                irsz_group.Top = usersGroup.Top + usersGroup.Height + 5;
            }
            else if (vhUser.Count != 0 && error.Count == 0) {
                usersGroup.Top = special_users.Height + 30;
                irsz_group.Top = usersGroup.Top + usersGroup.Height + 5;
            }
            else {
                usersGroup.Top = 0;
                irszTxtBox.Top = usersGroup.Height + 10;
            }
            iIsSet.Text = "nem";
            pIsSet.Text = "nem";
            if (winUser.Count > 0) {
                foreach (winUser w in winUser) {
                    if (w.name.ToLower() == "pénztár") {
                        pIsSet.Text = "igen";
                        pCreateUser.Enabled = false;
                        pCreateUser.Checked = false;
                    }

                    if (w.name.ToLower() == "iroda") {
                        iIsSet.Text = "igen";
                        iCreateUser.Enabled = false;
                        iCreateUser.Checked = false;
                    }
                }
            }
            pPsw.Text = "P+irsz";
            iPsw.Text = "Iroda+irsz";*/
		}

		//  Check if the VHCOM configuration files, 
		//  such as c:\ofsync.exe.config/database.xml exists
		//  If not readable (access restricted, not exists), it shows here
		public void showColorDescription() {
			Label green = new Label();
			green.Text = "Az adott fájl megvan, a név és jelszó sa/PE1267cs";
			green.ForeColor = System.Drawing.Color.Green;
			green.Font = new Font(green.Font, FontStyle.Bold);
			green.Top = 15;
			green.Left = 20;
			green.Width = 300;
			//colorDescription.Top = special_users.Height + 30;
			colorDescription.BackColor = System.Drawing.Color.White;
			colorDescription.Controls.Add(green);
			//colorDescription.Visible = true;

		}

		//  Search for the already exist VHCOM windows users
		//  Reset its properties if necessary
		//  e.g: iroda is Administrator, Pénztár is Remote Desktop User
		public void enumSettings() {
			winUser.Clear();
			PrincipalContext context = new PrincipalContext(ContextType.Machine);
			UserPrincipal UserPrincipal = new UserPrincipal(context);
			PrincipalSearcher PrincipalSearcher = new PrincipalSearcher(UserPrincipal);
			winUser temp = null;
			winUserFunctions WUF = new winUserFunctions();
			foreach( UserPrincipal result in PrincipalSearcher.FindAll() ) {
				//I was able to delete my own, active profile.... FFFFFFFUUUUUUCCCCCCCKKKKK
				if( Environment.UserName != result.SamAccountName ) {
					temp = new winUser();
					temp.name = result.SamAccountName;
					WUF.getGroupsByUser(temp, result, PrincipalSearcher);
					WUF.getUserStatus(temp);
					winUser.Add(temp);
					temp = null;
				}
			}
			/* log.writeToLog(null, "Userek:");
             foreach(winUser winUser in winUser) {
                 log.writeToLog(null, "------------------------------");
                 log.writeToLog(null, "Név: " + winUser.name);
                 log.writeToLog(null, "engedélyezve: "+winUser.enabled);
                 log.writeToLog(null, "Csoportjai: ");
                 log.writeToLog(winUser.group, null);
             }*/
		}

		public void createHeadersInListView() {
			Dictionary<string, int> headers = new Dictionary<string, int>();
			headers.Add("Micsoda", 100);
			headers.Add("SQL-hez név", 75);
			headers.Add("SQL-hez jelszó", 100);
			headers.Add("Útvonal", 245);
			ListView lv = createListView(headers);
			lv.MouseDoubleClick += (e, sender) => {
				MouseEventArgs arg = (MouseEventArgs)sender;
				System.Diagnostics.ProcessStartInfo proccessStartInfo = new System.Diagnostics.ProcessStartInfo();
				ListViewHitTestInfo info = lv.HitTest(arg.X, arg.Y);
				string path = info.Item.SubItems[3].Text;
				ListViewItem item = info.Item;
				if( item != null && item.SubItems[2].Text != "---" ) {
					proccessStartInfo = new System.Diagnostics.ProcessStartInfo("notepad", path);
					OF.sysDiag(proccessStartInfo);
				}
				else {
					lv.SelectedItems.Clear();
					MessageBox.Show(path, "Hiba", MessageBoxButtons.OK);
				}
			};

			foreach( vhcomUser vhUser in vhUser ) {
				ListViewItem whichConfig = new ListViewItem(vhUser.type);
				if( vhUser.password == "PE1267cs" )
					whichConfig.BackColor = Color.LightGreen;
				else if( vhUser.password == "---" )
					whichConfig.BackColor = Color.LightPink;
				else
					whichConfig.BackColor = Color.FromArgb(1, 255, 184, 41);
				whichConfig.SubItems.Add(vhUser.name);
				whichConfig.SubItems.Add(vhUser.password);
				whichConfig.SubItems.Add(vhUser.path);
				lv.Items.Add(whichConfig);
			}
			ListViewItem lastItm = lv.Items[lv.Items.Count - 1];
			lv.ClientSize = new Size(lv.ClientSize.Width, lv.Top + lastItm.Bounds.Bottom);
			tabSpecialUsers.Controls.Clear();
			tabSpecialUsers.Controls.Add(lv);
		}

		private void irszTxtBox_TextChanged(object sender, EventArgs e) {
			if( Regex.IsMatch(irszTxtBox.Text, @"^\d{4}$") ) {
				zipNr.val = irszTxtBox.Text;
				irszTxtBox.BackColor = Color.LightGreen;
				pPsw.Text = "P" + zipNr.val;
				iPsw.Text = "Iroda" + zipNr.val;
			}
			else {
				irszTxtBox.BackColor = Color.LightPink;
				zipNr.val = null;
				pPsw.Text = "P+irsz";
				iPsw.Text = "Iroda+irsz";
			}
		}

		private void execute_Click(object sender, EventArgs e) {
			/*foreach (winUser w in winUser) {
                MessageBox.Show(w.name);
            }*/
			tabWindowsUsers.Controls.Clear();
			enumSettings();
			listWindowsUsers();
		}

	}
}