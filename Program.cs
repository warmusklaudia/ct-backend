var builder = WebApplication.CreateBuilder(args);
//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//Mongo
var mongoSettings = builder.Configuration.GetSection("MongoConnection");
builder.Services.Configure<DatabaseSettings>(mongoSettings);
builder.Services.AddTransient<IMongoContext, MongoContext>();
//Blob
var blobSettings = builder.Configuration.GetSection("Blob");
builder.Services.Configure<BlobSettings>(blobSettings);
builder.Services.AddTransient<IBlobService, BlobService>();
//Repositories
builder.Services.AddTransient<IIngredientRepository, IngredientRepository>();
builder.Services.AddTransient<IInstructionRepository, InstructionRepository>();
builder.Services.AddTransient<IRecipeRepository, RecipeRepository>();
builder.Services.AddTransient<ICategoryRepository, CategoryRepository>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
//Services
builder.Services.AddTransient<IRecipeService, RecipeService>();



//GraphQL
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .ModifyRequestOptions(opt => opt.IncludeExceptionDetails = true);


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
//Swagger
app.MapSwagger();
app.UseSwaggerUI();
//GraphQL
app.MapGraphQL();
//Auth
app.UseRouting();
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

app.MapGet("/recipes/recipe/{recipeid}", async (IRecipeService recipeService, string recipeid) =>
{
    try
    {
        var results = await recipeService.GetRecipeById(recipeid);
        return Results.Ok(results);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        throw;
    }
});

app.MapGet("/recipes/all", [Authorize] async (IRecipeService recipeService, ClaimsPrincipal user) =>
{
    try
    {
        // var email = user.FindFirstValue(ClaimTypes.Email);
        var results = await recipeService.GetRecipes();
        return Results.Ok(results);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        throw;
    }
});

app.MapGet("/recipes/all/{uid}", [Authorize] async (IRecipeService recipeService, string uid) =>
{
    try
    {
        var results = await recipeService.GetRecipesByOwner(uid);
        return Results.Ok(results);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        throw;
    }
});

app.MapGet("/recipes/favorites/{uid}", [Authorize] async (IRecipeService recipeService, string uid) =>
{
    try
    {
        var results = await recipeService.GetUsersFavoriteRecipes(uid);
        return Results.Ok(results);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        throw;
    }
});

app.MapGet("/users", async (IRecipeService recipeService) =>
{
    try
    {
        var results = await recipeService.GetUsers();
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

app.MapPost("/recipes/all", [Authorize] async (IRecipeService recipeService, Recipe recipe) =>
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

app.MapPost("/signup", async (IRecipeService recipeService, User user) =>
{
    try
    {
        UserRecordArgs args = new()
        {
            Email = user.Email,
            EmailVerified = false,
            Password = user.Password,
            DisplayName = user.DisplayName,
            Disabled = false,
        };

        var createdUser = await FirebaseAuth.DefaultInstance.CreateUserAsync(args);
        var uid = createdUser.Uid;
        user.UID = uid;
        var result = await recipeService.AddUser(user);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        throw;
    }
});

// PUT

app.MapPut("/recipes/recipe/upload/{recipeid}", [Authorize] async (IRecipeService recipeService, IBlobService blobService, string recipeId) =>
{
    try
    {
        var endpoint = "https://caketime.blob.core.windows.net/recipes/";
        var createBlob = blobService.CreateBlob($"{recipeId}.jpg", "./assets/CarrotMuffins.jpg");
        var result = await recipeService.UpdatePhoto(recipeId, $"{endpoint}{recipeId}.jpg");
        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        throw;
    }
});

app.MapPut("/users/{uid}/recipes/favorite", async (IRecipeService recipeService, string uid, Recipe recipe) =>
{
    try
    {
        var result = await recipeService.ToggleFavorite(uid, recipe);
        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        throw;
    }
});

app.MapPut("/users/{uid}/recipes/myrecipes/add", async (IRecipeService recipeService, string uid, Recipe recipe) =>
{
    try
    {
        var result = await recipeService.AddMyRecipe(uid, recipe);
        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        throw;
    }
});

app.MapPut("/users/{uid}/recipes/myrecipes/delete", async (IRecipeService recipeService, string uid, Recipe recipe) =>
{
    try
    {
        var result = await recipeService.DeleteMyRecipe(uid, recipe);
        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        throw;
    }
});


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
