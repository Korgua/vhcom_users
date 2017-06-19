using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace VHCom_users {
    public class ConfigurationFiles {
		private List<vhcomUser> vhUser = new List<vhcomUser>();
		private logging log = new logging();
        public ConfigurationFiles() {
            GetUsersFromDatabaseXml();
            GetUsersFromDbXml();
            GetUsersFromOfsync();
            GetUsersFromPetroline();
            GetUsersFromAcis();
            GetUsersFromExcelExport();
        }
		public List<vhcomUser> enumVhComUsers() {
			return vhUser;
		}

		void GetUsersFromOfsync() {
			string path = @"C:\\ofsync\\ofsync.exe.config";
            vhcomUser temp = new vhcomUser(path);
            temp.path = path;
            temp.type="ofsync.exe.config";
            bool success = false;
            try {
                using(FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read)) {
                    try {
                        XmlNodeList elemList;
                        int i = 0;
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(fs);
                        try {
                            elemList = xmlDoc.GetElementsByTagName("setting");
                            for(i = 0; i <= elemList.Count - 1; i++) {
                                if(elemList[i].Attributes["name"].Value == "SQL_USER") {
                                    temp.name = elemList[i].InnerText;
                                }

                                if(elemList[i].Attributes["name"].Value == "SQL_PASSWORD") {
                                    temp.password = elemList[i].InnerText;
                                }
                            }
                            success = true;
                            log.writeToLog(null, string.Format("{0} parsed --> name: {1}, password: {2}", temp.type, temp.name, temp.password));
                        }
                        catch(Exception e) {
                            log.writeToLog(null, String.Format("An exception was found while store data from  {0} --> {1}", path, e.Message));
                        }
                    }
                    catch(Exception e) {
                        log.writeToLog(null, String.Format("An exception was found while parsing {0} --> {1}", path, e.Message));
                    }
                }
            }
            catch (Exception e) {
                temp.path = "Az ofsync.exe.config nem található";
                log.writeToLog(null, String.Format("An exception was found while open {0} --> {1}", path, e.Message));
            }
            if (!success) {
                temp.name = "---";
                temp.password = "---";
            }
            vhUser.Add(temp);
            temp = null;
        }
        void GetUsersFromDbXml() {
            string path = @"C:\\ofsync\\db.xml";
            vhcomUser temp = new vhcomUser(path);
            temp.type = "DB.xml";
            bool success = false;
            try {
                using(FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read)) {
                    log.writeToLog(null, string.Format("{0} opened for reading", path));
                    try {
                        XmlNodeList xmlUser, xmlPassword;
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(fs);
                        log.writeToLog(null, string.Format("{0} loaded for XML parsing", path));
                        try {
                            xmlUser = xmlDoc.GetElementsByTagName("sql_user");
                            xmlPassword = xmlDoc.GetElementsByTagName("sql_password");
                            temp.password = xmlPassword[0].InnerText;
                            temp.name = xmlUser[0].InnerText;
                            success = true;
                            log.writeToLog(null, string.Format("{0} parsed --> name: {1}, password: {2}", temp.type, temp.name, temp.password));
                        }
                        catch(Exception e) {
                            log.writeToLog(null, String.Format("An exception was found while store data from  {0} --> {1}", path, e.Message));
                        }
                    }
                    catch(Exception e) {
                        log.writeToLog(null, String.Format("An exception was found while parsing {0} --> {1}", path, e.Message));
                    }
                }
            }
            catch(Exception e) {
                temp.path = "A db.xml nem található";
                log.writeToLog(null, String.Format("An exception was found while open {0} --> {1}", path, e.Message));
            }
            if (!success) {
                temp.name = "---";
                temp.password = "---";
            }
            vhUser.Add(temp);
            temp = null;
        }
        void GetUsersFromDatabaseXml() {
            string path = @"C:\\ofsync\\database.xml";
            vhcomUser temp = new vhcomUser(path);
            temp.type = "Database.xml";
            bool success = false;
            try {
                using(FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read)) {
                    log.writeToLog(null, string.Format("{0} opened for reading", path));
                    try {
                        XmlNodeList xmlUser, xmlPassword;
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(fs);
                        log.writeToLog(null, string.Format("{0} loaded for XML parsing", path));
                        try {
                            xmlUser = xmlDoc.GetElementsByTagName("sql_user");
                            xmlPassword = xmlDoc.GetElementsByTagName("sql_password");
                            temp.password = xmlPassword[0].InnerText;
                            temp.name = xmlUser[0].InnerText;
                            success = true;
                            log.writeToLog(null, string.Format("{0} parsed --> name: {1}, password: {2}", temp.type, temp.name, temp.password));
                        }
                        catch(Exception e) {
                            log.writeToLog(null, String.Format("An exception was found while store data from  {0} --> {1}", path, e.Message));
                        }
                    }
                    catch(Exception e) {
                        log.writeToLog(null, String.Format("An exception was found while parsing {0} --> {1}", path, e.Message));
                    }
                }
            }
            catch(Exception e) {
                temp.path = "A Database.xml nem található";
                log.writeToLog(null, String.Format("An exception was found while open {0} --> {1}", path, e.Message));
            }
            if (!success) {
                temp.name = "---";
                temp.password = "---";
            }
            vhUser.Add(temp);
            temp = null;
        }


        void GetUsersFromPetroline() {
            string path = @"C:\\VH-Petroline\\data\\settings.xml";
            bool success = false;
            vhcomUser temp = new vhcomUser(path);
            temp.type = "Petro\\Settings.xml";
            try {
                using(FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read)) {
                    try {
                        XmlNodeList xmltoExplode;
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(fs);
                        try {
                            xmltoExplode = xmlDoc.GetElementsByTagName("ConnectionString");
                            string[] explodedXml = xmltoExplode[0].InnerText.ToString().Split('|');
                            for(int i = 0; i < explodedXml.Length; i++) {
                                string[] moreExplode = explodedXml[i].Split('=');
                                if(moreExplode[0] == "User ID") {
                                    temp.name = moreExplode[1];
                                }
                                if(moreExplode[0] == "Password") {
                                    if(moreExplode[1] == "@^535532242D296766^@") {
                                        temp.password = "PE1267cs";
                                    }
                                    else {
                                        temp.password = "nem PE1267cs!!!";
                                    }
                                    break;
                                }
                            }
                            success = true;
                            log.writeToLog(null, string.Format("{0} parsed --> name: {1}, password: {2}", temp.type, temp.name, temp.password));
                        }
                        catch(Exception e) {
                            log.writeToLog(null, String.Format("An exception was found while store data from  {0} --> {1}", path, e.Message));
                        }
                    }
                    catch(Exception e) {
                        log.writeToLog(null, String.Format("An exception was found while parsing {0} --> {1}", path, e.Message));
                    }
                }
            }
            catch (Exception e) {
                temp.path = "Az Database.xml nem található";
                log.writeToLog(null, String.Format("An exception was found while open {0} --> {1}", path, e.Message));
            }
            if (!success) {
                temp.name = "---";
                temp.password = "---";
            }
            vhUser.Add(temp);
            temp = null;
        }
        void GetUsersFromAcis() {
            string path = @"C:\\vhcom\\pumpservice\\ACISAutomat.INI";
            bool success = false;
            vhcomUser temp = new vhcomUser(path);
            temp.type = "ACISAutomata.ini";
            try {
                using(StreamReader sr = new StreamReader(path)) {
                    string line;
                    try {
                        while((line = sr.ReadLine()) != null) {
                            if(line.Contains("Password")) {
                                string[] datas = line.Split('|');
                                foreach(string s in datas) {
                                    if(s.Contains("User")) {
                                        temp.name = s.Split('=')[1];
                                    }
                                    if(s.Contains("Password")) {
                                        if(s.Split('=')[1] == "@^535532242D296766^@") {
                                            temp.password = "PE1267cs";
                                        }
                                        else {
                                            temp.password = "nem PE1267cs!!!";
                                        }
                                    }
                                }
                                //sr.Close();
                                success = true;
                                break;
                            }
                        }
                    }
                    catch(Exception e) {
                        log.writeToLog(null, String.Format("An exception was reading {0} --> {1}", path, e.Message));
                    }
                }
            }
            catch(Exception e) {
                temp.path = "Az ACISAutomat.ini nem található";
                log.writeToLog(null, String.Format("An exception was found while open {0} --> {1}", path, e.Message));
            }
            if (!success) {
                temp.name = "---";
                temp.password = "---";
            }
            vhUser.Add(temp);
            temp = null;
        }
        void GetUsersFromExcelExport() {
            string path = @"c:\\Excel export\\config.ini";
            bool success = false;
            vhcomUser temp = new vhcomUser(path);
            temp.type = "Excel Export";
            try {
                using(StreamReader sr = new StreamReader(path)) {
                    string line;
                    try {
                        while((line = sr.ReadLine()) != null) {
                            if(line.Contains("Password")) {
                                string[] datas = line.Split(';');
                                foreach(string s in datas) {
                                    Console.WriteLine(s);
                                    if(s.Contains("User")) {
                                        temp.name = s.Split('=')[1];
                                    }
                                    if(s.Contains("Password")) {
                                        if(s.Split('=')[1] == "PE1267cs") {
                                            temp.password = "PE1267cs";
                                        }
                                        else {
                                            temp.password = s.Split('=')[1];
                                        }
                                    }
                                }
                                //sr.Close();
                                success = true;
                                break;
                            }
                        }
                    }
                    catch(Exception e) {
                        log.writeToLog(null, String.Format("An exception was reading {0} --> {1}", path, e.Message));
                    }
                }
            }
            catch(Exception e) {
                temp.path = "Az ExcelExport\\config.ini nem található";
                log.writeToLog(null, String.Format("An exception was found while open {0} --> {1}", path, e.Message));
            }
            if (!success) {
                temp.name = "---";
                temp.password = "---";
            }
            vhUser.Add(temp);
            temp = null;
        }
    }
}