using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Drawing;
using System.IO;
using System.Management;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

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
            createHeadersInListView();
            
            enumSettings();
            log.writeToLog(null, "createHeadersInListView Start");
            log.writeToLog(null, "showErrors Start");
            showColorDescription();
            log.writeToLog(null, "servicesListView Start");
            servicesListView();
            log.writeToLog(null, "enumUsers Start");
            enumUsers();
            log.writeToLog(null, "End Start");
            listWindowsUsers();
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
        }

        public ListView createListView(Dictionary<string, int> header) {
            ListView lv = new ListView();
            lv.FullRowSelect = true;
            lv.View = View.Details;
            lv.AllowColumnReorder = false;
            lv.AllowDrop = false;
            lv.Top = 5; lv.Left = 10;
            lv.BackColor = Color.WhiteSmoke;
            lv.GridLines = true;
            // Prevent the column resizeeeee
            lv.ColumnWidthChanging += (e, sender) => {
                ColumnWidthChangingEventArgs arg = (ColumnWidthChangingEventArgs)sender;
                arg.Cancel = true;
                arg.NewWidth = lv.Columns[arg.ColumnIndex].Width;
            };
            bool first = true;
            foreach(KeyValuePair<string,int> kvp in header) {
                lv.Columns.Add(kvp.Key.ToString(), kvp.Value, first==true ? HorizontalAlignment.Left : HorizontalAlignment.Center);

            }
            int width = 5;
            foreach(ColumnHeader ch in lv.Columns) {
                width += ch.Width;
            }
            lv.Width = width;
            return lv;
        }

        public void listWindowsUsers() {
            log.writeToLog(null, "[ListWindowsUsers] Begin");
            Dictionary<string, int> headers = new Dictionary<string, int>();
            headers.Add("Felhasználónév", 150);
            headers.Add("Rendszergazda?", 100);
            headers.Add("Távoli?", 100);
            headers.Add("Aktív?", 100);
            ListView lv = createListView(headers);
            log.writeToLog(null, "[ListWindowsUsers] Lisrview created");
            foreach (winUser winUser in winUser) {
                log.writeToLog(null,winUser.name);
                ListViewItem lvi = new ListViewItem(winUser.name);
                lvi.SubItems.Add(winUser.admin == true ? "igen" : "nem");
                lvi.SubItems.Add(winUser.rdp == true ? "igen" : "nem");
                lvi.SubItems.Add(winUser.enabled == true ? "igen" : "nem");
                if (winUser.enabled) {
                    lvi.BackColor = Color.LightGreen;
                }
                else {
                    lvi.BackColor = Color.LightPink;
                }
                lv.Items.Add(lvi);
                lvi = null;
            }
            lv.Height = lv.Items.Count * 20 + 10;
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

            Dictionary<string, string> ServiceStartType = new Dictionary<string, string>();
            ServiceStartType.Add("Auto", "Automatikus");
            ServiceStartType.Add("Manual", "Kézi");
            ServiceStartType.Add("Disabled", "Letiltva");
            ServiceStartType.Add("Stopped", "Leállítva");
            ServiceStartType.Add("Running", "Fut");

            // Prevent the column resize

            Dictionary<string, int> windowsServices = new Dictionary<string, int>();
            windowsServices.Add("Szolgáltatás", 120);
            windowsServices.Add("Futattó", 120);
            windowsServices.Add("Indítás", 120);
            windowsServices.Add("Állapot", 120);

            ListView services = createListView(windowsServices);
            services.ColumnWidthChanging += (e, sender) => {
                ColumnWidthChangingEventArgs arg = (ColumnWidthChangingEventArgs)sender;
                arg.Cancel = true;
                arg.NewWidth = services.Columns[arg.ColumnIndex].Width;
            };
            
            ListViewItem serviceName = null;
            ManagementObjectCollection queryCollection = OF.wqlQuery("select Name, StartName, StartMode, State from Win32_Service");// where name='"+kv.Key+"'");
            foreach (ManagementObject service in queryCollection) {
                string dictKeyToDelete = null;
                foreach (KeyValuePair<string, string> kv in serviceList) {
                    if ((string)service["Name"] == kv.Key) {
                        Console.WriteLine("name: {0}, startname: {1}, mode: {2}, state: {3}", service["Name"], service["StartName"], service["StartMode"], service["State"]);
                        dictKeyToDelete = service["Name"].ToString();
                        serviceName = new ListViewItem(kv.Value);
                        serviceName.SubItems.Add(service["StartName"].ToString());
                        serviceName.SubItems.Add(ServiceStartType[service["StartMode"].ToString()]);
                        serviceName.SubItems.Add(ServiceStartType[service["State"].ToString()]);
                       services.Items.Add(serviceName);
                        break;
                    }
                }
                //It is forbidden to delete a key from Dictionary in a loop
                if (dictKeyToDelete != null) {
                    serviceList.Remove(dictKeyToDelete);
                    dictKeyToDelete = null;
                }
            }
            foreach (KeyValuePair<string, string> kv in serviceList) {
                serviceName = new ListViewItem(kv.Value);
                serviceName.SubItems.Add("Nincs adat");
                serviceName.SubItems.Add("Nincs adat");
                serviceName.SubItems.Add("Nincs adat");
                services.Items.Add(serviceName);
            }
            services.Height = services.Items.Count * 20 + 10;
            tabServices.Controls.Add(services);
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
            PrincipalContext context = new PrincipalContext(ContextType.Machine);
            UserPrincipal user = new UserPrincipal(context);
            PrincipalSearcher search = new PrincipalSearcher(user);
            winUser temp = null;
            winUserFunctions WUF = new winUserFunctions();
            foreach (UserPrincipal result in search.FindAll()) {
                temp = new winUser();
                temp.name = result.SamAccountName;
                WUF.getGroupsByUser(temp);
                WUF.getUserStatus(temp);
                winUser.Add(temp);
                temp = null;
            }
            log.writeToLog(null, "Userek:");
            foreach(winUser winUser in winUser) {
                log.writeToLog(null, "------------------------------");
                log.writeToLog(null, "Név: " + winUser.name);
                log.writeToLog(null, "engedélyezve: "+winUser.enabled);
                log.writeToLog(null, "Csoportjai: ");
                log.writeToLog(winUser.group, null);
            }
        }

        public void createHeadersInListView() {
            Dictionary<string, int> headers = new Dictionary<string, int>();
            headers.Add("Micsoda",100);
            headers.Add("SQL-hez név", 75);
            headers.Add("SQL-hez jelszó", 100);
            headers.Add("Útvonal", 250);
            ListView lv = createListView(headers);
            lv.MouseDoubleClick += (e, sender) => {
                MouseEventArgs arg = (MouseEventArgs)sender;
                System.Diagnostics.ProcessStartInfo proccessStartInfo = new System.Diagnostics.ProcessStartInfo();
                ListViewHitTestInfo info = lv.HitTest(arg.X,arg.Y);
                string path = info.Item.SubItems[3].Text;
                ListViewItem item = info.Item;
                if(item != null && item.SubItems[2].Text != "---") {
                    proccessStartInfo = new System.Diagnostics.ProcessStartInfo("notepad", path);
                    OF.sysDiag(proccessStartInfo);
                }
                else {
                    lv.SelectedItems.Clear();
                    MessageBox.Show(path, "Hiba", MessageBoxButtons.OK);
                }
            };

            foreach (vhcomUser vhUser in vhUser) {
                ListViewItem whichConfig = new ListViewItem(vhUser.type);
                if (vhUser.password == "PE1267cs")
                    whichConfig.BackColor = System.Drawing.Color.LightGreen;
                else if (vhUser.password == "---")
                    whichConfig.BackColor = System.Drawing.Color.LightPink;
                else
                    whichConfig.BackColor = System.Drawing.Color.FromArgb(1, 255, 184, 41);
                whichConfig.SubItems.Add(vhUser.name);
                whichConfig.SubItems.Add(vhUser.password);
                whichConfig.SubItems.Add(vhUser.path);
                lv.Items.Add(whichConfig);
            }
            lv.Height = lv.Items.Count * 20 + 10;
            tabSpecialUsers.Controls.Add(lv);
        }

        private void irszTxtBox_TextChanged(object sender, EventArgs e) {
            if (Regex.IsMatch(irszTxtBox.Text, @"^\d{4}$")) {
                zipNr.val = irszTxtBox.Text;
                irszTxtBox.BackColor = System.Drawing.Color.LightGreen;
                pPsw.Text = "P" + zipNr.val;
                iPsw.Text = "Iroda" + zipNr.val;
            }
            else {
                irszTxtBox.BackColor = System.Drawing.Color.LightPink;
                zipNr.val = null;
                pPsw.Text = "P+irsz";
                iPsw.Text = "Iroda+irsz";
            }
        }

        private void execute_Click(object sender, EventArgs e) {
            /*foreach (winUser w in winUser) {
                MessageBox.Show(w.name);
            }*/
            this.Hide();
            vhcomUserSettings alma = new vhcomUserSettings();
            //this.Close();
            //Application.Restart();
            //alma.Show();
            alma.ShowDialog();
            MessageBox.Show("alma");
        }

    }
}
