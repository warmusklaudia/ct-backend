namespace Caketime.GraphQL.Mutations;

public interface IMutations
{
    Task<AddCategoryPayload> AddCategory([Service(ServiceKind.Default)] IValidator<AddCategoryInput> validator, [Service(ServiceKind.Default)] IRecipeService recipeService, AddCategoryInput input);
}

public class Mutations : IMutations
{
    public async Task<AddCategoryPayload> AddCategory([Service] IValidator<AddCategoryInput> validator, [Service] IRecipeService recipeService, AddCategoryInput input)
    {
        var validationResult = validator.Validate(input);
        if (validationResult.IsValid)
        {
            var newCategory = new Category()
            {
                Name = input.name,
            };
            var created = await recipeService.AddCategory(newCategory);
            return new AddCategoryPayload(created);
        }
        string message = string.Empty;
        foreach (var error in validationResult.Errors)
        {
            message += error.ErrorMessage;
        }
        throw new Exception(message);
    }
    public async Task<UpdateCategoryPayload> UpdateCategoryName([Service] IRecipeService recipeService, UpdateCategoryInput input)
    {
        var update = await recipeService.UpdateCategory(input.id, input.name);
        return new UpdateCategoryPayload(update);
    }
}