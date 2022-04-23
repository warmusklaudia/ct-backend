var builder = WebApplication.CreateBuilder(args);
//Mongo
var mongoSettings = builder.Configuration.GetSection("MongoConnection");
builder.Services.Configure<DatabaseSettings>(mongoSettings);
builder.Services.AddTransient<IMongoContext, MongoContext>();
//Repositories
builder.Services.AddTransient<IIngredientRepository, IngredientRepository>();
builder.Services.AddTransient<IInstructionRepository, InstructionRepository>();
builder.Services.AddTransient<IRecipeRepository, RecipeRepository>();
builder.Services.AddTransient<ICategoryRepository, CategoryRepository>();
//Services
builder.Services.AddTransient<IRecipeService, RecipeService>();

var app = builder.Build();

//GET
app.MapGet("/", () => "Hello World!");
app.MapGet("/setup", (IRecipeService recipeService) => recipeService.DummyData());
app.MapGet("/categories", async (IRecipeService recipeService) =>
{
    try
    {
        var results = await recipeService.GetCategories();
        return Results.Ok(results);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        throw;
    }
});

app.MapGet("/categories/{name}", async (IRecipeService recipeService, string name) =>
{
    try
    {
        var result = await recipeService.GetCategoryByName(name);
        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        throw;
    }
});

app.MapGet("/recipes", async (IRecipeService recipeService) =>
{
    try
    {
        var results = await recipeService.GetRecipes();
        return Results.Ok(results);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        throw;
    }
});

// POST
app.MapPost("/categories", async (IRecipeService recipeService, Category category) =>
{
    try
    {
        var result = await recipeService.AddCategory(category);
        return Results.Created($"/categories/{result.Id}", result);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        throw;
    }
});

app.MapPost("/recipes", async (IRecipeService recipeService, Recipe recipe) =>
{
    try
    {
        var result = await recipeService.AddRecipe(recipe);
        return Results.Created($"/recipes/{result.RecipeId}", result);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        throw;
    }
});

app.Run("http://localhost:3000");
