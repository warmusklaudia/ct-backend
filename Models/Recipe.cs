namespace Caketime.Models;

public class Recipe
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string RecipeId { get; set; }
    public string Name { get; set; }
    public int? Time { get; set; }
    public Category Category { get; set; }
    public int? Servings { get; set; }
    public int? Steps { get; set; }
    public string? Difficulty { get; set; }
    public int? QuantityIngredients { get; set; }
    public List<Ingredient>? Ingredients { get; set; }
    public List<Instruction>? Instructions { get; set; }
    public string? Img { get; set; }
}