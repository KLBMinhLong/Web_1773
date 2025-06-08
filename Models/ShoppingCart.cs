namespace Web_1773.Models
{
    public class ShoppingCart
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        
        public decimal TotalAmount => Items.Sum(i => i.Price * i.Quantity);
        
        public int TotalItems => Items.Sum(i => i.Quantity);
        
        public void AddItem(CartItem item)
        {
            var existingItem = Items.FirstOrDefault(i => i.ProductId == item.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                Items.Add(item);
            }
        }
        
        public void RemoveItem(int productId)
        {
            Items.RemoveAll(i => i.ProductId == productId);
        }
        
        public void UpdateQuantity(int productId, int quantity)
        {
            var item = Items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                item.Quantity = quantity;
            }
        }
        
        public void Clear()
        {
            Items.Clear();
        }
    }
}