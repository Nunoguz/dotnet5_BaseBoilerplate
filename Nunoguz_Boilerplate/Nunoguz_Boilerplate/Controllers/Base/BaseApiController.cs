using Nunoguz_Boilerplate.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nunoguz_Boilerplate.Controllers.Base
{
    //[Authorize]
    [Route("[controller]")] // api/
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        [NonAction]
        protected IActionResult Success<T>(string message, string internalMessage, T data)
        {
            return Success(new ApiResponse<T>
            {
                Success = true,
                Message = message,
                InternalMessage = internalMessage,
                Data = data
            });
        }

        [NonAction]
        protected IActionResult Success<T>(ApiResponse<T> data)
        {
            return Ok(data);
        }

        [NonAction]
        protected IActionResult Created<T>(string message, string internalMessage, T data)
        {
            return Success(new ApiResponse<T>
            {
                Success = true,
                Message = message,
                InternalMessage = internalMessage,
                Data = data
            });
        }
        [NonAction]
        protected IActionResult Created<T>(ApiResponse<T> data)
        {
            return StatusCode(201, data);
        }

        [NonAction]
        protected IActionResult NoContent<T>(string message, string internalMessage, T data)
        {
            return NoContent<T>(new ApiResponse<T>
            {
                Success = true,
                Message = message,
                InternalMessage = internalMessage,
                Data = data
            });
        }

        [NonAction]
        private IActionResult NoContent<T>(ApiResponse<T> data)
        {
            return StatusCode(204, data);
        }

        protected IActionResult BadRequest<T>(string message, string internalMessage, T data)
        {
            return BadRequest<T>(new ApiResponse<T>
            {
                Success = true,
                Message = message,
                InternalMessage = internalMessage,
                Data = data
            });
        }

        [NonAction]
        protected IActionResult BadRequest<T>(ApiResponse<T> data)
        {
            return StatusCode(400, data);
        }

        [NonAction]
        protected IActionResult Unauthorized<T>(string message, string internalMessage, T data)
        {
            return Unauthorized(new ApiResponse<T>
            {
                Success = true,
                Message = message,
                InternalMessage = internalMessage,
                Data = data
            });
        }

        [NonAction]
        protected IActionResult Unauthorized<T>(ApiResponse<T> data)
        {
            return StatusCode(401, data);
        }

        [NonAction]
        protected IActionResult Forbidden<T>(string message, string internalMessage, T data)
        {
            return Forbidden(new ApiResponse<T>
            {
                Success = true,
                Message = message,
                InternalMessage = internalMessage,
                Data = data
            });
        }

        [NonAction]
        protected IActionResult Forbidden<T>(ApiResponse<T> data)
        {
            return StatusCode(403, data);
        }

        [NonAction]
        protected IActionResult NotFound<T>(string message, string internalMessage, T data)
        {
            return NotFound(new ApiResponse<T>
            {
                Success = true,
                Message = message,
                InternalMessage = internalMessage,
                Data = data
            });
        }

        [NonAction]
        protected IActionResult NotFound<T>(ApiResponse<T> data)
        {
            return StatusCode(404, data);
        }

        [NonAction]
        protected IActionResult Error<T>(string message, string internalMessage, T data)
        {
            return Error(new ApiResponse<T>
            {
                Success = true,
                Message = message,
                InternalMessage = internalMessage,
                Data = data
            });
        }

        [NonAction]
        protected IActionResult Error<T>(ApiResponse<T> data)
        {
            return StatusCode(500, data);
        }
    }
}
