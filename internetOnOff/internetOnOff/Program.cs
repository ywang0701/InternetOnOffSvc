using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using System.Net.NetworkInformation;


namespace InternetOnOff
{
    class InternetOnOff
    {
        public static void Main()
        {
            var url = "http://www.taascloud.com/treeview/data" + "/user/ywangperl@gmail.com.txt";
            turnInternetOnOff(url);

        }

        public static void turnInternetOnOff(string url)
        {
            try
            {
                if (pingHost("www.yahoo.com"))
                {
                    ;
                }
                else
                {
                    ipconfig("/renew");
                }
                var textFromFile = (new WebClient()).DownloadString(url);
                string[] lines = textFromFile.Split('\n');
                foreach (string line in lines)
                {
                    RegexOptions options = RegexOptions.IgnoreCase;
                    Regex r1 = new Regex(@"^\s*Internet\s*blocking\s*:\s*\s*startTime\s*=\s*(.+)finishTime\s*=\s*(.+)\s*$", options);
                    Match match = r1.Match(line);
                    if (match.Success)
                    {
                        var startTime = match.Groups[1].Value;
                        var finishTime = match.Groups[2].Value;
                        Time4BlockingInternet(startTime, finishTime);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                ; // Console.WriteLine("turnInternetOnOff is executed");
            }
        }

        private static bool pingHost(string nameOrAddress)
        {

            bool pingable = false;
            Ping pingSender = new Ping();
            try
            {
                PingReply reply = pingSender.Send(nameOrAddress);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                // 
            }
            return pingable;
        }
        private static bool Time4BlockingInternet(string startTimeStr, string finishTimeStr)
        {

            DateTime startTime = DateTime.Parse(startTimeStr);
            DateTime finishTime = DateTime.Parse(finishTimeStr);
            DateTime currentTime = DateTime.Now;

            if ((DateTime.Compare(startTime, currentTime) <= 0) && (DateTime.Compare(currentTime, finishTime) <= 0))
            {
                // Console.WriteLine("Turn Off Internet");
                WriteErrorLog("Turn Off Internet");
                ipconfig("/release");

            }
            else
            {
                WriteErrorLog("Turn On Internet");
            }

            return true;
        }
        private static void ipconfig(string args)
        {
            Process myProcess = new Process();
            try
            {
                myProcess.StartInfo.UseShellExecute = false;
                myProcess.StartInfo.CreateNoWindow = true;
                myProcess.StartInfo.RedirectStandardOutput = true;
                myProcess.StartInfo.FileName = "c:\\Windows\\System32\\ipconfig.exe";
                myProcess.StartInfo.Arguments = args;
                myProcess.Start();
                string output = myProcess.StandardOutput.ReadToEnd();
                WriteErrorLog(output);
            }
            catch (Exception e)
            {
                WriteErrorLog(e.Message);
            }
        }

        public static void WriteErrorLog(Exception ex)
        {
            System.IO.StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.txt", true);
                sw.WriteLine(DateTime.Now.ToString() + ": " + ex.Source.ToString().Trim() + "; " + ex.Message.ToString().Trim());
                sw.Flush();
                sw.Close();
            }
            catch
            {
            }
        }
        public static void WriteErrorLog(string Message)
        {
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.txt", true);
                sw.WriteLine(DateTime.Now.ToString() + ": " + Message);
                sw.Flush();
                sw.Close();
            }
            catch
            {
            }
        }


    }
}

