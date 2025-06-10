using Microsoft.EntityFrameworkCore;
using Web_1773.Models;
using Web_1773.Data;
using System.Linq;

namespace Web_1773.Repositories
{
    public class EFProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public EFProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Category) 
                .ToListAsync();
        }        public async Task<Product> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }        public async Task UpdateAsync(Product product)
        {
            // Detach any existing tracked entity with the same key to avoid tracking conflicts
            var existingEntry = _context.Entry(product);
            if (existingEntry.State == EntityState.Detached)
            {
                var trackedEntity = _context.Products.Local.FirstOrDefault(p => p.Id == product.Id);
                if (trackedEntity != null)
                {
                    _context.Entry(trackedEntity).State = EntityState.Detached;
                }
            }
            
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
    }
}
