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

app.Run("http://0.0.0.0:3000");
