using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net.Mail; //Mail message
using System.Net;

namespace mailer
{
    static class ServiceLog
    {
        #region Error logging methods
        public static void WriteErrorLog(Exception ex)
        {
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\\LogFile.txt", true);
                sw.WriteLine(DateTime.Now.ToString() + ": " + ex.Source.ToString().Trim() + "; " + ex.Message.ToString().Trim());
                sw.Flush();
                sw.Close();
            }
            catch (Exception exception)
            {
                EventLog x = new EventLog("Application");
                x.WriteEntry("Your service log class has created an exception: " + exception.Message.ToString().Trim());
                throw;
            }
        }

        public static void WriteErrorLog(string Message)
        {
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\\LogFile.txt", true);
                sw.WriteLine(DateTime.Now.ToString() + ": " + Message);
                sw.Flush();
                sw.Close();
            }
            catch (Exception ex)
            {
                EventLog x = new EventLog("Application");
                x.WriteEntry("Your service log class has created an exception: " + ex.Message.ToString().Trim());
                throw;
            }
        }
        #endregion


        #region Sending an email

        public static void SendEmail(string toEmail, string cc, string bcc, string subject, string message, string attchPaths)
        {
            string HostAdd = ConfigurationManager.AppSettings["Host"];
            string FromEmailid = ConfigurationManager.AppSettings["FromMail"];
            string Pass = ConfigurationManager.AppSettings["Password"];

            //Creating the object of MailMessage //using system.net.mail
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(FromEmailid);
            mailMessage.Subject = subject;
            mailMessage.Body = message;
            mailMessage.IsBodyHtml = true; //set bool based on the body of the email being html or not

            //all the people you are sending to
            string[] ToMuliID = toEmail.Split(',');
            foreach (string email in ToMuliID)
            {
                mailMessage.To.Add(new MailAddress(email));
            }

            //all the cc (add error checking to check if string empty- cc and bcc)
            if (cc != string.Empty || cc != null)
            {
                string[] CCMail = cc.Split(',');
                foreach (string email in CCMail)
                {
                    mailMessage.CC.Add(new MailAddress(email));
                }
            }
            
            //all the bcc
            if (bcc != string.Empty || bcc != null)
            {
                string[] BCCMail = bcc.Split(',');
                foreach (string email in BCCMail)
                {
                    mailMessage.Bcc.Add(new MailAddress(email));
                }
            }

            if (attchPaths != string.Empty || attchPaths != null)
            {
                string[] att = attchPaths.Split(',');
                foreach (string attachPath in att)
                {
                    mailMessage.Attachments.Add(new Attachment(attachPath));
                }
            }

            //network and security related creds

            //using system.net

            var smtp = new SmtpClient
            {
                Host = HostAdd,   
                Port = int.Parse(ConfigurationManager.AppSettings["smtpPort"]),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(FromEmailid, Pass)
            };

            smtp.Send(mailMessage);
        }

        #endregion

    }
}
