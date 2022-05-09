namespace Caketime.Validators;

public class CategoryValidator : AbstractValidator<Category>
{
    public CategoryValidator()
    {
        RuleFor(c => c.Name).NotEmpty().WithMessage("Name is obligated");
    }
}

public class CategoryInputValidator : AbstractValidator<AddCategoryInput>
{
    public CategoryInputValidator()
    {
        RuleFor(c => c.name).NotEmpty().WithMessage("Name is obligated");
    }
}