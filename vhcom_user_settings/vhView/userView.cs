using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Windows.Forms;

namespace VHCom_users {
    public class userView : vhcomUserSettings {
        winUserFunctions WUF = new winUserFunctions();
        //vhcomUserSettings VUS = new vhcomUserSettings();
        List<winUser> winUser = new List<winUser>();
        List<vhcomUser> vhUser = new List<vhcomUser>();
        OuterFunctions OF = new OuterFunctions();

        //logging log = new logging();

        public void selectedWindowsUsersFromListView(winUser winUser, int Top) {

        t
            foreach(Control item in VUS.tabWindowsUsers.Controls) {
                if(item.GetType().ToString() == "System.Windows.Forms.Button") {
                    VUS.tabWindowsUsers.Controls.Remove(item);
                }
            }
            Button btn_delete = new Button();
            Button btn_modify = new Button();
            int btn_width = 140;

            btn_delete.Text = "Felhasználó törlése";
            btn_delete.Width = btn_width;
            btn_delete.Left = VUS.tabWindowsUsers.Width - btn_width - 5;
            btn_delete.Top = Top + 5;
            btn_delete.Name = "Delete";

            btn_delete.MouseClick += (e, sender) => {
                MouseEventArgs arg = (MouseEventArgs)sender;
                if(WUF.deleteWindowsUserConfirmation(winUser)) {
                    listWindowsUsers();
                }
            };

            btn_modify.Text = "Felhasználó módosítása";
            btn_modify.Width = btn_width;
            btn_modify.Left = VUS.tabWindowsUsers.Width - ((btn_width + 5) * 2);
            btn_modify.Top = Top + 5;
            btn_modify.Name = "Modify";

            btn_modify.MouseClick += (e, sender) => {
                MouseEventArgs arg = (MouseEventArgs)sender;
                editWindowsUserForm(winUser);
            };
            VUS.tabWindowsUsers.Controls.Add(btn_delete);
            VUS.tabWindowsUsers.Controls.Add(btn_modify);
        }
        public void editWindowsUserForm(winUser winUser) {
            WindowsFormUser WindowsFormUser = new WindowsFormUser();
            WindowsFormUser.Text = "Módosítás";
            WindowsFormUser.txt_username.Text = winUser.name;
            WindowsFormUser.txt_password.Text = winUser.password;
            WindowsFormUser.chk_enabled.Checked = winUser.enabled;
            WindowsFormUser.chk_grpadmin.Checked = winUser.admin;
            WindowsFormUser.chk_grpusers.Checked = false;
            foreach(string s in winUser.group) {
                if(s == "Felhasználók")
                    WindowsFormUser.chk_grpusers.Checked = true;
            }

            bool allSet = false;
            string newUserName, password;
            bool rdp, admin, users, enabled;
            WindowsFormUser.btn_winuser_confirm.MouseClick += (e, sender) => {
                if(WindowsFormUser.txt_newusername.Text.Length == 0) {
                    WindowsFormUser.txt_newusername.BackColor = Color.LightPink;
                    newUserName = null;
                    allSet = false;
                }
                else {
                    WindowsFormUser.txt_newusername.BackColor = Color.LightGreen;
                    newUserName = WindowsFormUser.txt_newusername.Text;
                    allSet = true;
                }
                if(WindowsFormUser.txt_password.Text.Length == 0) {
                    WindowsFormUser.txt_password.BackColor = Color.LightPink;
                    password = null;
                    allSet = false;
                }
                else {
                    WindowsFormUser.txt_password.BackColor = Color.LightGreen;
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
            foreach(KeyValuePair<string, int> kvp in header) {
                lv.Columns.Add(kvp.Key.ToString(), kvp.Value, first == true ? HorizontalAlignment.Left : HorizontalAlignment.Center);
                if(first)
                    first = false;

            }
            int width = 5;
            foreach(ColumnHeader ch in lv.Columns) {
                width += ch.Width;
            }
            lv.Width = width;
            lv.Sorting = SortOrder.Ascending;

            return lv;
        }

        public void listWindowsUsers() {
            //log.writeToLog(null, "[ListWindowsUsers] Begin");
            Dictionary<string, int> headers = new Dictionary<string, int>();
            headers.Add("Felhasználónév", 160);
            headers.Add("Rendszergazda?", 120);
            headers.Add("Távoli?", 120);
            headers.Add("Aktív?", 120);
            ListView lv = createListView(headers);

            //log.writeToLog(null, "[ListWindowsUsers] Listview created");
            foreach(winUser winUser in winUser) {
                ListViewItem lvi = new ListViewItem(winUser.name);
                lvi.SubItems.Add(winUser.admin ? "igen" : "nem");
                lvi.SubItems.Add(winUser.rdp ? "igen" : "nem");
                lvi.SubItems.Add(winUser.enabled ? "igen" : "nem");
                if(winUser.enabled) {
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
                if(item != null) {
                    foreach(winUser winUser in winUser) {
                        if(winUser.name == item.Text) {
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

            VUS.tabWindowsUsers.Controls.Clear();
            VUS.tabWindowsUsers.Controls.Add(lv);
            //log.writeToLog(null, "[ListWindowsUsers] End");
        }

        public void ConfigurationFilesListView() {
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
                if(item != null && item.SubItems[2].Text != "---") {
                    proccessStartInfo = new System.Diagnostics.ProcessStartInfo("notepad", path);
                    OF.sysDiag(proccessStartInfo);
                }
                else {
                    lv.SelectedItems.Clear();
                    MessageBox.Show(path, "Hiba", MessageBoxButtons.OK);
                }
            };

            foreach(vhcomUser vhUser in vhUser) {
                ListViewItem whichConfig = new ListViewItem(vhUser.type);
                if(vhUser.password == "PE1267cs")
                    whichConfig.BackColor = Color.LightGreen;
                else if(vhUser.password == "---")
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
            VUS.tabSpecialUsers.Controls.Clear();
            VUS.tabSpecialUsers.Controls.Add(lv);
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
            foreach(ManagementObject service in queryCollection) {
                string dictKeyToDelete = null;
                foreach(KeyValuePair<string, string> kv in serviceList) {
                    if(service["Name"].ToString().ToLower() == kv.Key.ToLower()) {
                        dictKeyToDelete = kv.Key;
                        serviceName = new ListViewItem(kv.Value);
                        serviceName.SubItems.Add(service["StartName"].ToString());
                        serviceName.SubItems.Add(ServiceStartType[service["StartMode"].ToString()]);
                        serviceName.SubItems.Add(ServiceStartType[service["State"].ToString()]);
                        if(service["StartMode"].ToString().ToLower() == "disabled" || service["State"].ToString().ToLower() == "stopped") {
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
                if(dictKeyToDelete != null) {
                    serviceList.Remove(dictKeyToDelete);
                    dictKeyToDelete = null;
                }
            }
            foreach(KeyValuePair<string, string> kv in serviceList) {
                //log.writeToLog(null, string.Format("Unknown services: {0}", kv.Key));
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
            VUS.tabServices.Controls.Clear();
            VUS.tabServices.Controls.Add(lv);
        }
    }
}