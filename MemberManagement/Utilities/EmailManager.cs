using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.Extensions.Configuration;
//namespace MemberManagement.Utilities
//{
    //public class EmailManager
    //{
       // public static void AppSettings(out string UserID, out string Password, out string SMTPPort, out string Host, IConfiguration configuration)
        //{
         //   string emailHost = configuration.GetSection("SMTP").GetSection("Host").Value;
         //   string passWord = configuration.GetSection("SMTP").GetSection("Password").Value;

          //  UserID = configurationManager.AppSettings.Get("UserID");
          //  Password = ConfigurationManager.AppSettings.Get("Password");
           // SMTPPort = ConfigurationManager.AppSettings.Get("SMTPPort");
           // Host = ConfigurationManager.AppSettings.Get("Host");
       // }
        //public static void SendEmail(string From, string Subject, string Body, string To, string UserID, string Password, string SMTPPort, string Host)
       /// {
        //    System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
         //   mail.To.Add(To);
           // mail.From = new MailAddress(From);
         //   mail.Subject = Subject;
         ///   mail.Body = Body;
          //  SmtpClient smtp = new SmtpClient();
        ///    smtp.Host = Host;
         //   smtp.Port = Convert.ToInt16(SMTPPort);
          //  smtp.Credentials = new NetworkCredential(UserID, Password);
         //   smtp.EnableSsl = true;
          //  smtp.Send(mail);
       // }
  //  }
//}
