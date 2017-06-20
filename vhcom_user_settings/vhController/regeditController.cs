using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VHCom_users {
    class regeditController {
        logging log = new logging();
        public RegistryKey openKey(string Key) {
            log.writeToLog(null, String.Format("[RegeditOpenKey: {0}] Begin", Key));
            try {
                RegistryKey key = Registry.LocalMachine.OpenSubKey(Key);
                if(key == null) {
                    log.writeToLog(null, String.Format("[RegeditOpenKey] {0} is present", Key));
                    return key;
                }
                else {
                    log.writeToLog(null, String.Format("[RegeditOpenKey] {0} is not present", Key));

                }
            }
            catch(Exception e) {
                log.writeToLog(null, String.Format("[RegeditOpenKey] Exception found while opening {0}: {1}", Key, e.Message));
            }
            return null;
        }
        public string getValue(RegistryKey key,string name) {
            log.writeToLog(null, String.Format("[RegeditGetValue: {0}] Begin", name));
            try {
                string value = key.GetValue(name).ToString();
                return value;
            }
            catch(Exception e) {
                log.writeToLog(null, String.Format("[RegeditGetValue] Exception found while getting value from {0}: {1}", name, e.Message));
            }
            return null;
        }

        public string searchForOFL() {
            RegistryKey key = openKey("Software\vhcom");
            return key.ToString();
        }
    }
}
