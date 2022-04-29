
namespace Caketime.DTO;
public class UserDTO
{
    public string UID { get; set; }
    public string Email { get; set; }
    public List<Recipe>? MyRecipes { get; set; }
    public List<Recipe>? FavoriteRecipes { get; set; }
}