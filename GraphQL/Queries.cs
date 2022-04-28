namespace Caketime.GraphQL.Queries;

public class Queries
{
    public async Task<List<Recipe>> GetRecipes([Service] IRecipeService recipeService) => await recipeService.GetRecipes();
    public async Task<List<User>> GetUsers([Service] IRecipeService recipeService) => await recipeService.GetUsers();
}
