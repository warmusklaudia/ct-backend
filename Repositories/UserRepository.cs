namespace Caketime.Repositories;

public interface IUserRepository
{
    Task<User> AddUser(User user);
    Task<User> GetUserByUid(string uid);
    Task<List<User>> GetUsers();
    Task<User> GetUserByMail(string email);
    Task<User> AddToFavoriteRecipes(string userUid, Recipe recipe);
    Task<User> DeleteFromFavoriteRecipes(string userUid, Recipe recipe);
    Task<User> AddMyRecipe(string userUid, Recipe recipe);
    Task<User> DeleteMyRecipe(string userUid, Recipe recipe);
}

public class UserRepository : IUserRepository
{
    private readonly IMongoContext _context;

    public UserRepository(IMongoContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetUsers()
    {
        var projection = Builders<User>.Projection.Exclude(u => u.Password);

        var users = (await _context.UserCollection.FindAsync(
            filter: _ => true,
            options: new FindOptions<User, User> { Projection = projection })
            ).ToListAsync();
        return await users;
    }

    public async Task<User> GetUserByUid(string uid)
    {
        var projection = Builders<User>.Projection.Exclude(u => u.Password);

        var user = (await _context.UserCollection.FindAsync(
            filter: u => u.UID == uid,
            options: new FindOptions<User, User> { Projection = projection })
            ).SingleOrDefaultAsync();

        return await user;
    }
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

    public async Task<User> AddMyRecipe(string userUid, Recipe recipe)
    {
        try
        {
            User user = await GetUserByUid(userUid);
            var filter = Builders<User>.Filter.Eq("UID", userUid);
            if (user.MyRecipes == null)
            {
                List<Recipe> listRecipes = new();
                listRecipes.Add(recipe);
                var update = Builders<User>.Update.Set(r => r.MyRecipes, listRecipes);
                var result = await _context.UserCollection.UpdateOneAsync(filter, update);
                return await GetUserByUid(userUid);
            }
            else
            {
                var update = Builders<User>.Update.Set(r => r.MyRecipes[user.MyRecipes.Count()], recipe);
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

    public async Task<User> DeleteMyRecipe(string userUid, Recipe recipe)
    {
        try
        {
            User user = await GetUserByUid(userUid);
            var filter = Builders<User>.Filter.Eq("UID", userUid);
            if (user.MyRecipes != null)
            {
                Recipe r = new();
                user.MyRecipes.RemoveAll(r => r.RecipeId == recipe.RecipeId);
                var update = Builders<User>.Update.Set(r => r.MyRecipes, user.MyRecipes);
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