using System.ComponentModel.DataAnnotations;

namespace Web_1773.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        
        public int OrderId { get; set; }
        
        public int ProductId { get; set; }
        
        [Display(Name = "Số lượng")]
        public int Quantity { get; set; }
        
        [Display(Name = "Đơn giá")]
        public decimal Price { get; set; }
        
        public Order Order { get; set; }
        
        public Product Product { get; set; }
    }
}