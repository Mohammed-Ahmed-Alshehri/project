using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Hosting;

namespace TEST2.Services
{
    public class EmailSender : IEmailSender

    {

        public string PopulateMessageBody(string WwwrootPath, string Template, string userName, string url)
        {
            string body = string.Empty;



            var TemplatePath = Path.Combine(WwwrootPath, $@"EmailTemplates\{Template}");

            using (StreamReader reader = System.IO.File.OpenText(TemplatePath))
            {

                body = reader.ReadToEnd();
            }

            body = body.Replace("{UserName}", userName);
            // body = body.Replace("{Title}", title);
            body = body.Replace("{URL}", url);
            // body = body.Replace("{Description}", description);

            return body;
        }

        public string SendEmail(string ToEmail, string Subject, string EmailHtmlMessage)
        {
            MailMessage mailMessage = new MailMessage();

            mailMessage.From = new MailAddress("tadarabhr@gmail.com");

            mailMessage.To.Add(new MailAddress(ToEmail));

            mailMessage.Subject = Subject;

            mailMessage.IsBodyHtml = true;
            var htmlMessage = EmailHtmlMessage;
            mailMessage.Body = htmlMessage;

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential("tadarabhr@gmail.com", "xhgakgrhtdjuoeyn")
            };

            var success = "email hass ben send";

            try
            {
                client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                success = "Something went wrong!! " + ex.Message;
            }

            return success;

        }


    }
}
