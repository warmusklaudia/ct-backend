namespace Caketime.Repositories;

public interface IRecipeRepository
{
    Task<Recipe> AddRecipe(Recipe newRecipe);
    Task<List<Recipe>> GetRecipes();
    Task DeleteRecipe(string id);
    Task<List<Recipe>> GetRecipesByOwner(string uid);
    Task<List<Recipe>> GetUsersFavoriteRecipes(string uid);
}

public class RecipeRepository : IRecipeRepository
{
    private readonly IMongoContext _context;

    public RecipeRepository(IMongoContext context)
    {
        _context = context;
    }

    public async Task<List<Recipe>> GetRecipes() => await _context.RecipeCollection.Find(_ => true).ToListAsync();

    public async Task<Recipe> GetRecipeById(string id) => await _context.RecipeCollection.Find(r => r.RecipeId == id).FirstOrDefaultAsync();

    public async Task<List<Recipe>> GetRecipesByOwner(string uid) => await _context.RecipeCollection.Find(r => r.uidOwner == uid).ToListAsync();
    public async Task<List<Recipe>> GetUsersFavoriteRecipes(string uid)
    {
        List<Recipe> ListAll = await GetRecipes();
        List<Recipe> ListFav = new List<Recipe>();
        for (int i = 0; i < ListAll.Count(); i++)
        {
            for (int x = 0; x < ListAll[i].uidFavorites.Count(); x++)
            {
                // TODO: Move favorites to User Entity
                if (ListAll[i].uidFavorites[x] == uid) ListFav.Add(ListAll[i]);
            }
            Console.WriteLine(ListFav.Count());
        }
        return ListFav;
    }
    public async Task<Recipe> AddRecipe(Recipe newRecipe)
    {
        await _context.RecipeCollection.InsertOneAsync(newRecipe);
        return newRecipe;
    }
    public async Task DeleteRecipe(string id) => await _context.RecipeCollection.DeleteOneAsync(r => r.RecipeId == id);
}