namespace Caketime.Services;

public interface IUserService
{
    Task<User> AddUser(User user);
    Task<User> GetUserByUID(string uid);
    Task<List<User>> GetUsers();
    Task<User> GetUserByMail(string email);
}

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<User>> GetUsers() => await _userRepository.GetUsers();
    public async Task<User> GetUserByUID(string uid) => await _userRepository.GetUserByUid(uid);
    public async Task<User> GetUserByMail(string email) => await _userRepository.GetUserByMail(email);
    public async Task<User> AddUser(User user)
    {

        var checkUser = await GetUserByMail(user.Email);
        if (checkUser == null)
        {
            return await _userRepository.AddUser(user);
        }
        else
        {
            throw new ArgumentException("An account with this email already exist");
        }
    }
}