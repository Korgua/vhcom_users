using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;

namespace VHCom_users {
    public class OuterFunctions {
        //To make CMD calls, it's easier to call it as a function
        //that returns as a string with the output
        public string sysDiag(System.Diagnostics.ProcessStartInfo proccessStartInfo) {
            System.Diagnostics.Process proc = new System.Diagnostics.Process { StartInfo = proccessStartInfo };
            proc.StartInfo.StandardOutputEncoding = Encoding.GetEncoding(850);
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.CreateNoWindow = true;
            proc.Start();
            string result = proc.StandardOutput.ReadToEnd();
            return result;
        }

        //To call WMI query, that returns with collection
        public ManagementObjectCollection wqlQuery(string qry) {
            ConnectionOptions options = new ConnectionOptions();
            options.Impersonation = ImpersonationLevel.Impersonate;
            ManagementScope scope = new ManagementScope("\\\\localhost\\root\\cimv2", options);
            scope.Connect();
            ObjectQuery query = new ObjectQuery(qry);
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
            ManagementObjectCollection queryCollection = searcher.Get();
            return queryCollection;
        }

    }
}
