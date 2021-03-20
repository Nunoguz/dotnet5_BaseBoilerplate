using Nunoguz_Boilerplate.Controllers.Base.v1;
using Nunoguz_Boilerplate.DataModels.RequestModels.Users;
using Nunoguz_Boilerplate.DataModels.ResponseModels;
using Nunoguz_Boilerplate.Domain.Services;
using Nunoguz_Boilerplate.Shared.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nunoguz_Boilerplate.Controllers.v1
{
    [ApiVersion("1.0")]
    [AllowAnonymous]
    [ValidateModel]
    public class UsersController : BaseV1ApiController
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        #region GetUserJWT
        /// <summary>
        /// Returns An User By JWT Bearer
        /// </summary>
        /// <remarks>
        /// 
        ///     Sample request:
        ///             GET /api/v1/users/auth
        ///                 
        /// </remarks>
        /// <response code="200"> Gets an user in the system</response>
        /// <response code="400"> Unable to get this user due to does not match or wrong JWT</response>
        [Shared.Utilities.Authorize]
        [HttpPost]
        [Route("Auth")]
        public IActionResult GetUserByAuth()
        {
            var user = _userService.GetUserAuth();
            if (user != null)
                return Success("User Found.", null, new UserResponse { User = user });

            _logger.LogError("Doesn't find user via jwt bearer auth");
            return NotFound("Given id is not match with any user.");


        }
        #endregion

        #region Get Users
        /// <summary>
        /// Returns All Users with inactive
        /// </summary>
        /// <response code="200"> Gets an user in the system</response>
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetUsersAsync();
            if (users != null)
                return Success("Users Found.", null, new { Users = users });

            _logger.LogInformation("Didn't found any user ");
            return NotFound("Didn't found any user.");

        }
        #endregion

        #region Get User By Id
        /// <summary>
        /// Returns user by id
        /// </summary>
        /// <response code="200"> Gets an user in the system with id</response>
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user != null)
                return Success("User Found.", null, user);

            _logger.LogInformation("Didn't found any user ");
            return NotFound("Didn't found any user.");
        }
        #endregion

        #region Delete User By Id
        /// <summary>
        /// Deletes user by id (actullay sets the User.isActive field to false)
        /// </summary>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteUserById(int id)
        {
            var isDeleted = await _userService.DeleteUserById(id);
            if (isDeleted)
                return Success("User deleted successfully.", null, isDeleted);

            _logger.LogInformation("Didn't found any user ");
            return NotFound("Didn't found any user with given.");
        }
        #endregion

        #region CreateUser
        /// <summary>
        /// Creates User
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Ok(user)</returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Message = "Invalid parameters.", Errors = ModelState.Values.SelectMany(e => e.Errors) }); ;

            var isCreated = await _userService.CreateUser(request);
            if (isCreated == false)
            {
                _logger.LogError("User couldn't create");
                return BadRequest("Could not register the user", null, string.Empty);
            }
            _logger.LogInformation($"User created successfully with mail = {request.Email}");
            return Success("Registration successfully done", null, new { isCreated = isCreated });
        }
        #endregion

        #region Confirm Email
        [Shared.Utilities.Authorize]
        [HttpPost]
        [Route("Confirmation")]
        public async Task<IActionResult> ConfirmMail()
        {
            var isConfirmed = await _userService.ConfirmEmail();
            if (isConfirmed)
            {
                _logger.LogInformation($"Users' mail confirmed");
                return Success("Confirmation successfully done.", null, isConfirmed);
            }
            _logger.LogError("Users' mail couldn't confirmed");
            return BadRequest("Mail confirmation not succeed.", null, string.Empty);

        }
        #endregion

        #region Login User
        /// <summary>
        /// Login existing user
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Login
        ///     {
        ///        "Email": "mail@email.com",
        ///        "Password": "P4asswordd"
        ///     }
        ///
        /// </remarks>
        /// <returns>Ok(user)</returns>
        /// <response code="200">Returns if login success with user info </response>
        /// <response code="400">If given mail or Password is not match any user's which in database</response> 
        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> LoginUserAsync(LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid Parameters");

            var user = await _userService.LoginUser(request);
            if (user == null)
            {
                _logger.LogInformation($"User not logged in {request.Email}");
                return BadRequest("Wrong mail or password. Please Check it", null, string.Empty);
            }
            _logger.LogInformation($"User logged in {request.Email}");
            return Success("Logged in", null, new { UserInfo = user });
        }

        #endregion

        #region Update
        /// <summary>
        /// Updates User
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Ok(user)</returns>
        [Shared.Utilities.Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromForm] UpdateUserRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Message = "Invalid Parameters", Errors = ModelState.Values.SelectMany(e => e.Errors) }); ;

            var isSuccess = await _userService.UpdateUser(request);
            if (isSuccess)
            {
                _logger.LogInformation($"User updated successfully with mail = {request.FirstName} + {request.LastName}");
                return Success("Updated successfully", null, new { isCreated = isSuccess });
            }
            _logger.LogInformation("User couldn't update");
            return BadRequest("Could not update user", null, string.Empty);

        }
        #endregion

        #region ResetPassword
        /// <summary>
        /// Resets User Password
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Password/Reset
        ///     {
        ///        "Email": "usermail@account.com",
        ///     }
        ///
        /// </remarks>
        /// <returns>Ok(true)</returns>
        /// <response code="200">Returns if password reset successful </response>
        /// <response code="400">Invalid or not matched passwords</response>
        [AllowAnonymous]
        [HttpPost]
        [Route("Password/Reset")]
        public async Task<IActionResult> ResetPasswordByEmail(ForgotPasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid Parameters");

            var result = await _userService.ResetUserPassword(request);
            if (result)
                return Success("Password resetted successfully, please check your mail to see your new temporary password.", null, result);

            _logger.LogInformation("Users' password couldn't reset");
            return BadRequest("Users' password couldn't reset", null, string.Empty);
        }
        #endregion

        #region ChangePassword
        /// <summary>
        /// Changes User Password
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /Password
        ///     {
        ///        "NewPassword": "ShouldBeSame0",
        ///        "ConfirmPassword": "ShouldBeSame0"
        ///     }
        ///
        /// </remarks>
        /// <returns>Ok(true)</returns>
        /// <response code="200">Returns if password reset successful </response>
        /// <response code="400">Invalid or not matched passwords</response> 
        [Shared.Utilities.Authorize]
        [HttpPut]
        [Route("Password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid Parameters");

            var isUpdated = await this._userService.ChangeUserPassword(request);
            if (isUpdated)
            {
                _logger.LogInformation("User changed the password successfully");
                return Success("Password updated successfully", null, isUpdated);
            }
            _logger.LogInformation("User didn't change the password");
            return BadRequest("Password could not update", null, string.Empty);
        }
        #endregion

    }
}
