using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web_1773.Models
{
    public class Order
    {
        public int Id { get; set; }
        
        public string UserId { get; set; }
        
        [Display(Name = "Ngày đặt hàng")]
        public DateTime OrderDate { get; set; }
        
        [Display(Name = "Tổng tiền")]
        public decimal TotalPrice { get; set; }
        
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng")]
        [Display(Name = "Địa chỉ giao hàng")]
        public string ShippingAddress { get; set; }
        
        [Display(Name = "Ghi chú")]
        public string Notes { get; set; }
        
        public IdentityUser User { get; set; }
        
        public List<OrderDetail> OrderDetails { get; set; }
    }
}