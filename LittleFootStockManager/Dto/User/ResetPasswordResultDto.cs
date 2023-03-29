namespace LittleFootStockManager.Dto.User
{
    public class ResetPasswordResultDto
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public string UserId { get; set; }
    }
}
