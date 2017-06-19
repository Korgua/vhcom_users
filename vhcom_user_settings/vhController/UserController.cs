using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Text;
using System.Windows.Forms;

namespace VHCom_users {
	public class UserController {
		private logging log = new logging();
		//check if existing windows user is in the specified group
		public bool isUserInGroup(string name, string group) {
			log.writeToLog(null, "[isUserInGroup] Begin");
			PrincipalContext context = new PrincipalContext(ContextType.Machine, Environment.MachineName);
			UserPrincipal user = new UserPrincipal(context);
			PrincipalSearcher usrSearch = new PrincipalSearcher(user);
			log.writeToLog(null, "[isUserInGroup Start] Principals are setted");
			log.writeToLog(null, String.Format("[isUserInGroup] Search for {0} in {1}...", name, group));
			foreach(UserPrincipal usrResult in usrSearch.FindAll()) {
				if(usrResult.SamAccountName.ToLower() == name.ToLower()) {
					foreach(var getGroups in usrResult.GetGroups()) {
						if(getGroups.ToString().ToLower() == group.ToLower()) {
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

		public bool modifyUser(winUser winUserOriginal, winUser winUser) {
			log.writeToLog(null, "[modifyUser] Begin");
			if(isUserPresent(winUserOriginal.name) != null) {
				if(removeWinUserFromGroup(winUserOriginal) && RenameWinUser(winUserOriginal, winUser.name) && addWinUserToGroup(winUser)) {
					PrincipalContext context = new PrincipalContext(ContextType.Machine, Environment.MachineName);
					UserPrincipal user = isUserPresent(winUser.name);
					if(winUser.password != null) {
						log.writeToLog(null, "[modifyUser] Principals setted");
						log.writeToLog(null, String.Format("[modifyUser] Trying to set the new password: {0} to {1}", winUser.password, winUser.name));
						try {
							user.SetPassword(winUser.password);
							log.writeToLog(null, String.Format("[modifyUser] New password has set"));
							try {
								user.Save();
								log.writeToLog(null, String.Format("[modifyUser] Properties of {0} is saved", winUser.name));
							}
							catch(Exception e) {
								log.writeToLog(null, String.Format("[modifyUser] Exception was found while saving user settings: {0}", e.Message));
								return false;
							}
						}
						catch(Exception e) {
							log.writeToLog(null, String.Format("[modifyUser] Exception was found while setting the new password: {0}", e.Message));
							return false;
						}
					}
					else {
						log.writeToLog(null, String.Format("[modifyUser] No password was set, nothing changed"));
					}
					if(user.Enabled != winUser.enabled) {
						try {
							log.writeToLog(null, String.Format("[modifyUser] Stored and new user status is different. Changing from {0} to {1}", user.Enabled,winUser.enabled));
							user.Enabled = winUser.enabled;
							user.Save();
							log.writeToLog(null, String.Format("[modifyUser] User status successfully changed from {0} to {1}", user.Enabled, winUser.enabled));
						}
						catch(Exception e) {
							log.writeToLog(null, String.Format("[modifyUser] Exception was found while changing user status {0}", e.Message));
							return false;
						}
					}
					return true;
				}
			}
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
                user.Enabled = winUser.enabled;
                user.AccountExpirationDate = null;
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
                    addWinUserToGroup(winUser);
                }
                return true;
            }
            catch (Exception e) {
                log.writeToLog(null, String.Format("[addWinUser] UserSave exception was found: {0}", e.Message));
                return false;
            }
        }

        public UserPrincipal isUserPresent(string name) {
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
                        return usrResult;
                    }
                }
            }
            catch (Exception e) {
                log.writeToLog(null, String.Format("[isUserPresent] Search for user exception: {0}", e.Message));
            }
            log.writeToLog(null, "[isUserPresent] End");
            return null;
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
        public bool addWinUserToGroup(winUser winUser) {
            log.writeToLog(null, String.Format("[addWinUserToGroup] Begin"));
			PrincipalContext context = new PrincipalContext(ContextType.Machine, Environment.MachineName);
			UserPrincipal user = isUserPresent(winUser.name);
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


		public bool removeWinUserFromGroup(winUser winUser) {
			log.writeToLog(null, String.Format("[removeWinUserFromGroup] Begin"));
			PrincipalContext context = new PrincipalContext(ContextType.Machine, Environment.MachineName);
			UserPrincipal user = isUserPresent(winUser.name);
			GroupPrincipal group = new GroupPrincipal(context);
			foreach(string g in winUser.group) {
				if(isUserInGroup(winUser.name, g)) {
					PrincipalSearcher grpSearch = new PrincipalSearcher(group);
					log.writeToLog(null, String.Format("[removeWinUserFromGroup] Removing {0} from {1} group", winUser.name, g.ToString()));
					foreach(GroupPrincipal grpResult in grpSearch.FindAll()) {
						if(grpResult.ToString().ToLower() == g.ToLower()) {
							grpResult.Members.Remove(user);
							try {
								grpResult.Save();
								log.writeToLog(null, String.Format("[removeWinUserFromGroup] {0} removed {1} group", winUser.name, g.ToString()));
							}
							catch(Exception e) {
								log.writeToLog(null, String.Format("[removeWinUserFromGroup] GroupSave exception: {0}", e.Message));
								log.writeToLog(null, String.Format("[removeWinUserFromGroup] End"));
								return false;
							}
							break;
						}
					}
				}
			}
			log.writeToLog(null, String.Format("[removeWinUserFromGroup] End"));
			return true;
		}

		public bool RenameWinUser(winUser winUser, string newname) {
            log.writeToLog(null, "[renameWinUser] Begin");
            try {
				if(isUserPresent(winUser.name) != null && winUser.name != newname) {
					try {
						System.Diagnostics.ProcessStartInfo proccessStartInfo = new System.Diagnostics.ProcessStartInfo();
						proccessStartInfo = new System.Diagnostics.ProcessStartInfo("wmic", "useraccount where name='" + winUser.name + "' rename " + newname);
						try {
							OuterFunctions OF = new OuterFunctions();
							string result = OF.sysDiag(proccessStartInfo);
							string[] results = result.Split('\n');
							int returnValue = 9;
							foreach(string s in results) {
								if(s.Contains("ReturnValue = ")) {
									string[] temp = s.Split('=');
									temp[1] = temp[1].Trim();
									temp[1] = temp[1].Replace(";", "");
									returnValue = Convert.ToInt32(temp[1]);
									temp = null;
									break;
								}

							}
							if(returnValue != 0) {
								throw new Exception(string.Format("System Error Code: {0}, desired username ({1}) is already present", returnValue, newname));
							}
							try {
								log.writeToLog(null, String.Format("[renameWinUser] Renaming {0}'s Full Name to {1} ...", winUser.name, newname));
								PrincipalContext context = new PrincipalContext(ContextType.Machine, Environment.MachineName);
								UserPrincipal user = new UserPrincipal(context);
								PrincipalSearcher usrSearch = new PrincipalSearcher(user);
								foreach(UserPrincipal usrResult in usrSearch.FindAll()) {
									if(usrResult.SamAccountName.ToLower() == newname.ToLower()) {
										usrResult.SamAccountName = newname;
										usrResult.Name = newname;
										usrResult.DisplayName = newname;
										try {
											log.writeToLog(null, String.Format("[renameWinUser] Properties setted, trying to save..."));
											usrResult.Save();
											log.writeToLog(null, String.Format("[renameWinUser] Renaming {0}'s Full Name to {1} was successful", winUser.name, newname));
										}
										catch(Exception e) {
											log.writeToLog(null, String.Format("[renameWinUser] Excepetion found while renaming {0}'s Full Name to {1}: {2}", winUser.name, newname, e.Message.ToString()));
											return false;
										}
										break;
									}
								}
							}
							catch(Exception e) {
								log.writeToLog(null, String.Format("[renameWinUser] Excepetion found while renaming {0}'s Full Name to {1}: {2}", winUser.name, newname, e.Message.ToString()));
							}
							log.writeToLog(null, String.Format("[renameWinUser] Rename was succes: {0} --> {1}", winUser.name, newname));
							winUser.name = newname;
							return true;
						}
						catch(Exception e) {
							log.writeToLog(null, String.Format("[renameWinUser] Excepetion found while renaming: {0}", e.Message.ToString()));
						}
					}
					catch(Exception e) {
						log.writeToLog(null, String.Format("[renameWinUser] Excepetion found while try to rename: {0}", e.Message.ToString()));
					}
				}
				else {
					log.writeToLog(null, String.Format("[renameWinUser] Old username and new username are the same, nothing changed"));
					return true;
				}
            }
            catch (Exception e) {
                log.writeToLog(null, String.Format("[renameWinUser] Excepetion found while trying to search for {0}: {1}", winUser.name, e.Message.ToString()));
            }
            return false;
        }


		public void getGroupsByUser(winUser winUser, UserPrincipal UserPrincipal, PrincipalSearcher PrincipalSearcher) {
			log.writeToLog(null, "[getGroupsByUser] Begin");
			PrincipalContext context = new PrincipalContext(ContextType.Machine, Environment.MachineName);
			UserPrincipal user = new UserPrincipal(context);
			PrincipalSearcher usrSearch = new PrincipalSearcher(user);
			log.writeToLog(null, "[getGroupsByUser] Principals are setted");
			log.writeToLog(null, String.Format("[getGroupsByUser] Searching for groups for {0}...", winUser.name));

			foreach(var getGroups in UserPrincipal.GetGroups()) {
				if(getGroups.ToString().ToLower() == "asztal távoli felhasználói") {
					winUser.rdp = true;
				}
				if(getGroups.ToString().ToLower() == "rendszergazdák") {
					winUser.admin = true;
				}
				if(getGroups.ToString().ToLower() == "felhasználók") {
					winUser.users = true;
				}
				log.writeToLog(null, String.Format("[getGroupsByUser] Group found: {0}", getGroups.ToString()));
				winUser.setGroup(getGroups.ToString());
			}

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
        public List<winUser> enumUsers() {
            List<winUser> winUser = new List<winUser>();
            PrincipalContext context = new PrincipalContext(ContextType.Machine);
            UserPrincipal UserPrincipal = new UserPrincipal(context);
            PrincipalSearcher PrincipalSearcher = new PrincipalSearcher(UserPrincipal);
            winUser temp = null;
            UserController UF = new UserController();
            foreach(UserPrincipal result in PrincipalSearcher.FindAll()) {
                //I was able to delete my own, active profile.... FFFFFFFUUUUUUCCCCCCCKKKKK
                if(Environment.UserName != result.SamAccountName) {
                    temp = new winUser();
                    temp.name = result.SamAccountName;
                    UF.getGroupsByUser(temp, result, PrincipalSearcher);
                    UF.getUserStatus(temp);
                    winUser.Add(temp);
                    temp = null;
                }
            }
			return winUser;
            /*log.writeToLog(null, "Userek:");
            foreach(winUser winUser in winUser) {
                log.writeToLog(null, "------------------------------");
                log.writeToLog(null, "Név: " + winUser.name);
                log.writeToLog(null, "engedélyezve: " + winUser.enabled);
                log.writeToLog(null, "Csoportjai: ");
                log.writeToLog(winUser.group, null);
            }*/
        }
    }
}
