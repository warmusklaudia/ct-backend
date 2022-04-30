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
//Validation
builder.Services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<RecipeValidator>());
builder.Services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CategoryValidator>());
builder.Services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<UserValidator>());


//GraphQL
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Queries>()
    .ModifyRequestOptions(opt => opt.IncludeExceptionDetails = true)
    .AddMutationType<Mutations>();


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

//Automapper
builder.Services.AddAutoMapper(typeof(Program));

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


app.UseExceptionHandler(c => c.Run(async context =>
{
    var exception = context.Features
        .Get<IExceptionHandlerFeature>()
        ?.Error;
    if (exception is not null)
    {
        var response = new { error = exception.Message };
        context.Response.StatusCode = 400;

        await context.Response.WriteAsJsonAsync(response);
    }
}));

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
        if (result != null)
            return Results.Ok(result);
        else
            return Results.NotFound($"Category {name} not fount");
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        throw;
    }
});

app.MapGet("/categories/{id}", async (IRecipeService recipeService, string id) =>
{
    try
    {
        var result = await recipeService.GetCategoryById(id);
        if (result != null)
            return Results.Ok(result);
        else
            return Results.NotFound($"Category not fount");
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
        if (results != null)
            return Results.Ok(results);
        else
            return Results.NotFound("Ingredient not found");
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
        if (results != null)
            return Results.Ok(results);
        else
            return Results.NotFound("Instruction not found");
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
        if (results != null)
            return Results.Ok(results);
        else
            return Results.NotFound("Recipe not found");
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        throw;
    }
});

app.MapGet("/recipes/all", async (IRecipeService recipeService) =>
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

app.MapGet("/recipes/all/{uid}", [Authorize] async (IRecipeService recipeService, string uid) =>
{
    try
    {
        var results = await recipeService.GetRecipesByOwner(uid);
        if (results != null)
            return Results.Ok(results);
        else
            return Results.NotFound($"This user created no recipes yet");
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        throw;
    }
});


app.MapGet("/users", [Authorize] async (IRecipeService recipeService) =>
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

app.MapGet("/users/{uid}", [Authorize] async (IMapper mapper, IRecipeService recipeService, string uid) =>
{
    try
    {
        var results = await recipeService.GetUserByUID(uid);
        if (results != null)
            return Results.Ok(mapper.Map<UserDTO>(results));
        else
            return Results.NotFound("User not found");
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        throw;
    }
});


// POST

app.MapPost("/categories", async (IValidator<Category> validator, IRecipeService recipeService, Category category) =>
{
    try
    {
        var validationResult = validator.Validate(category);
        if (validationResult.IsValid)
        {
            var result = await recipeService.AddCategory(category);
            return Results.Created($"/categories/{result.Id}", result);
        }
        else
        {
            var errors = validationResult.Errors.Select(x => new { errors = x.ErrorMessage });
            return Results.BadRequest(errors);
        }

    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        throw;
    }
});

app.MapPost("/recipes/all", [Authorize] async (IValidator<Recipe> validator, IRecipeService recipeService, Recipe recipe) =>
{
    try
    {
        var validationResult = validator.Validate(recipe);
        if (validationResult.IsValid)
        {
            var result = await recipeService.AddRecipe(recipe);
            return Results.Created($"/recipes/{result.RecipeId}", result);
        }
        else
        {
            var errors = validationResult.Errors.Select(x => new { errors = x.ErrorMessage });
            return Results.BadRequest(errors);
        }

    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        throw;
    }
});

app.MapPost("/signup", async (IValidator<User> validator, IRecipeService recipeService, User user) =>
{
    try
    {
        var validationResult = validator.Validate(user);
        if (validationResult.IsValid)
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
            return Results.Created($"/users/{result.UID}", result);
        }
        else
        {
            var errors = validationResult.Errors.Select(x => new { errors = x.ErrorMessage });
            return Results.BadRequest(errors);
        }
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
        var filepath = "./assets/Pavlova.jpg";
        var result = await recipeService.UpdatePhoto(recipeId, filepath);
        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        throw;
    }
});

app.MapPut("/users/{uid}/recipes/favorite", [Authorize] async (IRecipeService recipeService, string uid, Recipe recipe) =>
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

app.MapPut("/users/{uid}/recipes/myrecipes/add", [Authorize] async (IRecipeService recipeService, string uid, Recipe recipe) =>
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

app.MapPut("/users/{uid}/recipes/myrecipes/delete", [Authorize] async (IRecipeService recipeService, string uid, Recipe recipe) =>
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
// app.Run();
// public partial class Program { }