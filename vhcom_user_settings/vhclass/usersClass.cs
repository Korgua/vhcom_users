using System.Collections.Generic;
namespace VHCom_users {
    public class winUser {
        public string name {get; set;}
        public List<string> group {get;}
        public string password{get; set;}
        public bool enabled {get; set;}
        public bool admin { get; set; }
        public bool rdp { get; set; }


        public winUser() {
            this.name = null;
            this.group = new List<string>();
            this.password = null;
            this.enabled = false;
        }
        public void setGroup(string group) { this.group.Add(group); }
    }

    public class vhcomUser {
        public string type {get; set;}
        public string name {get; set;}
        public string password {get; set;}
        public string path {get; set;}

        public vhcomUser(string path) {
            this.path = path;
        }
    }

    public class zipNr {
        public string val {get; set;}
    }
}
