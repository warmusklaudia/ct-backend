namespace Caketime.Repositories;

public interface IRecipeRepository
{
    Task<Recipe> AddRecipe(Recipe newRecipe);
    Task<List<Recipe>> GetRecipes();
    Task DeleteRecipe(string id);
    Task<List<Recipe>> GetRecipesByOwner(string uid);
    Task<Recipe> GetRecipeById(string id);
    Task<Recipe> UpdatePhoto(string recipeId, string uri);
}

public class RecipeRepository : IRecipeRepository
{
    private readonly IMongoContext _context;

    public RecipeRepository(IMongoContext context)
    {
        _context = context;
    }

    public async Task<List<Recipe>> GetRecipes() => await _context.RecipeCollection.Find(_ => true).ToListAsync();

    public async Task<Recipe> GetRecipeById(string id) => await _context.RecipeCollection.Find<Recipe>(r => r.RecipeId == id).FirstOrDefaultAsync();

    public async Task<List<Recipe>> GetRecipesByOwner(string uid) => await _context.RecipeCollection.Find(r => r.uidOwner == uid).ToListAsync();

    public async Task<Recipe> AddRecipe(Recipe newRecipe)
    {
        await _context.RecipeCollection.InsertOneAsync(newRecipe);
        return newRecipe;
    }

    public async Task<Recipe> UpdatePhoto(string recipeId, string uri)
    {
        try
        {
            var filter = Builders<Recipe>.Filter.Eq("RecipeId", recipeId);
            var update = Builders<Recipe>.Update.Set("Img", uri);
            var result = await _context.RecipeCollection.UpdateOneAsync(filter, update);
            return await GetRecipeById(recipeId);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }

    }

    // public async Task<Recipe> UpdateFavorite(string recipeid, string uid)
    // {
    //     try
    //     {
    //         Recipe recipe = await GetRecipeById(recipeid);
    //         for (int x = 0; x < recipe.uidFavorites.Count(); x++)
    //         {
    //             if (recipe.uidFavorites[x] == uid)
    //             {
    //                 var filter = Builders<Recipe>.Filter.Eq("RecipeId", recipeid);
    //                 var update = Builders<Recipe>.Update.Set($"UidFavorites", ' ');
    //                 var result = await _context.RecipeCollection.UpdateOneAsync(filter, update);
    //             }
    //             else
    //             {
    //                 var filter = Builders<Recipe>.Filter.Eq("RecipeId", recipeid);
    //                 var update = Builders<Recipe>.Update.Set($"UidFavorites", uid);
    //                 var result = await _context.RecipeCollection.UpdateOneAsync(filter, update);
    //             }
    //         }
    //         return recipe;
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine(ex);
    //         throw;
    //     }
    // }
    public async Task DeleteRecipe(string id) => await _context.RecipeCollection.DeleteOneAsync(r => r.RecipeId == id);
}