using JWTAuth.DTOs;
using JWTAuth.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTAuth.Services
{
    public class UserService : IUserService
    {
        private readonly AppSettings _settings;
        private readonly JWTAuthDbContext _dbcontext;

        public UserService(IOptions<AppSettings> settings, JWTAuthDbContext db)
        {
            _settings = settings.Value;
            _dbcontext = db;
        }

        public async Task<AuthenticateResponseDTO?> Authenticate(AuthenticateRequestDTO model)
        {
            var user = await _dbcontext.Users.SingleOrDefaultAsync(x => x.Email == model.Email && x.Password == model.Password);

            // return null if user not found
            if (user == null) return null;

            // authentication successful so generate jwt token
            var token = await generateJwtToken(user);

            return new AuthenticateResponseDTO(user, token);
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await _dbcontext.Users.Where(x => x.IsActive == true).ToListAsync();
        }

        public async Task<User?> GetById(int id)
        {
            return await _dbcontext.Users.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<User?> AddAndUpdateUser(User userObj)
        {
            bool isSuccess = false;
            if (userObj.Id > 0)
            {
                var obj = await _dbcontext.Users.FirstOrDefaultAsync(c => c.Id == userObj.Id);
                if (obj != null)
                {
                    // obj.Address = userObj.Address;
                    obj.Name = userObj.Name;
                    obj.Email = userObj.Email;
                    _dbcontext.Users.Update(obj);
                    isSuccess = await _dbcontext.SaveChangesAsync() > 0;
                }
            }
            else
            {
                await _dbcontext.Users.AddAsync(userObj);
                isSuccess = await _dbcontext.SaveChangesAsync() > 0;
            }

            return isSuccess ? userObj : null;
        }
        // helper methods
        private async Task<string> generateJwtToken(User user)
        {
            //Generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = await Task.Run(() =>
            {

                var key = Encoding.ASCII.GetBytes(_settings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                return tokenHandler.CreateToken(tokenDescriptor);
            });

            return tokenHandler.WriteToken(token);
        }
    }
}
