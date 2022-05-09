public interface IProductRepository
{
    Task<Product> AddProduct(Product product);
    Task<Product> GetProductById(string id);
    Task<List<Product>> GetProducts();
    // Task<Product> UpdateProductPromo(string id, bool promo);
}

public class ProductRepository : IProductRepository
{
    private readonly IMongoContext _context;

    public ProductRepository(IMongoContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetProducts() => await _context.ProductCollection.Find(_ => true).ToListAsync();


    public async Task<Product> GetProductById(string id) => await _context.ProductCollection.Find<Product>(p => p.Id == id).FirstOrDefaultAsync();
    public async Task<Product> AddProduct(Product product)
    {
        await _context.ProductCollection.InsertOneAsync(product);
        return product;
    }

    // public async Task<Product> UpdateProductPromo(string id, bool promo)
    // {
    //     try
    //     {
    //         var filter = Builders<Product>.Filter.Eq("Id", id);
    //         var update = Builders<Product>.Update.Set("Promo", promo);
    //         var result = await _context.ProductCollection.UpdateOneAsync(filter, update);
    //         return await GetProductById(id);
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine(ex);
    //         throw;
    //     }

    // }


}