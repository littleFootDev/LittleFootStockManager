namespace LittleFootStockManager.Dto.User
{
    public class AuthReponseDto
    {
        public string UserId { get; set; }
        public bool IsAuthenticated { get; set; }
        //public string Token { get; set; }

        public string ErrorMessage { get; set; }
    }
}
