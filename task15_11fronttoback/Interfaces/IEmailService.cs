namespace task15_11fronttoback.Interfaces
{
    public interface IEmailService
    {
        Task SendMailAsync(string emailTo, string subject, string body, bool ishtml = false);
    }
}
