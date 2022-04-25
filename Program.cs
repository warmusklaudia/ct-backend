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



//Authentication
FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile(@"./auth/service-account.json")
});

builder.Services.AddAuthorization();
builder.Services.AddAuthentication("Bearer").AddJwtBearer(opt =>
{
    opt.Authority = builder.Configuration["Jwt:Firebase:ValidIssuer"];
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Firebase:ValidIssuer"],
        ValidAudience = builder.Configuration["Jwt:Firebase:ValidAudience"]
    };
});


var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();



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

app.MapGet("/ingredients", async (IRecipeService recipeService) =>
{
    try
    {
        var results = await recipeService.GetIngredients();
        return Results.Ok(results);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        throw;
    }
});

app.MapGet("/ingredients/{name}/{quantity}", async (IRecipeService recipeService, string name, string quantity) =>
{
    try
    {
        var results = await recipeService.GetIngredientByNameAndQuantity(name, quantity);
        return Results.Ok(results);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        throw;
    }
});

app.MapGet("/instructions", async (IRecipeService recipeService) =>
{
    try
    {
        var results = await recipeService.GetInstructions();
        return Results.Ok(results);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        throw;
    }
});

app.MapGet("/instructions/{name}/{manual}", async (IRecipeService recipeService, string name, string manual) =>
{
    try
    {
        var results = await recipeService.GetInstructionByNameAndManual(name, manual);
        return Results.Ok(results);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        throw;
    }
});

app.MapGet("/recipes", [Authorize] async (IRecipeService recipeService) =>
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

app.MapPost("/recipes", [Authorize] async (IRecipeService recipeService, Recipe recipe) =>
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

// PUT

// DELETE 

app.MapDelete("/recipes/{id}", [Authorize] async (IRecipeService recipeService, string id) =>
{
    try
    {
        await recipeService.DeleteRecipe(id);
        return Results.Ok($"Recipe with id {id} deleted successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        throw;
    }
});

app.Run("http://localhost:3000");
