using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Timers;

namespace mailer
{
    public partial class Service1 : ServiceBase
    {
        Timer timer1;
        int getCallType;
        public Service1()
        {
            InitializeComponent();
            int strTime = int.Parse(ConfigurationManager.AppSettings["callDuration"]);
            getCallType = int.Parse(ConfigurationManager.AppSettings["CallType"]);
            if (getCallType == 1)
            {
                ServiceLog.WriteErrorLog("the get call value is equal to 1");
                timer1 = new Timer();
                double inter = GetNextInterval();
                timer1.Interval = inter;
                timer1.Elapsed += new ElapsedEventHandler(ServiceTimer_Tick);
            }
            else
            {
                timer1 = new Timer();
                timer1.Interval = strTime * 1000;
                timer1.Elapsed += new ElapsedEventHandler(ServiceTimer_Tick);
            }
        }

        private void ServiceTimer_Tick(object sender, ElapsedEventArgs e)
        {
            helperMethods helper = new helperMethods();
            string Msg = helper.getBody(); //has to have the base text file called MessageBody
            string toEmail = ConfigurationManager.AppSettings["toMail"];
            string ccList = ConfigurationManager.AppSettings["ccList"];
            string bccList = ConfigurationManager.AppSettings["bccList"];
            string subject = ConfigurationManager.AppSettings["subject"];
            string attchList = ConfigurationManager.AppSettings["attcList"];

            //sending the email
            ServiceLog.SendEmail(toEmail,ccList,bccList,subject,Msg, attchList);

            if (getCallType == 1)
            {
                timer1.Stop();
                System.Threading.Thread.Sleep(1000000);
                SetTimer();
            }
        }

        private double GetNextInterval()
        {
            var timeString = ConfigurationManager.AppSettings["StartTime"];
            DateTime t = DateTime.Parse(timeString);
            TimeSpan ts = new TimeSpan();
            ts = t - DateTime.Now;
            if (ts.TotalMilliseconds < 0)
            {
                ts = t.AddDays(1) - DateTime.Now; //inc the interval based on your req
            }
            return ts.TotalMilliseconds;
        }

        private void SetTimer()
        {
            try
            {
                double inter = (double)GetNextInterval();
                timer1.Interval = inter;
                timer1.Start();
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(ex.Message);
                EventLog.WriteEntry(ex.StackTrace);
                throw;
            }
        }

        protected override void OnStart(string[] args)
        {
            timer1.AutoReset = true;
            timer1.Enabled = true;
            ServiceLog.WriteErrorLog("Reporting service started");
        }

        protected override void OnStop()
        {
            timer1.AutoReset = false;
            timer1.Enabled = false;
            ServiceLog.WriteErrorLog("Reporting service has stopped");
        }
    }
}
