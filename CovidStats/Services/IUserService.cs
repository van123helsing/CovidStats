using CovidStats.Entities;
using CovidStats.Models;

namespace CovidStats.Services
{
    public interface IUserService
    {
        AuthenticateResponse? Authenticate(AuthenticateRequest model);
        IEnumerable<User> GetAll();
        User? GetById(int id);
    }
}
