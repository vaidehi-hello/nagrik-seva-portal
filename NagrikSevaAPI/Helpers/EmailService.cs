using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace NagrikSevaAPI.Helpers
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendVerificationEmailAsync(
            string toEmail,
            string toName,
            string verificationLink)
        {
            var email = new MimeMessage();

            // From
            email.From.Add(new MailboxAddress(
                _config["EmailSettings:FromName"],
                _config["EmailSettings:FromEmail"]));

            // To
            email.To.Add(new MailboxAddress(toName, toEmail));

            // Subject
            email.Subject =
                "Verify Your Email — Nagrik Seva Portal";

            // HTML Body
            email.Body = new TextPart("html")
            {
                Text = $@"
                <!DOCTYPE html>
                <html>
                <head>
                  <style>
                    body {{
                      font-family: Arial, sans-serif;
                      background: #f4f4f4;
                      margin: 0;
                      padding: 0;
                    }}
                    .container {{
                      max-width: 600px;
                      margin: 30px auto;
                      background: white;
                      border-radius: 8px;
                      overflow: hidden;
                      box-shadow: 0 2px 10px rgba(0,0,0,0.1);
                    }}
                    .header {{
                      background: #1a3a6b;
                      padding: 30px;
                      text-align: center;
                      color: white;
                    }}
                    .header h1 {{
                      margin: 0;
                      font-size: 22px;
                    }}
                    .header p {{
                      margin: 8px 0 0;
                      opacity: 0.8;
                      font-size: 13px;
                    }}
                    .body {{
                      padding: 30px;
                    }}
                    .body p {{
                      color: #555;
                      font-size: 15px;
                      line-height: 1.6;
                    }}
                    .btn-wrap {{
                      text-align: center;
                      margin: 28px 0;
                    }}
                    .btn {{
                      display: inline-block;
                      padding: 14px 36px;
                      background: #FF6B00;
                      color: white;
                      text-decoration: none;
                      border-radius: 6px;
                      font-size: 15px;
                      font-weight: bold;
                    }}
                    .warning {{
                      background: #fff3cd;
                      border: 1px solid #ffc107;
                      border-radius: 6px;
                      padding: 12px 16px;
                      margin-top: 20px;
                      color: #856404;
                      font-size: 13px;
                    }}
                    .link-box {{
                      margin-top: 20px;
                      padding: 12px;
                      background: #f8f8f8;
                      border-radius: 6px;
                      font-size: 12px;
                      color: #999;
                      word-break: break-all;
                    }}
                    .footer {{
                      background: #f4f4f4;
                      padding: 20px;
                      text-align: center;
                      color: #999;
                      font-size: 12px;
                      border-top: 1px solid #e0e0e0;
                    }}
                  </style>
                </head>
                <body>
                  <div class='container'>

                    <div class='header'>
                      <h1>🏛️ Nagrik Seva Portal</h1>
                      <p>Government of India — Digital Services</p>
                    </div>

                    <div class='body'>
                      <p>Dear <strong>{toName}</strong>,</p>

                      <p>Thank you for registering with
                         <strong>Nagrik Seva Portal</strong>.
                         To complete your registration, please
                         verify your email address by clicking
                         the button below:</p>

                      <div class='btn-wrap'>
                        <a href='{verificationLink}' class='btn'>
                          ✅ Verify My Email Address
                        </a>
                      </div>

                      <div class='warning'>
                        ⏰ This verification link will expire in
                        <strong>24 hours</strong>. If you did not
                        create an account on Nagrik Seva Portal,
                        please ignore this email.
                      </div>

                      <div class='link-box'>
                        <strong>If button doesn't work,
                        copy this link:</strong><br/>
                        <a href='{verificationLink}'
                           style='color:#1a3a6b'>
                          {verificationLink}
                        </a>
                      </div>
                    </div>

                    <div class='footer'>
                      © 2024 Nagrik Seva Portal |
                      Government of India<br/>
                      This is an automated email,
                      please do not reply.
                    </div>

                  </div>
                </body>
                </html>"
            };

            // Connect to Gmail SMTP and send
            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                _config["EmailSettings:SmtpHost"],
                int.Parse(_config["EmailSettings:SmtpPort"]!),
                SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(
                _config["EmailSettings:SmtpUser"],
                _config["EmailSettings:SmtpPass"]);

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}