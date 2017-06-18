using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Text;
using System.Windows.Forms;

namespace vhcom_user_settings {
    public class winUserFunctions {
        logging log = new logging();
        OuterFunctions OF = new OuterFunctions();

        //check if existing windows user is in the specified group
        public bool isUserInGroup(string name, string group) {
            log.writeToLog(null, "[isUserInGroup] Begin");
            PrincipalContext context = new PrincipalContext(ContextType.Machine, Environment.MachineName);
            UserPrincipal user = new UserPrincipal(context);
            PrincipalSearcher usrSearch = new PrincipalSearcher(user);
            log.writeToLog(null, "[isUserInGroup Start] Principals are setted");
            log.writeToLog(null, String.Format("[isUserInGroup] Search for {0} in {1}...", name, group));
            foreach (UserPrincipal usrResult in usrSearch.FindAll()) {
                if (usrResult.SamAccountName.ToLower() == name.ToLower()) {
                    foreach (var getGroups in usrResult.GetGroups()) {
                        if (getGroups.ToString().ToLower() == group.ToLower()) {
                            log.writeToLog(null, String.Format("[isUserInGroup] Foreach in usrResult ended with match: {0} is in {1}", name, group));
                            log.writeToLog(null, "[isUserInGroup] End");
                            return true;
                        }
                    }
                    break;
                }
            }
            log.writeToLog(null, String.Format("[isUserInGroup] Foreach in usrResult ended without match: {0} is not in {1}", name, group));
            log.writeToLog(null, "[isUserInGroup] End");
            return false;
        }

        //Add windows user (if not present yet)
        //Set the user properties, like password expirition, full name, or account expirition
        public bool addWinUser(winUser winUser) {
            try {
                log.writeToLog(null, string.Format("[addWinUser] Begin"));
                PrincipalContext context = new PrincipalContext(ContextType.Machine, Environment.MachineName);
                UserPrincipal user = new UserPrincipal(context), userTemp = null;
                log.writeToLog(null, "[addWinUser] UserPrincipals setted");
                if (winUser.password != null) {
                    user.SetPassword(winUser.password);
                }
                user.SamAccountName = winUser.name;
                user.DisplayName = winUser.name;
                user.Name = winUser.name;
                user.UserCannotChangePassword = true;
                user.PasswordNeverExpires = true;
                user.Enabled = true;
                user.AccountExpirationDate = null;
                userTemp = user;
                log.writeToLog(null, "[addWinUser] Properties setted");
                try {
                    user.Save();
                    List<string> s = new List<string>();
                    s.Add("Felhasználónév: " + user.SamAccountName);
                    if (winUser.password != null) {
                        s.Add("Jelszó: " + winUser.password);
                    }
                    else {
                        s.Add("Jelszó: nem volt megadva");
                    }
                    s.Add("Jelszó lejár: " + user.PasswordNeverExpires);
                    s.Add("Felhasználói jelszóváltoztatás: " + user.UserCannotChangePassword);
                    s.Add("Fiók aktív: " + user.Enabled);
                    s.Add("Fiók lejár: " + user.AccountExpirationDate);
                    log.writeToLog(s, null);
                    log.writeToLog(null, String.Format("[addWinUser] {0} saved", winUser.name));
                    log.writeToLog(null, "[addWinUser] End");
                }
                catch (Exception e) {
                    log.writeToLog(null, String.Format("[addWinUser] UserSave exception was found while saving: {0} --> {1}", winUser.name, e.Message));
                    return false;
                }

                if (winUser.group.Count > 0) {
                    addWinUserToGroup(user, context, winUser);
                }
                return true;
            }
            catch (Exception e) {
                log.writeToLog(null, String.Format("[addWinUser] UserSave exception was found: {0}", e.Message));
                return false;
            }
        }

        public bool isUserPresent(string name) {
            try {
                log.writeToLog(null, string.Format("[isUserPresent] Begin"));
                PrincipalContext context = new PrincipalContext(ContextType.Machine, Environment.MachineName);
                UserPrincipal user = new UserPrincipal(context);
                PrincipalSearcher usrSearch = new PrincipalSearcher(user);
                log.writeToLog(null, "[isUserPresent] Principals are setted");
                log.writeToLog(null, String.Format("[isUserPresent] Searching for user: {0}", name));
                foreach (UserPrincipal usrResult in usrSearch.FindAll()) {
                    if (usrResult.SamAccountName.ToLower() == name.ToLower()) {
                        log.writeToLog(null, "[isUserPresent] End");
                        return true;
                    }
                }
            }
            catch (Exception e) {
                log.writeToLog(null, String.Format("[isUserPresent] Search for user exception: {0}", e.Message));
            }
            log.writeToLog(null, "[isUserPresent] End");
            return false;
        }

