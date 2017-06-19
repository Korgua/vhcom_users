using System;
using System.Collections.Generic;
using System.IO;

namespace VHCom_users {

    public class logging {

        List<string> loggingExc = new List<string>();
        public string logPath = @".\\vhuserlog.txt";
        public logging() {
            /*try {
                File.Delete(logPath);
            }
            catch (Exception e) {
                loggingExc.Add(e.Message);
            }*/
            createLogFile();
        }

        public bool createLogFile() {
            FileStream fs = null;
            try {
                if (!File.Exists(logPath)) {
                    try {
                        fs = File.Create(logPath);
                        fs.Close();
                        try {
                            StreamWriter log = new StreamWriter(logPath, true);
                            DateTimeOffset logDateStart = new DateTimeOffset(DateTime.Now);
                            log.WriteLine(logDateStart.ToString("yyyy.MM.dd HH:mm:ss:fff") + ("\t------------   Log Start   ------------"));
                            log.Close();
                            return true;
                        }
                        catch (Exception e) {
                            loggingExc.Add(e.Message);
                        }
                    }
                    catch (Exception e) {
                        loggingExc.Add(e.Message);
                    }
                }
                else {
                    return true;
                }
            }
            catch (Exception e) {
                loggingExc.Add(e.Message);
            }
            return false;
        }
        public void writeToLog(List<string> multiline, string line) {
            if (createLogFile()) {
                try {
                    StreamWriter log = new StreamWriter(logPath, true);
                    DateTimeOffset logStart = new DateTimeOffset(DateTime.Now);
                    if (multiline != null) {
                        bool isFirst = true;
                        foreach (string s in multiline) {
                            if (isFirst) {
                                log.WriteLine(logStart.ToString("yyyy.MM.dd HH:mm:ss:fff") + ("\t" + s));
                                isFirst = !isFirst;
                            }
                            else
                                log.WriteLine(logStart.ToString("                       ") + ("\t" + s));
                        }
                    }
                    else if (line != null) {
                        log.WriteLine(logStart.ToString("yyyy.MM.dd HH:mm:ss:fff") + ("\t" + line));
                    }
                    else
                        log.WriteLine();
                    log.Close();
                }
                catch (Exception e) {
                    loggingExc.Add(e.Message);
                }
            }
        }
    }
}
