using JWTAuth.DTOs;
using JWTAuth.Models;

namespace JWTAuth.Services
{
    public interface IUserService
    {
        Task<AuthenticateResponseDTO?> Authenticate(AuthenticateRequestDTO model);
        Task<IEnumerable<User>> GetAll();
        Task<User?> GetById(int id);
        Task<User?> AddAndUpdateUser(User userObj);
    }
}