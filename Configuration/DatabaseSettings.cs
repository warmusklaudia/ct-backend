namespace Caketime.Configuration;

public class DatabaseSettings
{
    public string? ConnectionString { get; set; }
    public string? DatabaseName { get; set; }
    public string? IngredientCollection { get; set; }
    public string? InstructionCollection { get; set; }
    public string? RecipeCollection { get; set; }
    public string? CategoryCollection { get; set; }
}