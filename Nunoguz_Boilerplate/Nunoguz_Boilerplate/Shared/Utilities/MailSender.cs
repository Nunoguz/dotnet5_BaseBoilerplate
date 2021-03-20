using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

namespace Nunoguz_Boilerplate.Shared.Utilities
{
    public static class MailSender
    {
        private const string templatePath = @"C:\YourHtmlTemplatePATH"; 
        private const string domainUrl = @"http://yourDomainURL";
        public static bool SendMailAfterNewPassword(string email, string newPassword)
        {
            try
            {
                // it works with gmail - should change the client if you want to use another provider
                SmtpClient client = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    UseDefaultCredentials = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential("yourMailAccount", "yourAccountPassword"), 
                    EnableSsl = true
                };

                MailAddress mailAddress = new MailAddress("yourMailAccount"); // example@gmail.com

                MailAddress to = new MailAddress(email); // Mail of the user account to be mailed.
                string mess = string.Format("Your account password has been reset. After logging in with your new password, you can change your password again. : {0} ", newPassword);
                MailMessage message = new MailMessage(mailAddress, to)
                {
                    Subject = "Password Reset",
                    Body = mess,
                    IsBodyHtml = false,
                };

                /*  When to use the template --> topol.io it helps
                   
                        var valueDict = new Dictionary<string, string>
                        {
                                { "#title#",subject},
                                { "#content#", mess},
                                { "#additional#", ""},
                                { "#footer#", "Footer side"},
                        };
                        var htmlText = TextContentReplacer.TextContentReplacerAsync(templatePath, valueDict).Result;
                 *  
                var htmlText = System.IO.File.ReadAllText("index.html");
                htmlText = htmlText.Replace("{FirstName}", mail.FirstName);
                htmlText = htmlText.Replace("{Password}", mail.Password);
                message.Body = htmlText;
                */

                client.Send(message);
                return true;
            }
            catch (Exception exception)
            {
                throw new ApiException(new Error { Message = "An error occured while sending mail.", StackTrace = exception.StackTrace });
            }
        }


    }
}
