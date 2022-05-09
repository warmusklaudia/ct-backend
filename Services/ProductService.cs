public interface IProductService
{
    Task<Product> AddProduct(Product newProduct);
    Task<List<Product>> GetProducts();
    Task<Product> GetProductById(string id);
}

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<List<Product>> GetProducts() => await _productRepository.GetProducts();

    public async Task<Product> AddProduct(Product newProduct)
    {
        if (newProduct.Stock < 50 && newProduct.Prijs > 100)
        {
            newProduct.Promo = true;
        }
        else
        {
            newProduct.Promo = false;
        }
        return await _productRepository.AddProduct(newProduct);
    }
    public async Task<Product> GetProductById(string id) => await _productRepository.GetProductById(id);


}