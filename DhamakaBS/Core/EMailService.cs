using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace DhamakaBS.Core
{
    public class EmailService
    {
        public static async Task SendEmailAsync(string toEmail, string subject, string messageBody, bool showGreetings = true)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("sojib352", "sojib352@gmail.com"));
                message.Subject = subject;

                var toMailList = toEmail.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var to in toMailList)
                {
                    message.To.Add(new MailboxAddress(to, to));
                }

                BodyBuilder bodyBuilder;
                if (showGreetings)
                {
                    bodyBuilder = new BodyBuilder
                    {
                        TextBody = $@"Greetings,",
                        HtmlBody = messageBody
                    };
                }
                else
                {
                    bodyBuilder = new BodyBuilder { HtmlBody = messageBody };
                }

                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.Auto);

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate("sojib352@gmail.com", "&6509gj!dk");

                await client.SendAsync(message);
                client.Disconnect(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //_logger.LogError(ex.Message);
            }
        }
    }
}
