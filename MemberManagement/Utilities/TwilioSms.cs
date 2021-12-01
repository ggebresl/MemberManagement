//Install-package Twilio  - using Tools, NuGet Package Manager from  "Package Manager Console"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using MemberManagement.Models;
using System.ComponentModel.DataAnnotations;
//Gerawork Gebreslassie -we can we get the body of the text to be sent either from GUI or configuration

namespace MemberManagement.Utilities
{
    public class TwilioSms
    {

        public string RoleType { get; set; }

        [Required(ErrorMessage = "Please Phone number to send Text to.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Please enter text message to send to.")]
        public string Text { get; set; }
        

       // private readonly ILogger<TwilioSms> _logger;

        public List<string> GetPhoneList(List<Payment> payer)
        {

            List<string> phoneList = new List<string>();

            for (int count = 0; count < payer.Count; count++)
            {
                string ph = payer[count].Phone;
                if (!string.IsNullOrWhiteSpace(ph))
                {
                    phoneList.Add(ph);
                }
            }
            return phoneList;
        }

        public async Task sendText(string txtToBeSent, List<string> phoneList, IConfiguration configuration)
        {
            /* In case if you have to get the following confguration entries from the host Environment Variables, follow the code below.
             private string accntId = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
              private string authID = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");
             private string memberPhone = Environment.GetEnvironmentVariable("OUT_BOUND_DAILER");*/

            //Get entries from appsettings.json file
            string accntId = configuration.GetSection("TWILIO_ACCOUNT_SID").Value;
            string authID = configuration.GetSection("TWILIO_AUTH_TOKEN").Value;
            string outBoundDailer = configuration.GetSection("OUT_BOUND_DAILER").Value;

            this.Text = txtToBeSent;

            try
            {
                TwilioClient.Init(accntId, authID);
                for (int count = 0; count < phoneList.Count; count++)
                {
                     await MessageResource.CreateAsync(
                     to: new PhoneNumber(phoneList[count]),
                     from: new PhoneNumber(outBoundDailer),
                     body: this.Text);
                }
            }
            catch(Exception ex)
            {
                string message = ex.Message;
               // just for unit test 
            //    _logger.LogError(ex.ToString());
                return;
            }
        }
}
}
