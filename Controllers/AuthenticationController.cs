using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using YourAssetManager.Server.DTOs;
using YourAssetManager.Server.Repositories;

namespace YourAssetManager.Server
{
    [AllowAnonymous]
    [ApiController]
    [Route("YourAssetManager.Server/{controller}")]
    public class AuthenticationController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, MailSettings mailSettings, IUrlHelperFactory urlHelperFactory, IConfiguration configuration) : ControllerBase
    {
        // Initialize the authentication repository using the provided services
        private readonly AuthenticationRepository _authenticationRepository = new(userManager, roleManager, mailSettings, configuration);

        // Define a simple endpoint to check if the API is alive
        [HttpGet("/IsAlive")]
        public IActionResult Get()
        {
            // Return a simple OK response indicating the API is alive
            return Ok("Hello, Yes I am alive...");
        }

        // Define the SignUp endpoint to handle user registration
        [HttpPost("/SignUp")]
        public async Task<IActionResult> SignUp([FromBody] SignUpModel signUpModel)
        {
            var _urlHelper = urlHelperFactory.GetUrlHelper(ControllerContext);
            // Call the SignUp method from the authentication repository
            ApiResponce result = await _authenticationRepository.SignUp(signUpModel, _urlHelper, HttpContext);
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

            // Return a BadRequest response with the result for any other status
            return BadRequest(result);
        }

        // Define the ConfirmEmail endpoint to handle email confirmation
        [HttpGet("/ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            // Call the ConfirmEmail method from the authentication repository
            ApiResponce result = await _authenticationRepository.ConfirmEmail(token, email);

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
        [HttpPost("/SignIn")]
        public async Task<IActionResult> SignIn(SignInModel signInModel)
        {
            var _urlHelper = urlHelperFactory.GetUrlHelper(ControllerContext);
            // Call the SignIn method from the authentication repository
            ApiResponce result = await _authenticationRepository.SignIn(signInModel, _urlHelper, HttpContext);

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

            // Return a BadRequest response with the result for any other status
            return BadRequest(result);
        }

        // Define the ForgetPassword endpoint to handle passwords reset after forgetting
        [HttpPost("/ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            var _urlHelper = urlHelperFactory.GetUrlHelper(ControllerContext);
            ApiResponce result = await _authenticationRepository.ForgetPassword(email, _urlHelper, HttpContext);
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

        // Define the ResetPassword Get endpoint to handle passwords reset
        [HttpGet("/ResetPassword")]
        public IActionResult ResetPassword(string token, string email)
        {
            ResetPassowrdModel resetPassowrdModel = new()
            {
                Token = token,
                Email = email
            };
            return Ok(resetPassowrdModel);
        }
        // Define the ResetPassword Post endpoint to handle passwords reset
        [HttpPost("/ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPassowrdModel resetPassowrdModel)
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


// {
//   "email": "burhanburewala@gmail.com",
//   "userName": "Burhan123@",
//   "password": "Burhan123@"
// }

// {
//     "email": "burhanburewala@gmail.com",
//   "password": "Burhan123@"
// }

// {
//   "newPassword": "Burewala123@",
//   "confirmedNewPassword": "Burewala123@",
//   "email": "burhanburewala@gmail.com",
//   "token": "CfDJ8CZUNWhaE/BDlDSBRIF6gsfgqNVsKMzTBr0e5fu9Bh5y89bNphYXbrLmVKEAJQ1zZVx/bB9zxeZZGVR1HR6jqBFeg3jVsnGOPjJasxak2TaYh0vYAwRfJEwMFnuY8cf9FHOVm3dFXj5+s2I2lVZJDAZLaL1A2sWZk/zL6ZflHlngrdlRG7dC+57fFEBR7oe1SmrZT2834YwHxaQJ7aDvWq/GaWNo5rsk4WLKbHSFM0Ck"
// }
