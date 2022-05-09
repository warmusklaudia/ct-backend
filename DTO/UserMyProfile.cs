namespace Caketime.DTO;

public class UserMyProfile : Profile
{
    public UserMyProfile()
    {
        CreateMap<User, UserMyDTO>();
    }
}