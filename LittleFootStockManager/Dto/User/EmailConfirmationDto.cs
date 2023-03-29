namespace LittleFootStockManager.Dto.User
{
    public class EmailConfirmationDto
    {
        public bool IsSucccesFull { get; set; }
        public string Email { get; set; }
        public string UserId { get; set; }
        public DateTime ConfirmationDate { get; set; }
        public string ErrorMessage { get; set; }
    }
}
