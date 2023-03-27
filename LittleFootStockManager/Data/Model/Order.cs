namespace LittleFootStockManager.Data.Model
{
    public class Order
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string NameClient { get; set; }
        public DateTime DeliveryDate { get; set; }
        public bool IsPayed { get; set; }
        public decimal AmountTotal { get; set; }
        public bool IsArchive { get; set; }


        public virtual ICollection<Product> Products { get; set; }
    }
}
