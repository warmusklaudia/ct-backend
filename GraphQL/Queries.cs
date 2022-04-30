namespace Caketime.GraphQL.Queries;

public class Queries
{
    public async Task<List<Recipe>> GetRecipes([Service] IRecipeService recipeService) => await recipeService.GetRecipes();
    public async Task<Recipe> GetRecipe([Service] IRecipeService recipeService, string id) => await recipeService.GetRecipeById(id);
    public async Task<List<User>> GetUsers([Service] IRecipeService recipeService) => await recipeService.GetUsers();
    public async Task<List<Ingredient>> GetIngredients([Service] IRecipeService recipeService) => await recipeService.GetIngredients();
    public async Task<List<Instruction>> GetInstructions([Service] IRecipeService recipeService) => await recipeService.GetInstructions();
    public async Task<List<Category>> GetCategories([Service] IRecipeService recipeService) => await recipeService.GetCategories();
}
