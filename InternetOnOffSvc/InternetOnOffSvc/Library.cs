using System;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;
using System.Collections.Generic;

namespace InternetOnOffSvc
{
    public static class Library
    {
        public static void turnInternetOnOff()
        {

            var hostUrl = @"http://online.okwebsolution.com"; var username = @"a";
            hostUrl = @"http://www.tafcloud.com"; username = "ywang";

            var url = hostUrl + @"/account/" + username + @"/schedule.txt";                 // http://online.okwebsolution.com/account/a/schedule.txt

            try
            {
                string filecontent = readFile(AppDomain.CurrentDomain.BaseDirectory + "_url.txt");
                if ((filecontent == "") || (filecontent.Contains(@"#")))  // use default url
                {
#if DEBUG
                    WriteErrorLog("turnInternetOnOff: use default " + url);
                    // WriteErrorLog("turnInternetOnOff: treeview editor: " + urlEdit);
                    WriteErrorLog("turnInternetOnOff: URL definition file:  " + AppDomain.CurrentDomain.BaseDirectory + "_url.txt");
#endif
                }
                else // Read url from local file "_url.txt" 
                {
                    url = filecontent;
#if DEBUG
                    WriteErrorLog("turnInternetOnOff: Reading from turnInternetOnOff (arguments) " + url);
                    WriteErrorLog("turnInternetOnOff: URL is defined in " + AppDomain.CurrentDomain.BaseDirectory + "_url.txt: " + url);
                    WriteErrorLog("turnInternetOnOff: URL is " + url);
#endif
                }

                if (pingHost("www.yahoo.com"))
                {
                    ;
#if DEBUG
                    WriteErrorLog("turnInternetOnOff: " + "ping www.yahoo.com for intenet connectivity");
#endif
                }
                else
                {
                    ipconfig("/renew");
#if DEBUG
                    WriteErrorLog("turnInternetOnOff: config /renew to reconnect the internet");
#endif
                }

                var textFromFile = (new WebClient()).DownloadString(url);
#if DEBUG
                WriteErrorLog("turnInternetOnOff: get url    = " + url);
                WriteErrorLog("turnInternetOnOff: url content= " + textFromFile);
#endif

                List<String> urls = new List<String>();
                bool blockInternet = false;

                string[] lines = textFromFile.Split('\n');

                foreach (string line in lines)
                {
                    RegexOptions options = RegexOptions.IgnoreCase;
                    // Regex r1 = new Regex(@"^\s*(\S+)\s*(\S+)\s*:\s*\s*startTime\s*=\s*(.+)finishTime\s*=\s*(.+)\s*$", options);
                    Regex r1 = new Regex(@"^\s*\d+\|(.+)\|(.+)\|(.+)\|(.+)\r", options);
                    Regex r2 = new Regex(@"^\s*(\S+)\s+(\S+)", options);
                    Regex rWWW = new Regex(@"www\.", options);
                    Regex rCOM = new Regex(@".com", options);

#if DEBUG
                    WriteErrorLog("turnInternetOnOff: line= " + line);
#endif

                    Match match = r1.Match(line);
                    if (match.Success)          // Match the 75|block www.yahoo.com|block www.yahoo.com |2016-03-10 12:29|2016-03-10 14:29
                    //                       1               1                  2            3        4         
                    {
                        var url_block = match.Groups[1].Value;      // block internet
                        var description = match.Groups[2].Value;    // yahoo
                        var permission = description;               // 
                        var startTime = match.Groups[3].Value;
                        var finishTime = match.Groups[4].Value;

#if DEBUG
                        WriteErrorLog("turnInternetOnOff: format match = " + line);
#endif     
                        Match match2 = r2.Match(url_block); // Match the (block) (yahoo) from the title
                        if (match2.Success)
                        {
                            url_block = match2.Groups[2].Value;
                            permission = match2.Groups[1].Value;
                        }
                        else
                        {
                            continue; // The title is not in the block internet format
                        }

                        if (Time4Blocking(startTime, finishTime))   // Build the block site list 
                        {
#if DEBUG
                            WriteErrorLog("turnInternetOnOff: time match = " + line);
#endif     
                            if (url_block.Equals("internet", StringComparison.OrdinalIgnoreCase)) { blockInternet = true; }
                            else
                            {
                                Match matchWWW = rWWW.Match(url_block);
                                Match matchCOM = rCOM.Match(url_block);
                                // if (matchWWW.Success) { ;} else { url_block = "www." + url_block; }
                                if (matchCOM.Success) { ;} else { url_block = url_block + ".com"; }
                                urls.Add("127.0.0.1 " + url_block);
                            }
#if DEBUG
                            WriteErrorLog("startBlocking time: " + startTime + " FinishBlockTime: " + finishTime + " " + permission + "   " + url_block);
#endif     

                            foreach (string str in urls)
                            {
                                {
                                    ;
#if DEBUG
                                    WriteErrorLog("startBlocking time: " + startTime + " FinishBlockTime: " + finishTime + " blick: " + str);
#endif     
                                }

                            }
                        }
                    }
                }
                if (blockInternet)
                {
#if DEBUG
                    WriteErrorLog("turnInternetOnOff: Turn Off Internet");
                    WriteErrorLog("turnInternetOnOff: ipconfig /release");
#endif     
                    ipconfig("/release");
                }
                else
                {
                    updateHostsFile(urls);
                    ipconfig("/flushdns");
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
        private static bool Time4Blocking(string startTimeStr, string finishTimeStr)
        {

            DateTime startTime = DateTime.Parse(startTimeStr);
            DateTime finishTime = DateTime.Parse(finishTimeStr);
            DateTime currentTime = DateTime.Now;

            if ((DateTime.Compare(startTime, currentTime) <= 0) && (DateTime.Compare(currentTime, finishTime) <= 0))
            {
                return true;
            }
            else
            {
                return false;
            }

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
                WriteErrorLog("ipconfig: ......");
                // WriteErrorLog("ipconfig: " + output);
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
        public static string readFile(string fname)
        {
            string filecontent = "";
            try
            {

                if (File.Exists(fname))
                {
                    filecontent = File.ReadAllText(fname);
                }
                else
                {
                    StreamWriter sw = null;
                    sw = new StreamWriter(fname, true);
                    sw.WriteLine(@"#http://www.tafcloud.com/account/ywang/schedule.txt");
                    sw.Flush();
                    sw.Close();
                    filecontent = "";
                }
            }
            catch
            {

            }
            return filecontent;
        }
        static public void updateHostsFile(List<String> list)
        {

            string fname = @"C:\Windows\System32\drivers\etc\hosts";
            // using (StreamWriter writetext = new StreamWriter(@"C:\Windows\System32\drivers\etc\hosts"))
            // using (StreamWriter writetext = new StreamWriter(@"c:\write.txt"))
            using (StreamWriter writetext = new StreamWriter(fname))
            {
                string str1 = @"
# Copyright (c) 1993-2009 Microsoft Corp.
#
# This is a sample HOSTS file used by Microsoft TCP/IP for Windows.
#
# This file contains the mappings of IP addresses to host names. Each
# entry should be kept on an individual line. The IP address should
# be placed in the first column followed by the corresponding host name.
# The IP address and the host name should be separated by at least one
# space.
#
# Additionally, comments (such as these) may be inserted on individual
# lines or following the machine name denoted by a '#' symbol.
#
# For example:
#
#      102.54.94.97     rhino.acme.com          # source server
#       38.25.63.10     x.acme.com              # x client host

# localhost name resolution is handled within DNS itself.
#	127.0.0.1       localhost
#	::1             localhost

0.0.0.1	mssplus.mcafee.com
";
                writetext.WriteLine(str1);
            }

            foreach (string str in list)
            {
                using (StreamWriter sw = File.AppendText(fname))
                {
                    sw.WriteLine(str);
                    Console.WriteLine(str);
                }

            }


        }
    }
}

