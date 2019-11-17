using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Jerry.GeneralTools
{
    public class MailerSendGrid
    {
        /// <summary>
        /// Sends with SendGrid API and email by default to AdminMail and TestDevMailsN (where N its a integer number),
        /// those emails preconfigured in web.config applications settings.
        /// </summary>
        /// <param name="subject">Email Title</param>
        /// <param name="bodyMessage">HTML body message</param>
        /// <param name="aditionalRecipients">Optional list to add additional recipients to the email</param>
        /// <param name="attachments">Optional list to add attachments to the email</param>
        /// <returns>A string with an error message, if its empty or null, everything worked OK.</returns>
        public static async Task<String> sendEmailToMultipleRecipients(string subject, string bodyMessage, List<EmailAddress> aditionalRecipients, List<Attachment> attachments)
        {
            string errorMessage = string.Empty;

            //Get admin email and name by default from webconfig application settings
            string AdminMail = ConfigurationManager.AppSettings["AdminMail"];
            string AdminNameMail = ConfigurationManager.AppSettings["AdminNameMail"];

            //Is mailer enabled
            bool emailEnabled = true;
            Boolean.TryParse(ConfigurationManager.AppSettings["enableEmail"], out emailEnabled);

            //Init receipients list
            var to = new List<EmailAddress>();

            //Send by default to admin
            if (!String.IsNullOrEmpty(AdminMail))
                to.Add(new EmailAddress(AdminMail, AdminNameMail));

            //If there is developer's emails for testing, add them to the list too
            int c = 1;
            string TestDevMail = string.Empty;
            while (true)
            {
                TestDevMail = ConfigurationManager.AppSettings["TestDevMail" + c];
                if (!String.IsNullOrEmpty(TestDevMail))
                    to.Add(new EmailAddress(TestDevMail));
                else
                    break;
                c++;
            }

            //Add more receipients
            if (aditionalRecipients != null && aditionalRecipients.Count() > 0)
                foreach (var rec in aditionalRecipients)
                    to.Add(rec);

            //If at least one receipient was added to the list
            if (to.Count() > 0 && emailEnabled)
            {
                var from = new EmailAddress(AdminMail, AdminNameMail);

                var apiKey = ConfigurationManager.AppSettings["SendGrindAPIKey"];
                var client = new SendGridClient(apiKey);

                //var plainTextContent = "and easy to do anywhere, even with C#";
                var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, to, subject, string.Empty, bodyMessage);

                //Attach files to email if set
                if(attachments!=null && attachments.Count() > 0)
                    msg.AddAttachments(attachments);

                //Send email using sendgrid service
                var response = await client.SendEmailAsync(msg);

                //If an error ocurred
                if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
                    errorMessage = await response.Body.ReadAsStringAsync();
            }
            else
                errorMessage = to.Count()==0 ? GlobalMessages.ERROR_MSG_NO_RECEIPIENTS: 
                    !emailEnabled?GlobalMessages.ERROR_MSG_EMAILS_DISABLED:string.Empty;

            return errorMessage;
        }

        /// <summary>
        /// Validate a string for a valid email address.
        /// </summary>
        /// <param name="emailaddress">Email address</param>
        /// <returns>A boolean, true for valid email address</returns>
        public static bool IsValid(string emailaddress)
        {
            try
            {
                System.Net.Mail.MailAddress m = new System.Net.Mail.MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}