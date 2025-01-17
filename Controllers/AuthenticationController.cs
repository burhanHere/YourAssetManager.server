using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using YourAssetManager.Server.Data;
using YourAssetManager.Server.DTOs;
using YourAssetManager.Server.Models;
using YourAssetManager.Server.Repositories;

namespace YourAssetManager.Server.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("YourAssetManager.Server/[controller]")]
    public class AuthenticationController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, MailSettingsDTO mailSettings, IConfiguration configuration, ApplicationDbContext applicationDbContext) : ControllerBase
    {
        // Initialize the authentication repository using the provided services
        private readonly AuthenticationRepository _authenticationRepository = new(userManager, roleManager, mailSettings, configuration, applicationDbContext);

        // Define a simple endpoint to check if the API is alive
        [HttpGet("IsAlive")]
        public IActionResult Get()
        {
            // Return a simple OK response indicating the API is alive
            return Ok("Hello, Yes I am alive...");
        }

        // Define the SignUp endpoint to handle user registration
        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] SignUpDTO signUpDTO)
        {
            ApiResponseDTO result;
            if (signUpDTO.requiredRole == "ORGANIZATIONOWNER")
            {
                // Call the SignUpAsOrganizationOwner method from the authentication repository
                result = await _authenticationRepository.SignUpAsOrganizationOwner(signUpDTO);
            }
            else
            {
                // Call the SignUpAsEmployee method from the authentication repository
                result = await _authenticationRepository.SignUpAsEmployee(signUpDTO);
            }
            // Check the status of the result and return the appropriate response
            if (result.Status == StatusCodes.Status200OK)
            {
                // Return an OK response with the result
                return Ok(result);
            }
            else if (result.Status == StatusCodes.Status403Forbidden)
            {
                // Return a response with status 403 Forbidden and the result as the body
                return StatusCode(StatusCodes.Status403Forbidden, result);
            }
            else if (result.Status == StatusCodes.Status404NotFound)
            {
                return NotFound(result);
            }

            // Return a BadRequest response with the result for any other status
            return BadRequest(result);
        }

        // Define the ConfirmEmail endpoint to handle email confirmation
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            // Call the ConfirmEmail method from the authentication repository
            ApiResponseDTO result = await _authenticationRepository.ConfirmEmail(token, email);

            // Check the status of the result and return the appropriate response
            if (result.Status == StatusCodes.Status200OK)
            {
                // Return an OK response with the result
                return Ok(result);
            }
            else if (result.Status == StatusCodes.Status208AlreadyReported)
            {
                // Return a response with status 208 Already Reported and the result as the body
                return StatusCode(StatusCodes.Status208AlreadyReported, result);
            }
            else if (result.Status == StatusCodes.Status404NotFound)
            {
                // Return a NotFound response with the result
                return NotFound(result);
            }

            // Return a BadRequest response with the result for any other status
            return BadRequest(result);
        }

        // Define the SignIn endpoint to handle user SignIn
        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn([FromBody] SignInDTO signInModel)
        {
            // Call the SignIn method from the authentication repository
            ApiResponseDTO result = await _authenticationRepository.SignIn(signInModel);

            // Check the status of the result and return the appropriate response
            if (result.Status == StatusCodes.Status200OK)
            {
                // Return an OK response with the result
                return Ok(result);
            }
            else if (result.Status == StatusCodes.Status404NotFound)
            {
                // Return a NotFound response with the result
                return NotFound(result);
            }
            else if (result.Status == StatusCodes.Status403Forbidden)
            {
                // Return a ForbidResult response with the result
                return StatusCode(StatusCodes.Status403Forbidden, result);
            }
            else if (result.Status == StatusCodes.Status401Unauthorized)
            {
                // Return a Unauthorized response with the result
                return Unauthorized(result);
            }
            // Return a BadRequest response with the result for any other status
            return BadRequest(result);
        }

        // Define the ForgetPassword endpoint to handle passwords reset after forgetting
        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequestDTO forgetPasswordRequest)
        {
            ApiResponseDTO result = await _authenticationRepository.ForgetPassword(forgetPasswordRequest);
            if (result.Status == StatusCodes.Status200OK)
            {
                return Ok(result);
            }
            else if (result.Status == StatusCodes.Status404NotFound)
            {
                return NotFound(result);
            }
            return BadRequest(result);
        }

        
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPassowrdModel)
        {
            var result = await _authenticationRepository.ResetPassword(resetPassowrdModel);
            if (result.Status == StatusCodes.Status200OK)
            {
                return Ok(result);
            }
            else if (result.Status == StatusCodes.Status404NotFound)
            {
                return NotFound(result);
            }
            return BadRequest(result);
        }
    }
}

