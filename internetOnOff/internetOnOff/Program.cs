using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;

namespace InternetOnOff
{
        class InternetOnOff
        {
            public static void Main()
            {
                turnInternetOnOff();
            }

            public static void turnInternetOnOff()
            {
                var site = "http://www.taascloud.com/treeview/data";
                var user = "/user/tc.txt";

                var url = site + user;

                try
                {
                    ipconfig("/renew");
                    var textFromFile = (new WebClient()).DownloadString(url);
                    string[] lines = textFromFile.Split('\n');
                    foreach (string line in lines)
                    {
                        RegexOptions options = RegexOptions.IgnoreCase;
                        Regex r1 = new Regex(@"^\s*Internet\s*connection\s*:\s*\s*startTime\s*=\s*(.+)finishTime\s*=\s*(.+)\s*$", options);
                        Match match = r1.Match(line);
                        if (match.Success)
                        {
                            var startTime = match.Groups[1].Value;
                            var finishTime = match.Groups[2].Value;
                            if (timeIsRight(startTime, finishTime))
                            {
                                // ipconfig("/all");
                                ipconfig("");
                            }
                        }
                        // Console.WriteLine(">" + line);
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

            private static bool timeIsRight(string startTimeStr, string finishTimeStr)
            {

                DateTime startTime = DateTime.Parse(startTimeStr);
                DateTime finishTime = DateTime.Parse(finishTimeStr);
                DateTime currentTime = DateTime.Now;

                // Console.WriteLine(" st {0}  ct {1}  ft{2}", startTime.ToString(), currentTime.ToString(), finishTime.ToString());
                if ((DateTime.Compare(startTime, currentTime) <= 0) && (DateTime.Compare(currentTime, finishTime) <= 0))
                {
                    // Console.WriteLine("Turn On Internet");
                    // ipconfig("/renew");
                }
                else
                {
                    // Console.WriteLine("Turn Off Internet");
                    ipconfig("/release");
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
                    // Console.WriteLine(output);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
}

