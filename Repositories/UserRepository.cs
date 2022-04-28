namespace Caketime.Repositories;

public interface IUserRepository
{
    Task<User> AddUser(User user);
    Task<User> GetUserByUid(string uid);
    Task<List<User>> GetUsers();
    Task<User> GetUserByMail(string email);
    Task<User> AddToFavoriteRecipes(string userUid, Recipe recipe);
    Task<User> DeleteFromFavoriteRecipes(string userUid, Recipe recipe);
}

public class UserRepository : IUserRepository
{
    private readonly IMongoContext _context;

    public UserRepository(IMongoContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetUsers() => await _context.UserCollection.Find(_ => true).ToListAsync();

    public async Task<User> GetUserByUid(string uid) => await _context.UserCollection.Find<User>(u => u.UID == uid).FirstOrDefaultAsync();
    public async Task<User> GetUserByMail(string email) => await _context.UserCollection.Find<User>(u => u.Email == email).FirstOrDefaultAsync();

    public async Task<User> AddUser(User user)
    {
        await _context.UserCollection.InsertOneAsync(user);
        return user;
    }

    public async Task<User> AddToFavoriteRecipes(string userUid, Recipe recipe)
    {
        try
        {
            User user = await GetUserByUid(userUid);
            var filter = Builders<User>.Filter.Eq("UID", userUid);
            if (user.FavoriteRecipes == null)
            {
                List<Recipe> listRecipes = new();
                listRecipes.Add(recipe);
                var update = Builders<User>.Update.Set(r => r.FavoriteRecipes, listRecipes);
                var result = await _context.UserCollection.UpdateOneAsync(filter, update);
                return await GetUserByUid(userUid);
            }
            else
            {
                var update = Builders<User>.Update.Set(r => r.FavoriteRecipes[user.FavoriteRecipes.Count()], recipe);
                var result = await _context.UserCollection.UpdateOneAsync(filter, update);
                return await GetUserByUid(userUid);
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }

    }

    public async Task<User> DeleteFromFavoriteRecipes(string userUid, Recipe recipe)
    {
        try
        {
            User user = await GetUserByUid(userUid);
            var filter = Builders<User>.Filter.Eq("UID", userUid);
            if (user.FavoriteRecipes != null)
            {
                Recipe r = new();
                user.FavoriteRecipes.RemoveAll(r => r.RecipeId == recipe.RecipeId);
                var update = Builders<User>.Update.Set(r => r.FavoriteRecipes, user.FavoriteRecipes);
                var result = await _context.UserCollection.UpdateOneAsync(filter, update);
            }
            return await GetUserByUid(userUid);

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }

    }

}