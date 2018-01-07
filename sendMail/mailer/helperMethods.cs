using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace mailer
{
    class helperMethods
    {
        #region get message body
        public string getBody()
        {
            try
            {
                using (StreamReader sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"\\MessageBody.txt"))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                ServiceLog.WriteErrorLog(ex.Message);
                ServiceLog.WriteErrorLog("There was an error in the getBody method");
                throw;
            }
             
        }

        #endregion
    }
}
