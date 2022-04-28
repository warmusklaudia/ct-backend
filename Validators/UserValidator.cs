namespace Caketime.Validators;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(u => u.Email).NotEmpty().WithMessage("Email address is obligated");
        RuleFor(u => u.DisplayName).NotEmpty().WithMessage("Display name is obligated");
        RuleFor(u => u.Password).NotEmpty().WithMessage("Password is obligated").MinimumLength(6).WithMessage("Weak password! Password must be longer than 6 chars");
    }
}