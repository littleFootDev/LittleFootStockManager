using System.ComponentModel.DataAnnotations.Schema;

namespace LittleFootStockManager.Data.Model
{
    public class Event
    {
        public Guid Id { get; set; }
        public string NameClient { get; set; }
        public string Lieux { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public DateTime DDayDate { get; set; }

        public ICollection<Product> Products { get; set; }

        [ForeignKey(nameof(UserId))]
        public string UserId { get; set; }
        public Users User { get; set; }
    }
}
