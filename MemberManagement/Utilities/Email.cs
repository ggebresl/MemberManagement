using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MemberManagement.Models;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
namespace MemberManagement.Utilities
{
    public class Email
    {
        [Required(ErrorMessage = "Please enter Email Address.")]
        public string To { get; set; }

        [Required(ErrorMessage = "Please enter Subject of the  Email.")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Please enter Body of the Email")]
        public string Body { get; set; }

        public string RoleType { get; set; }

        public string GetRoleType(string roleType)
        {
            string returnVal = string.Empty;

            switch (roleType)
            {
                case "Member":
                case "All":
                case "Admin":
                    returnVal = roleType;
                    break;

                case "Audio_Admin":
                case "Vedio_Admin":
                    returnVal = roleType;
                    break;

                case "Image_Admin":
                    returnVal = roleType;
                    break;

                default:
                    returnVal = roleType;
                    break;
            }

            return roleType;
        }

        public List<string> GetEmailList(List<Payment> payer, string roleType)
        {

            //To Do. Retrieve email lists bassed on role type
            List<string> emailList = new List<string>();

            for (int count = 0; count < payer.Count; count++)
            {
                emailList.Add(payer[count].Email);
            }
            return emailList;
        }

        public async Task SendEmail(Email email, List<string> emailList, IConfiguration configuration)
        {
            string emailHost = configuration.GetSection("SMTP").GetSection("Host").Value;
            string passWord = configuration.GetSection("SMTP").GetSection("Password").Value;

            string roleType = email.GetRoleType(email.RoleType);

            foreach (var em in emailList)
            {
                email.To = em;

                if (!string.IsNullOrWhiteSpace(em))
                {

                 
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("Tigray Community, Boston", emailHost));

                    message.To.Add(new MailboxAddress(email.RoleType, em));
                 //   message.To.Add(new MailboxAddress("", email.To));
                    message.Subject = email.Subject;

                    message.Body = new TextPart("plain")
                    {
                        Text = email.Body
                    };

                    using (var client = new SmtpClient())
                    {
                        if (!client.IsConnected)
                        {
                            await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                            client.AuthenticationMechanisms.Remove("XOAUTH2");
                        }
                        if (!client.IsAuthenticated)
                        {
                            //sender host to authenticate
                            await client.AuthenticateAsync(emailHost, passWord);
                        }
                        await client.SendAsync(message);
                        await client.DisconnectAsync(true);
                        
                    }
                }
            }
            //add finally {
            //client.Disconnect(true);
            //client.Dispose();
             
        }

    }
}
