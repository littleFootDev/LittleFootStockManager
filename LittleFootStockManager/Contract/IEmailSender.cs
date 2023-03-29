namespace LittleFootStockManager.Contract
{
    public interface IEmailSender
    {
        Task SendEmail(string to, string subject, string body);
    }
}
