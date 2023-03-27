using System.ComponentModel.DataAnnotations.Schema;

namespace LittleFootStockManager.Data.Model
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public int Quantity { get; set; }
        public int QuantityAvaible { get; set; }
        public decimal Price { get; set; }
        public string ImagePath { get; set; }

        [ForeignKey(nameof(UserId))]
        public string UserId { get; set; }
        public Users User { get; set; }
    }
}
