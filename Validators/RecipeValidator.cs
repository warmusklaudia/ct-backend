namespace Caketime.Validators;

public class RecipeValidator : AbstractValidator<Recipe>
{
    public RecipeValidator()
    {
        RuleFor(r => r.Name).NotEmpty().WithMessage("Name is obligated");
        RuleFor(r => r.Time).GreaterThan(0).WithMessage("Time must be greather than 0");
        RuleFor(r => r.Category.Name).NotEmpty().WithMessage("Category name is obligated");
        RuleFor(r => r.Servings).GreaterThan(0).WithMessage("Servings must be greather than 0");
        RuleFor(r => r.Ingredients).NotEmpty().WithMessage("Ingredients are obligated");
        RuleFor(r => r.Instructions).NotEmpty().WithMessage("Instructions are obligated");
    }
}
