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
            var user = "/user/ywangperl@gmail.com";
            user = "/user/ywangperlgmail.com.txt";

            var url = "http://www.taascloud.com/treeview/data" + user;
            var urlEdit = "http://www.taascloud.com:8088/editor/?fname=" + user;

            try
            {
                string filecontent = readFile(AppDomain.CurrentDomain.BaseDirectory + "_url.txt");
                if (filecontent == "")
                {
                    WriteErrorLog("turnInternetOnOff: Reading from turnInternetOnOff (arguments) " + url);
                    WriteErrorLog("turnInternetOnOff: treeview editor: " + urlEdit);
                    WriteErrorLog("turnInternetOnOff: URL definition file:  "+ AppDomain.CurrentDomain.BaseDirectory + "_url.txt" );
                }
                else
                {
                    WriteErrorLog("turnInternetOnOff: URL is defined in "+ AppDomain.CurrentDomain.BaseDirectory + "_url.txt: " + url);
                    WriteErrorLog("turnInternetOnOff: treeview editor: " + urlEdit);
                    url = filecontent;
                }

                if (pingHost("www.yahoo.com"))
                {
                    ; //  WriteErrorLog("turnInternetOnOff: " + "ping www.yahoo.com");
                }
                else
                {
                    ipconfig("/renew");
                    WriteErrorLog("turnInternetOnOff: config /renew");
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
                        WriteErrorLog("startBlocking time: " + startTime + " FinishBlockTime: " + finishTime); 
                        Time4BlockingInternet(startTime, finishTime);
                    }

                    List<String> list = new List<String>();

                    list.Add("127.0.0.1 www.abc.com");
                    list.Add("127.0.0.1 www.abd.com");
                    list.Add("127.0.0.1 www.abe.com");
                    list.Add("127.0.0.1 www.abf.com");
                    list.Add("127.0.0.1 www.abg.com");


                    updateHostsFile(list);




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

            if ((DateTime.Compare(startTime, currentTime) <= 0) && (DateTime.Compare(currentTime, finishTime) <= 0))
            {
                WriteErrorLog("timeIsRight: Turn Off Internet");
                WriteErrorLog("timeIsRight: ipconfig /release");
                ipconfig("/release");
            }
            else
            {
                // Console.WriteLine("Turn On Internet");
                // ipconfig("/renew");
            }

            return true;
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
                WriteErrorLog("Time4BlockingInternet: Turn Off Internet");
                WriteErrorLog("Time4BlockingInternet: ipconfig /release");
                ipconfig("/release");

            }
            else
            {
                WriteErrorLog("Time4BlockingInternet: Turn On Internet");
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
                WriteErrorLog("ipconfig: ......" );
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

# test

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

