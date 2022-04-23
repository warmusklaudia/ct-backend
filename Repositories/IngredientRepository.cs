namespace Caketime.Repositories;

public interface IIngredientRepository
{
    Task<Ingredient> AddIngredient(Ingredient newIngredient);
    Task<List<Ingredient>> AddIngredients(List<Ingredient> newIngredients);
    Task<List<Ingredient>> GetIngredients();
}

public class IngredientRepository : IIngredientRepository
{
    private readonly IMongoContext _context;

    public IngredientRepository(IMongoContext context)
    {
        _context = context;
    }

    public async Task<List<Ingredient>> GetIngredients() => await _context.IngredientCollection.Find(_ => true).ToListAsync();
    public async Task<List<Ingredient>> AddIngredients(List<Ingredient> newIngredients)
    {
        await _context.IngredientCollection.InsertManyAsync(newIngredients);
        return newIngredients;
    }

    public async Task<Ingredient> AddIngredient(Ingredient newIngredient)
    {
        await _context.IngredientCollection.InsertOneAsync(newIngredient);
        return newIngredient;
    }
}