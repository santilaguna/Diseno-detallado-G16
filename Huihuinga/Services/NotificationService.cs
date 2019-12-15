using Huihuinga.Models;
using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace Huihuinga.Services
{
    public class NotificationService: INotificationService
    {
        private readonly EmailConfiguration _emailConfiguration;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationService(EmailConfiguration emailConfiguration, UserManager<ApplicationUser> userManager)
        {
            _emailConfiguration = emailConfiguration;
            _userManager = userManager;
        }

        public async Task<bool> SendConferenceNotification(ApplicationUserConcreteConference[] userConferences, string mailBodyMessage)
        {
            foreach (var user in userConferences)
            {
                MimeMessage message = new MimeMessage();

                MailboxAddress from = new MailboxAddress("Huihuinga", "admin@huihuinga.com");
                message.From.Add(from);


                var userName = await _userManager.GetUserNameAsync(user.User);
                var email = await _userManager.GetEmailAsync(user.User);
                MailboxAddress to = new MailboxAddress(userName, email);
                message.To.Add(to);
                //message.To.AddRange(users.Select(u => new MailboxAddress(_userManager.GetUserNameAsync(u.User), _userManager.GetEmailAsync(u.User))));

                message.Subject = $"Huihuinga - {user.Conference.name}";

                BodyBuilder bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = "" +
                    "<h2 style=\"font-family: Helvetica, Arial, sans-serif;font-weight: 300;color: #ff6600;font-size: 30px;line-height: 32px;margin: 30px 0px 5px 0px;text-align: center;\">" +
                        $"{user.Conference.name}" +
                    "</h2>" +
                    "<br />" + 
                    "<p style=\"font-family: Helvetica, Arial, sans-serif;font-weight: 200;color: #111;margin-right: 15%;margin-left: 15%;text-align: justify;font-size: 18px;line-height: 20px;\">" +
                         $"Hola {user.User.FullName}!" +
                    "</p>" +
                    "<p style=\"font-family: Helvetica, Arial, sans-serif;font-weight: 200;color: #111;margin-right: 15%;margin-left: 15%;text-align: justify;font-size: 16px;line-height: 18px;\">" +
                        $"{mailBodyMessage}" +
                        "<div style=\"font-family: Helvetica, Arial, sans-serif;padding-top: 20px;text-align: center;font-size: 13px;display: block;\" >" +
                           "<p style=\"font-family: Helvetica, Arial, sans-serif;padding-top: 30px;text-align: center;font-size: 13px;\" >" +
                              "<div style=\"color: #ff6600;display: inline\"> Huihuinga </div>" +
                              "<div style=\"color: #afafaf;display: inline\"> © 2019 All Rights Reserved </div>" +
                           "</p>" +
                        "</div>" +
                    "</p>";

                bodyBuilder.TextBody = mailBodyMessage;
                message.Body = bodyBuilder.ToMessageBody();

                using (var emailClient = new SmtpClient())
                {
                    //The last parameter here is to use SSL (Which you should!)
                    emailClient.Connect(_emailConfiguration.SmtpServer, _emailConfiguration.SmtpPort, true);

                    //Remove any OAuth functionality as we won't be using it. 
                    emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

                    emailClient.Authenticate(_emailConfiguration.SmtpUsername, _emailConfiguration.SmtpPassword);

                    emailClient.Send(message);

                    emailClient.Disconnect(true);
                }
            }
            
            return true;
        }

        public async Task<bool> SendEventNotification(ApplicationUserEvent[] userEvents, string mailBodyMessage)
        {
            foreach (var user in userEvents)
            {
                MimeMessage message = new MimeMessage();

                MailboxAddress from = new MailboxAddress("Huihuinga", "admin@huihuinga.com");
                message.From.Add(from);


                var userName = await _userManager.GetUserNameAsync(user.User);
                var email = await _userManager.GetEmailAsync(user.User);
                MailboxAddress to = new MailboxAddress(userName, email);
                message.To.Add(to);
                //message.To.AddRange(users.Select(u => new MailboxAddress(_userManager.GetUserNameAsync(u.User), _userManager.GetEmailAsync(u.User))));

                message.Subject = $"Huihuinga - {user.Event.name}";

                BodyBuilder bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = "" +
                    "<h2 style=\"font-family: Helvetica, Arial, sans-serif;font-weight: 300;color: #ff6600;font-size: 30px;line-height: 32px;margin: 30px 0px 5px 0px;text-align: center;\">" +
                        $"{user.Event.name}" +
                    "</h2>" +
                    "<br />" +
                    "<p style=\"font-family: Helvetica, Arial, sans-serif;font-weight: 200;color: #111;margin-right: 15%;margin-left: 15%;text-align: justify;font-size: 18px;line-height: 20px;\">" +
                         $"Hola {user.User.FullName}!" +
                    "</p>" +
                    "<p style=\"font-family: Helvetica, Arial, sans-serif;font-weight: 200;color: #111;margin-right: 15%;margin-left: 15%;text-align: justify;font-size: 16px;line-height: 18px;\">" +
                        $"{mailBodyMessage}" +
                        "<div style=\"font-family: Helvetica, Arial, sans-serif;padding-top: 20px;text-align: center;font-size: 13px;display: block;\" >" +
                           "<p style=\"font-family: Helvetica, Arial, sans-serif;padding-top: 30px;text-align: center;font-size: 13px;\" >" +
                              "<div style=\"color: #ff6600;display: inline\"> Huihuinga </div>" +
                              "<div style=\"color: #afafaf;display: inline\"> © 2019 All Rights Reserved </div>" +
                           "</p>" +
                        "</div>" +
                    "</p>";

                bodyBuilder.TextBody = mailBodyMessage;
                message.Body = bodyBuilder.ToMessageBody();

                using (var emailClient = new SmtpClient())
                {
                    //The last parameter here is to use SSL (Which you should!)
                    emailClient.Connect(_emailConfiguration.SmtpServer, _emailConfiguration.SmtpPort, true);

                    //Remove any OAuth functionality as we won't be using it. 
                    emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

                    emailClient.Authenticate(_emailConfiguration.SmtpUsername, _emailConfiguration.SmtpPassword);

                    emailClient.Send(message);

                    emailClient.Disconnect(true);
                }
            }

            return true;
        }
    }
}