        public bool removeWinUser(winUser winUser) {
            log.writeToLog(null, "[removeWinUser] Begin");
            DirectoryEntry localDirectory = new DirectoryEntry("WinNT://" + Environment.MachineName.ToString());
            DirectoryEntries localUsers = localDirectory.Children;
            try {
                DirectoryEntry toDelete = localUsers.Find(winUser.name);
                try {
                    log.writeToLog(null, String.Format("[removeWinUser] {0} is found, try to remove", winUser.name));
                    localUsers.Remove(toDelete);
                    log.writeToLog(null, String.Format("[removeWinUser] {0} is removed", winUser.name));
                    log.writeToLog(null, "[removeWinUser] End");
                    return true;
                }
                catch (Exception e) {
                    log.writeToLog(null, String.Format("[removeWinUser] Remove user exception: {0}", e.Message));
                }
            }
            catch (Exception e) {
                log.writeToLog(null, String.Format("[removeWinUser] Remove user exception while searching: {0}", e.Message));
            }
            log.writeToLog(null, String.Format("[removeWinUser] End"));
            return false;
        }

        public bool deleteWindowsUserConfirmation(winUser winUser) {
            DialogResult choice = MessageBox.Show(winUser.name.ToUpper() + " törlésre kerül!\nValóban törölni akarod?", "Megerősítés", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (choice == DialogResult.Yes) {
                string name = winUser.name;
                if (removeWinUser(winUser)) {
                    MessageBox.Show(name + " sikeresen törölve", "Információ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return true;
                }
                else {
                    MessageBox.Show("A törlés közben hiba lépett fel. Rendszergazdaként futtatod?", "Információ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            return false;
        }

        // add windows user to group(s)
        // winuser.group stores the group(s)
        // Group/UserPrincipal.FindByIdentity() takes 7-10 seconds
        // This method does the same, but in 0.2 second
        public bool addWinUserToGroup(UserPrincipal user, PrincipalContext context, winUser winUser) {
            log.writeToLog(null, String.Format("[addWinUserToGroup] Begin"));
            GroupPrincipal group = new GroupPrincipal(context);
            foreach (string g in winUser.group) {
                if (!isUserInGroup(winUser.name, g)) {
                    PrincipalSearcher grpSearch = new PrincipalSearcher(group);
                    log.writeToLog(null, String.Format("[addWinUserToGroup] Adding {0} to {1} group", winUser.name, g.ToString()));
                    foreach (GroupPrincipal grpResult in grpSearch.FindAll()) {
                        if (grpResult.ToString().ToLower() == g.ToLower()) {
                            grpResult.Members.Add(user);
                            try {
                                grpResult.Save();
                                log.writeToLog(null, String.Format("[addWinUserToGroup] {0} added to {1} group", winUser.name, g.ToString()));
                            }
                            catch (Exception e) {
                                log.writeToLog(null, String.Format("[addWinUserToGroup] GroupSave exception: {0}", e.Message));
                                log.writeToLog(null, String.Format("[addWinUserToGroup] End"));
                                return false;
                            }
                            break;
                        }
                    }
                }
            }
            log.writeToLog(null, String.Format("[addWinUserToGroup] End"));
            return true;
        }

        public bool RenameWinUser(winUser winUser, string newname) {
            log.writeToLog(null, "[renameWinUser] Begin");
            try {
                if (isUserPresent(winUser.name)) {
                    try {
                        System.Diagnostics.ProcessStartInfo proccessStartInfo = new System.Diagnostics.ProcessStartInfo();
                        proccessStartInfo = new System.Diagnostics.ProcessStartInfo("wmic", "useraccount where name='" + winUser.name + "' rename " + newname);
                        try {
                            string result = OF.sysDiag(proccessStartInfo);
                            string[] results = result.Split('\n');
                            int returnValue = 9;
                            foreach (string s in results) {
                                if (s.Contains("ReturnValue = ")) {
                                    string[] temp = s.Split('=');
                                    temp[1] = temp[1].Trim();
                                    temp[1] = temp[1].Replace(";", "");
                                    returnValue = Convert.ToInt32(temp[1]);
                                    temp = null;
                                    break;
                                }

                            }
                            if (returnValue != 0) {
                                throw new Exception(string.Format("System Error Code: {0}, desired username ({1}) is already present", returnValue, newname));
                            }
                            try {
                                log.writeToLog(null, String.Format("[renameWinUser] Renaming {0}'s Full Name to {1} ...", winUser.name, newname));
                                PrincipalContext context = new PrincipalContext(ContextType.Machine, Environment.MachineName);
                                UserPrincipal user = new UserPrincipal(context);
                                PrincipalSearcher usrSearch = new PrincipalSearcher(user);
                                foreach (UserPrincipal usrResult in usrSearch.FindAll()) {
                                    if (usrResult.SamAccountName.ToLower() == newname.ToLower()) {
                                        usrResult.SamAccountName = newname;
                                        usrResult.Name = newname;
                                        usrResult.DisplayName = newname;
                                        try {
                                            log.writeToLog(null, String.Format("[renameWinUser] Properties setted, trying to save..."));
                                            usrResult.Save();
                                            log.writeToLog(null, String.Format("[renameWinUser] Renaming {0}'s Full Name to {1} was successful", winUser.name, newname));
                                        }
                                        catch (Exception e) {
                                            log.writeToLog(null, String.Format("[renameWinUser] Excepetion found while renaming {0}'s Full Name to {1}: {2}", winUser.name, newname, e.Message.ToString()));
                                            return false;
                                        }
                                        break;
                                    }
                                }
                            }
                            catch (Exception e) {
                                log.writeToLog(null, String.Format("[renameWinUser] Excepetion found while renaming {0}'s Full Name to {1}: {2}", winUser.name, newname, e.Message.ToString()));
                            }
                            log.writeToLog(null, String.Format("[renameWinUser] Rename was succes: {0} --> {1}", winUser.name, newname));
                            winUser.name = newname;
                            return true;
                        }
                        catch (Exception e) {
                            log.writeToLog(null, String.Format("[renameWinUser] Excepetion found while renaming: {0}", e.Message.ToString()));
                        }
                    }
                    catch (Exception e) {
                        log.writeToLog(null, String.Format("[renameWinUser] Excepetion found while try to rename: {0}", e.Message.ToString()));
                    }
                }
            }
            catch (Exception e) {
                log.writeToLog(null, String.Format("[renameWinUser] Excepetion found while trying to search for {0}: {1}", winUser.name, e.Message.ToString()));
            }
            return false;
        }


        public void getGroupsByUser(winUser winUser, UserPrincipal UserPrincipal, PrincipalSearcher PrincipalSearcher ) {
            log.writeToLog(null, "[getGroupsByUser] Begin");
            PrincipalContext context = new PrincipalContext(ContextType.Machine, Environment.MachineName);
            UserPrincipal user = new UserPrincipal(context);
            PrincipalSearcher usrSearch = new PrincipalSearcher(user);
            log.writeToLog(null, "[getGroupsByUser] Principals are setted");
            log.writeToLog(null, String.Format("[getGroupsByUser] Searching for groups for {0}...", winUser.name));
            //foreach (UserPrincipal usrResult in usrSearch.FindAll()) {
            //    if (usrResult.SamAccountName.ToLower() == winUser.name.ToLower()) {
                    foreach (var getGroups in UserPrincipal.GetGroups()) {
                        if(getGroups.ToString().ToLower()== "asztal távoli felhasználói") {
                            winUser.rdp = true;
                        }
                        if (getGroups.ToString().ToLower() == "rendszergazdák") {
                            winUser.admin = true;
                        }
                        log.writeToLog(null, String.Format("[getGroupsByUser] Group found: {0}", getGroups.ToString()));
                        winUser.setGroup(getGroups.ToString());
                    }
            //        break;
            //   }
            //}
            log.writeToLog(null, "[getGroupsByUser] End");
        }
        public void getUserStatus(winUser winUser) {
            log.writeToLog(null, "[getUserStatus] Begin");
            PrincipalContext context = new PrincipalContext(ContextType.Machine, Environment.MachineName);
            UserPrincipal user = new UserPrincipal(context);
            PrincipalSearcher usrSearch = new PrincipalSearcher(user);
            log.writeToLog(null, "[getUserStatus] Principals are setted");
            log.writeToLog(null, String.Format("[getUserStatus] Searching for user status for {0}...", winUser.name));
            foreach (UserPrincipal usrResult in usrSearch.FindAll()) {
                if (usrResult.SamAccountName.ToLower() == winUser.name.ToLower()) {
                    if (usrResult.Enabled != null) {
                        winUser.enabled = (bool)usrResult.Enabled;
                    }
                    else {
                        winUser.enabled = false;
                    }
                }
            }
            log.writeToLog(null, String.Format("[getUserStatus] {0} is enabled:  {1}", winUser.name,winUser.enabled.ToString().ToUpper()));
        }
    }
}
