namespace Caketime.Models;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string? UID { get; set; }
    public string Email { get; set; }
    public string DisplayName { get; set; }
    public string Password { get; set; }
    public List<Recipe>? MyRecipes { get; set; }
    public List<Recipe>? FavoriteRecipes { get; set; }
}