using Microsoft.AspNetCore.Identity;

namespace LittleFootStockManager.Data.Model
{
    public class Users : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public ICollection<Product> Products { get; set; }
        public ICollection<Event> Events { get; set; }

    }
}
