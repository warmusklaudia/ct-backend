namespace Caketime.GraphQL.Queries;

public class Query
{
    public async Task<List<Recipe>> GetRecipes([Service] IRecipeService recipeService) => await recipeService.GetRecipes();
}
