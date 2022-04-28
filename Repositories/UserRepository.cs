namespace Caketime.Repositories;

public interface IUserRepository
{
    Task<User> AddUser(User user);
    Task<User> GetUserByUid(string uid);
    Task<List<User>> GetUsers();
    Task<User> GetUserByMail(string email);
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
}