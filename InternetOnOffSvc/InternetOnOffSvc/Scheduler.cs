using System;
using System.Timers;
using System.ServiceProcess;
using InternetOnOffSvc;

namespace InternetOnOffSvc
{
    public partial class Scheduler : ServiceBase
    {

        private Timer timer1 = null;
        public Scheduler()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            int tickTimer = 60;
            timer1 = new Timer();
            /*
            string filecontent = Library.readFile(AppDomain.CurrentDomain.BaseDirectory + "_tick.txt");
            if (filecontent == "")
            {
                Library.WriteErrorLog("Scheduler On Start: Reading from turnInternetOnOff (arguments) " + tickTimer);
                Library.WriteErrorLog("Scheduler On Start: TickTimer definition file:  " + AppDomain.CurrentDomain.BaseDirectory + "_tick.txt");
            }
            else
            {
                Library.WriteErrorLog("turnInternetOnOff: URL is defined in " + AppDomain.CurrentDomain.BaseDirectory + "_tick.txt: " + Int32.Parse(filecontent));
                tickTimer = Int32.Parse(filecontent);
            }
        */
            this.timer1.Interval = tickTimer * 1000; // every 5 sec
            this.timer1.Elapsed += new System.Timers.ElapsedEventHandler(this.timer1_Tick);
            timer1.Enabled = true;
            Library.WriteErrorLog("InternetOnOff window service started");
        }

        private void timer1_Tick(object sender, ElapsedEventArgs e)
        {

            Library.turnInternetOnOff();
            Library.WriteErrorLog("");
            Library.WriteErrorLog("timer1_Tick");
        }
        protected override void OnStop()
        {
            timer1.Enabled = false;
            Library.WriteErrorLog("InternetOnOff service stopped");

        }

   
    }
}
