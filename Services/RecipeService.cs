namespace Caketime.Services;

public interface IRecipeService
{
    Task<Ingredient> AddIngredient(Ingredient newIngredient);
    Task<Instruction> AddInstrucion(Instruction newInstruction);
    Task<Recipe> AddRecipe(Recipe newRecipe);
    Task<Category> AddCategory(Category newCategory);
    Task DummyData();
    Task<List<Ingredient>> GetIngredients();
    Task<List<Instruction>> GetInstructions();
    Task<List<Recipe>> GetRecipes();
    Task<List<Category>> GetCategories();
    Task<Category> GetCategoryByName(string name);
    Task<Ingredient> GetIngredientByNameAndQuantity(string name, string quantity);
    Task<Instruction> GetInstructionByNameAndManual(string name, string manual);
    Task DeleteRecipe(string id);

    Task<List<Recipe>> GetRecipesByOwner(string uid);
    Task<Recipe> GetRecipeById(string id);
    // Task<Recipe> UpdateFavorite(string recipeid, string uid);
    Task<Recipe> UpdatePhoto(string recipeId, string uri);

    // USER
    Task<User> AddUser(User user);
    Task<User> GetUserByUID(string uid);
    Task<List<User>> GetUsers();
    Task<User> GetUserByMail(string email);
    Task<User> ToggleFavorite(string userUid, Recipe recipe);
    Task<User> AddMyRecipe(string userUid, Recipe recipe);
    Task<User> DeleteMyRecipe(string userUid, Recipe recipe);
}

public class RecipeService : IRecipeService
{
    private readonly IIngredientRepository _ingredientRepository;
    private readonly IInstructionRepository _instructionRepository;
    private readonly IRecipeRepository _recipeRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserRepository _userRepository;

