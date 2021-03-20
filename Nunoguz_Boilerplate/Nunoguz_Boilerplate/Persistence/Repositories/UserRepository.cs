using Nunoguz_Boilerplate.DataModels.RequestModels.Users;
using Nunoguz_Boilerplate.Domain.Models;
using Nunoguz_Boilerplate.Domain.Repositories;
using Nunoguz_Boilerplate.Shared;
using Nunoguz_Boilerplate.Shared.Generators;
using Nunoguz_Boilerplate.Shared.Utilities;
using Nunoguz_Boilerplate.Persistence.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nunoguz_Boilerplate.Persistence.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITokenGenerator _tokenGenerator;

        public UserRepository(DatabaseContext context, IHttpContextAccessor httpContextAccessor, ITokenGenerator tokenGenerator, ILogger<UserRepository> logger) : base(context, logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<bool> Create(User user)
        {
            if (!string.IsNullOrEmpty(user.Email) && !string.IsNullOrEmpty(user.HashedPassword) && !string.IsNullOrEmpty(user.Salt))
            {
                try
                {
                    await _context.Users.AddAsync(user);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"User created to db -> {user.Email} - {user.FullName}");
                    return true;
                }
                catch (Exception exception)
                {
                    _logger.LogInformation($"Yeni kullanıcı veri tabanına kaydedilirken bir hata meydana geldi -> {exception.Message}");
                    throw new ApiException(new Error { Message = "Yeni kullanıcı veri tabanına kaydedilirken bir hata meydana geldi.", StackTrace = exception.StackTrace });
                }
            }
            _logger.LogError($"Yeni kullanıcının email password veya salt keyi null");
            throw new ApiException(new Error { Message = "Yeni kullanıcı kayıt edilirken bir hata meydana geldi." });
        }

        public User getThisUser()
        {
            var user = _httpContextAccessor.HttpContext.GetThisUser(_context);
            return user == null ? throw new ApiException(new Error { Message = "User not found!" }) : user;

        }

        public User GetUserById(int id)
        {
            var user = _context.Users.Where(u => u.Id == id).SingleOrDefault();

            return user == null
                ? throw new ApiException(new Error { Message = "Böyle bir kullanıcı sistemde mevcut değil" })
                : user;
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            var user = await _context.Users.Where(u => u.Id == id).SingleOrDefaultAsync();
            return user == null
                ? throw new ApiException(new Error { Message = "Böyle bir kullanıcı sistemde mevcut değil" })
                : user;
        }

        #region GetUserByEmail
        public async Task<User> GetUserByMailAsync(string email)
        {
            return await _context.Users.Where(u => u.Email.Trim().ToLower() == email.Trim().ToLower()).FirstOrDefaultAsync();
        }
        #endregion

        public async Task<User> GetUserByMailThrowAsync(string email)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email.Trim().ToLower() == email.Trim().ToLower());
            return user == null
                ? throw new ApiException(new Error { Message = "Girilen mail sistemde mevcut değil" })
                : user;
        }

        public async Task<List<User>> GetUserByName(string name)
        {
            return await _context.Users
               .Where(u => u.FirstName.Trim().ToLower() == name.Trim().ToLower() || u.LastName.Trim().ToLower() == name.Trim().ToLower())
               .OrderBy(o => o.FirstName)
               .ToListAsync();
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<bool> Save()
        {
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception exception)
            {
                _logger.LogError($"Couldn't save changes -> {exception.Message} -- ST = {exception.StackTrace}");
                throw new ApiException(new Error { Message = "Veri tabanı güncellenirken bir hata olustu.", StackTrace = exception.StackTrace });
            }
        }

        public async Task UpdateUser(User user)
        {
            if (await isGsmNumberMatchWithAnother(user))
                throw new ApiException(new Error { Message = "Girdiğiniz telefon numarası sistemde başka bir kullanıcının numarası ile eşleşmekte. Lütfen kontrol ediniz, farklı bir numara giriniz" });
            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                _logger.LogInformation($"Couldn't update the user. Sth went wrong = {exception.Message} - {exception.InnerException} - {exception.StackTrace}");
                throw new ApiException(new Error
                {
                    Message = "Kullanıcı güncellenirken bir hata meydana geldi",
                    StackTrace = exception.StackTrace
                });
            }
        }

        private async Task<bool> isGsmNumberMatchWithAnother(User user)
        {
            if (user.GsmNumber != null)
            {
                var userGsmLast10 = user.GsmNumber.Length > 10 ? user.GsmNumber.Substring(user.GsmNumber.Length - 10, 10) : user.GsmNumber;
                var usersNumbers = await _context.Users.Where(u => u.isActive && u.Id != user.Id).Select(u => u.GsmNumber).ToListAsync();
                foreach (var number in usersNumbers)
                {
                    var compareNumber = number.Length > 10 ? number.Substring(number.Length - 10, 10) : number;
                    if (userGsmLast10 == compareNumber)
                        return true;
                }
            }
            return false;
        }

        #region User Login
        public async Task<User> LoginUserAsync(LoginRequest request)
        {
            var user = await _context.Users.Where(U => U.Email == request.Email).SingleOrDefaultAsync();
            if (user == null)
            {
                _logger.LogInformation($"Girilen mail veya kullanıcı adı ile kayıt bulunamadı. mail, username = {request.Email}");
                throw new ApiException(new Error
                {
                    Message = "Böyle bir kullanıcı mevcut değil"
                });
            }

            var hashedPassword = PassHasherGenerator.HashPassword(request.Password, user.Salt).Item2;
            return hashedPassword == user.HashedPassword
                ? user
                : throw new ApiException(new Error
                {
                    Message = "Girilen parola doğru değil"
                });
        }
        #endregion
    }
}
