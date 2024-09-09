namespace email_sender
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
        Task SendEmailWithAttachment(string email, string subject, byte[] pdfBytes);
    }

}