    public RecipeService(IIngredientRepository ingredientRepository, IInstructionRepository instructionRepository, IRecipeRepository recipeRepository, ICategoryRepository categoryRepository, IUserRepository userRepository)
    {
        _ingredientRepository = ingredientRepository;
        _instructionRepository = instructionRepository;
        _recipeRepository = recipeRepository;
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
    }
    public async Task DummyData()
    {
        try
        {
            if (!(await _ingredientRepository.GetIngredients()).Any())
                await _ingredientRepository.AddIngredients(new List<Ingredient>() { new Ingredient() { Name = "Flour", Quantity = "120g" }, new Ingredient() { Name = "Egg", Quantity = "2" } });
            if (!(await _instructionRepository.GetInstructions()).Any())
                await _instructionRepository.AddInstructions(new List<Instruction>() { new Instruction() { Name = "Step 1", Manual = "Put Flour in the bowl" } });

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    public async Task<List<Ingredient>> GetIngredients() => await _ingredientRepository.GetIngredients();

    public async Task<Ingredient> GetIngredientByNameAndQuantity(string name, string quantity) => await _ingredientRepository.GetIngredientByNameAndQuantity(name, quantity);

    public async Task<List<Instruction>> GetInstructions() => await _instructionRepository.GetInstructions();

    public async Task<Instruction> GetInstructionByNameAndManual(string name, string manual) => await _instructionRepository.GetInstructionByNameAndManual(name, manual);

    public async Task<List<Recipe>> GetRecipes() => await _recipeRepository.GetRecipes();

    public async Task<Recipe> GetRecipeById(string id) => await _recipeRepository.GetRecipeById(id);

    public async Task<List<Recipe>> GetRecipesByOwner(string uid) => await _recipeRepository.GetRecipesByOwner(uid);

    public async Task<List<Category>> GetCategories() => await _categoryRepository.GetCategories();

    public async Task<Category> GetCategoryByName(string name) => await _categoryRepository.GetCategoryByName(name);
    public async Task<Ingredient> AddIngredient(Ingredient newIngredient)
    {
        return await _ingredientRepository.AddIngredient(newIngredient);
    }
    public async Task<Instruction> AddInstrucion(Instruction newInstruction)
    {
        return await _instructionRepository.AddInstruction(newInstruction);
    }
    public async Task<Recipe> AddRecipe(Recipe newRecipe)
    {
        if (newRecipe == null)
        {
            throw new ArgumentException();
        }
        var category = await GetCategoryByName(newRecipe.Category.Name);
        if (category == null)
        {
            throw new ArgumentException("Category not found!");
        }
        else
        {
            newRecipe.Category = category;
            for (int i = 0; i < newRecipe.Ingredients.Count(); i++)
            {
                var ingredient = await GetIngredientByNameAndQuantity(newRecipe.Ingredients[i].Name, newRecipe.Ingredients[i].Quantity);
                if (ingredient == null)
                {
                    var newIngredient = await AddIngredient(new Ingredient() { Name = newRecipe.Ingredients[i].Name, Quantity = newRecipe.Ingredients[i].Quantity });
                    newRecipe.Ingredients[i] = newIngredient;
                }
                else
                {
                    newRecipe.Ingredients[i] = ingredient;
                }
            }
            for (int i = 0; i < newRecipe.Instructions.Count(); i++)
            {
                var instruction = await GetInstructionByNameAndManual(newRecipe.Instructions[i].Name, newRecipe.Instructions[i].Manual);
                if (instruction == null)
                {
                    var newInstruction = await AddInstrucion(new Instruction() { Name = newRecipe.Instructions[i].Name, Manual = newRecipe.Instructions[i].Manual });
                    newRecipe.Instructions[i] = newInstruction;
                }
                else
                {
                    newRecipe.Instructions[i] = instruction;
                }
            }
            return await _recipeRepository.AddRecipe(newRecipe);
        }
    }
    public async Task<Category> AddCategory(Category newCategory)
    {
        return await _categoryRepository.AddCategory(newCategory);
    }

    public async Task<Recipe> UpdatePhoto(string recipeId, string uri) => await _recipeRepository.UpdatePhoto(recipeId, uri);

    public async Task DeleteRecipe(string id) => await _recipeRepository.DeleteRecipe(id);


    // USER

    public async Task<List<User>> GetUsers() => await _userRepository.GetUsers();
    public async Task<User> GetUserByUID(string uid) => await _userRepository.GetUserByUid(uid);
    public async Task<User> GetUserByMail(string email) => await _userRepository.GetUserByMail(email);
    public async Task<User> AddUser(User user)
    {

        var checkUser = await GetUserByMail(user.Email);
        if (checkUser == null)
        {
            return await _userRepository.AddUser(user);
        }
        else
        {
            throw new ArgumentException("An account with this email already exist");
        }
    }

    public async Task<User> ToggleFavorite(string userUid, Recipe recipe)
    {
        var user = await GetUserByUID(userUid);
        var favoriteRecipes = user.FavoriteRecipes;
        var fullRecipe = await GetRecipeById(recipe.RecipeId);
        if (user.FavoriteRecipes == null)
        {
            return await _userRepository.AddToFavoriteRecipes(userUid, fullRecipe);
        }
        else
        {
            for (int i = 0; i < user.FavoriteRecipes.Count(); i++)
            {
                if (user.FavoriteRecipes[i].RecipeId == recipe.RecipeId)
                {
                    return await _userRepository.DeleteFromFavoriteRecipes(userUid, recipe);
                }
            }
            return await _userRepository.AddToFavoriteRecipes(userUid, fullRecipe);
        }

    }

    public async Task<User> AddMyRecipe(string userUid, Recipe recipe)
    {
        recipe.uidOwner = userUid;
        Recipe newRecipe = await AddRecipe(recipe);
        return await _userRepository.AddMyRecipe(userUid, newRecipe);
    }

    public async Task<User> DeleteMyRecipe(string userUid, Recipe recipe) => await _userRepository.DeleteMyRecipe(userUid, recipe);


}