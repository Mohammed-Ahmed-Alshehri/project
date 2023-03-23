using Azure.Core;

namespace TEST2.Services
{
    public interface IEmailSender
    {
        public string SendEmail(string ToEmail, string Subject, string EmailHtmlMessage);

        public string PopulateMessageBody(string WwwrootPath, string Template, string userName, string url);
    }
}
