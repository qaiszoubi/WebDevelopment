using email_sender;
using System.Net;
using System.Net.Mail; // لنستخدم SmtpClient من System.Net.Mail
using System.Threading.Tasks;
using MimeKit; // للتعامل مع MIME
using MailKit.Net.Smtp; // مكتبة MailKit لإرسال البريد الإلكتروني مع مرفقات

public class EmailSender : IEmailSender
{
    private readonly string smtpHost = "smtp.office365.com"; // خادم SMTP
    private readonly int smtpPort = 587; // المنفذ (587 للـ TLS)
    private readonly string smtpUser = "hotlrev@outlook.com"; // بريدك الإلكتروني
    private readonly string smtpPass = "Asdf@12345"; // كلمة المرور

    // دالة لإرسال البريد الإلكتروني بدون مرفقات
    public async Task SendEmailAsync(string email, string subject, string message)
    {
        using (var client = new System.Net.Mail.SmtpClient(smtpHost, smtpPort)) // تحديد SmtpClient من System.Net.Mail
        {
            client.Credentials = new NetworkCredential(smtpUser, smtpPass);
            client.EnableSsl = true;

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpUser),
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(email);
            await client.SendMailAsync(mailMessage);
        }
    }

    // دالة لإرسال البريد الإلكتروني مع مرفق
    public async Task SendEmailWithAttachment(string email, string subject, byte[] attachment)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Hotel Reservation", smtpUser)); // العنوان والاسم المرسل
        message.To.Add(MailboxAddress.Parse(email)); // المستلم
        message.Subject = subject;

        // بناء الجسم والمرفقات
        var builder = new BodyBuilder
        {
            TextBody = "Please find the attached file."
        };

        // إضافة المرفق
        builder.Attachments.Add("invoice.pdf", attachment, ContentType.Parse("application/pdf"));
        message.Body = builder.ToMessageBody();

        using (var client = new MailKit.Net.Smtp.SmtpClient()) // تحديد SmtpClient من MailKit
        {
            await client.ConnectAsync(smtpHost, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(smtpUser, smtpPass);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
