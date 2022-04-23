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
}

public class RecipeService : IRecipeService
{
    private readonly IIngredientRepository _ingredientRepository;
    private readonly IInstructionRepository _instructionRepository;
    private readonly IRecipeRepository _recipeRepository;
    private readonly ICategoryRepository _categoryRepository;

    public RecipeService(IIngredientRepository ingredientRepository, IInstructionRepository instructionRepository, IRecipeRepository recipeRepository, ICategoryRepository categoryRepository)
    {
        _ingredientRepository = ingredientRepository;
        _instructionRepository = instructionRepository;
        _recipeRepository = recipeRepository;
        _categoryRepository = categoryRepository;
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

    public async Task<List<Instruction>> GetInstructions() => await _instructionRepository.GetInstructions();

    public async Task<List<Recipe>> GetRecipes() => await _recipeRepository.GetRecipes();

    public async Task<List<Category>> GetCategories() => await _categoryRepository.GetCategories();

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
        return await _recipeRepository.AddRecipe(newRecipe);
    }
    public async Task<Category> AddCategory(Category newCategory)
    {
        return await _categoryRepository.AddCategory(newCategory);
    }
}