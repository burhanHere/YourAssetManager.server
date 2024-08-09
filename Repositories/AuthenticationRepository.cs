using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using YourAssetManager.server.Services;
using YourAssetManager.Server.DTOs;
using YourAssetManager.Server.Helpers;
using YourAssetManager.Server.Models;

namespace YourAssetManager.Server.Repositories
{
    /// <summary>
    /// Repository for handling authentication-related tasks.
    /// </summary>
    public class AuthenticationRepository(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, MailSettingsDTO mailSettings, IConfiguration configuration)
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationRepository"/> class.
        /// </summary>
        /// <param name="userManager">The user manager for handling user-related tasks.</param>
        /// <param name="roleManager">The role manager for handling role-related tasks.</param>
        /// <param name="appSettings">Application settings for configuring services.</param>
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly EmailService _emailService = new(mailSettings);
        private readonly IConfiguration _configuration = configuration;

        /// <summary>
        /// Signs up a new user with the specified signup model.
        /// </summary>
        /// <param name="signUpModel">The signup model containing user information.</param>
        /// <returns>An <see cref="ApiResponseDTO"/> indicating the status of the signup operation.</returns>
        public async Task<ApiResponseDTO> SignUp(SignUpDTO signUpModel)
        {
            // Check if a user already exists with this email
            var checkUser = await _userManager.FindByEmailAsync(signUpModel.Email!);
            if (checkUser != null)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status403Forbidden,
                    ResponseData = new List<string>
                    {
                        "An account is already associated with this email.",
                        "Please use a different email."
                    }
                };
            }

            // Create a new user
            ApplicationUser newUser = new()
            {
                Email = signUpModel.Email,
                UserName = signUpModel.UserName,
                SecurityStamp = Guid.NewGuid().ToString(),
            };
            var createUser = await _userManager.CreateAsync(newUser, signUpModel.Password!);

            if (!createUser.Succeeded)
            {
                // User creation failed
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponseData = new List<string>
                    {
                        "Unable to register new user.",
                        "Please try again later."
                    }
                };
            }

            // Generating confirmation token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);

            // Sending confirmation link for email confirmation
            // string message = $"Please click the link below to confirm your email address.\nConfirmation Link: <a href ={HelperFunctions.TokenLinkCreated("http://localhost:5235", "ConfirmEmail", token, newUser.Email!)}>Click</a>";
            string message = $"Please click the link below to confirm your email address.\nConfirmation Link: <a href ={HelperFunctions.TokenLinkCreated("http://localhost:4200/auth", "EmailConfirmation", token, newUser.Email!)}>Click</a>";
            string subject = "Confirmation E-Mail (No Reply)";
            bool emailStatus = await _emailService.SendEmailAsync(newUser.Email!, subject, message);

            if (emailStatus)
            {
                // assigning OrganizationOwner role to the registerer
                var roleName = "OrganizationOwner";
                var organizationOwnerRole = await _roleManager.FindByNameAsync(roleName);
                if (organizationOwnerRole == null)
                {
                    return new ApiResponseDTO
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        ResponseData = new List<string>
                        {
                            $"Role '{roleName}' does not exist. Please contact support."
                        }
                    };
                }
                var addToRoleResult = await _userManager.AddToRoleAsync(newUser, organizationOwnerRole.Name!);
                if (!addToRoleResult.Succeeded)
                {
                    return new ApiResponseDTO
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        ResponseData = new List<string>
                        {
                            "Account created successfully.",
                            $"Reset password link has been sent to your email: {newUser.Email[..3]}...{newUser.Email[newUser.Email.IndexOf("@")..]}",
                            "Confirmation email sent. Please check your email.",
                            "Failed to assign role to the user.",
                            "Please try again later."
                        }
                    };
                }

                // Account created, Email sent and role assigned successfully 
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status200OK,
                    ResponseData = new List<string>
                    {
                        "Account created successfully.",
                        "Confirmation email sent.",
                        "Role assigned as OrganizationOwner Role"
                    }
                };
            }

            // Account created, but email sending failed
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = new List<string>
                {
                    "Account created successfully.",
                    "Unable to send confirmation email. Please try to log in later to confirm your email."
                }
            };
        }

        /// <summary>
        /// Confirms the email address of the specified user using the provided token.
        /// This method checks if the user exists, if the email has already been confirmed,
        /// and then attempts to confirm the email using the provided token.
        /// </summary>
        /// <param name="token">The confirmation token generated during user registration.</param>
        /// <param name="email">The email address of the user whose email is being confirmed.</param>
        /// <returns>An <see cref="ApiResponseDTO"/> indicating the status of the email confirmation operation.</returns>
        public async Task<ApiResponseDTO> ConfirmEmail(string token, string email)
        {
            // Find the user by email
            // var uriDecodedEmail = WebUtility.UrlDecode(email);
            // var user = await _userManager.FindByEmailAsync(uriDecodedEmail);
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // User not found
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "Unable to find user.",
                        "Email not confirmed."
                    }
                };
            }
            if (user.EmailConfirmed)
            {
                // Email already confirmed
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status208AlreadyReported,
                    ResponseData = new List<string>
                    {
                        "Email is already confirmed."
                    }
                };
            }

            // Confirm email with the token
            // var uriDecodedToken = WebUtility.UrlDecode(token);
            // var result = await _userManager.ConfirmEmailAsync(user, uriDecodedToken);
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                // Email confirmation succeeded
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status200OK,
                    ResponseData = new List<string>
                    {
                        "Congratulations! Your email has been confirmed."
                    }
                };
            }

            return new ApiResponseDTO
            {
                Status = StatusCodes.Status400BadRequest,
                ResponseData = new List<string>
                {
                    "Email confirmation failed. Please try again."
                }
            };
        }

        /// <summary>
        /// Signs in a user with the specified sign-in model.
        /// This method checks if the user exists, if the email has been confirmed, 
        /// validates the password, and generates a JWT token for the authenticated user.
        /// </summary>
        /// <param name="signInModel">The sign-in model containing user credentials.</param>
        /// <param name="urlHelper">The URL helper for generating confirmation links.</param>
        /// <param name="httpContext">The HTTP context for accessing request-related information.</param>
        /// <returns>An <see cref="ApiResponseDTO"/> indicating the status of the sign-in operation.</returns>
        public async Task<ApiResponseDTO> SignIn(SignInDTO signInModel)
        {
            var user = await _userManager.FindByEmailAsync(signInModel.Email!);
            if (user == null)
            {
                // User not found
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "User not found.",
                        "Please check your email address."
                    }
                };
            }

            // Check if the user's email is confirmed
            if (!user.EmailConfirmed)
            {

                // Generating confirmation token
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                // Sending confirmation link for email confirmation
                // string message = $"Please click the link below to confirm your email address.\nConfirmation Link: <a href ={HelperFunctions.TokenLinkCreated("http://localhost:5235", "ConfirmEmail", token, user.Email!)}>Click</a>";
                string message = $"Please click the link below to confirm your email address.\nConfirmation Link: <a href ={HelperFunctions.TokenLinkCreated("http://localhost:4200/auth", "EmailConfirmation", token, user.Email!)}>Click</a>";
                string subject = "Confirmation E-Mail (No Reply)";
                bool emailStatus = await _emailService.SendEmailAsync(user.Email!, subject, message);

                if (!emailStatus)
                {
                    // Failed to send confirmation email
                    return new ApiResponseDTO
                    {
                        Status = StatusCodes.Status400BadRequest,
                        ResponseData = new List<string>
                        {
                            "Email not confirmed.",
                            "Unable to send confirmation email. Please try again later."
                        }
                    };
                }
                // Email not confirmed, confirmation email sent
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status401Unauthorized,
                    ResponseData = new List<string>
                    {
                        $"Email Confirmation link has been sent to your email: {user.Email[..3]}...{user.Email[user.Email.IndexOf("@")..]}",
                        "A confirmation email has been sent. Please check your inbox."
                    }
                };
            }

            // Check the user's password
            var passwordCheck = await _userManager.CheckPasswordAsync(user, signInModel.Password!);
            if (!passwordCheck)
            {
                // Incorrect password
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status403Forbidden,
                    ResponseData = new List<string>
                    {
                        "Incorrect password.",
                        "Please check your password and try again."
                    }
                };
            }

            // Create JWT token
            var authClaim = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Name, user.UserName!),
                new(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
            };
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                authClaim.Add(new(ClaimTypes.Role, userRole));
            }

            var secret = _configuration.GetValue<string>("JWT:Secret");
            var issuer = _configuration.GetValue<string>("JWT:ValidIssuer");
            var audience = _configuration.GetValue<string>("JWT:ValidAudience");
            var authSiginKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret!));
            var newJwtToken = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                expires: DateTime.Now.AddHours(12),
                claims: authClaim,
                signingCredentials: new SigningCredentials(authSiginKey, SecurityAlgorithms.HmacSha256)
            );
            var finalJwtToken = new JwtSecurityTokenHandler().WriteToken(newJwtToken);

            // Successful sign-in, return JWT token
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = new
                {
                    message = "Sign-in successful.",
                    jwtToken = finalJwtToken
                }
            };
        }

        public async Task<ApiResponseDTO> ForgetPassword(ForgetPasswordRequestDTO forgetPasswordRequest)
        {
            var user = await _userManager.FindByEmailAsync(forgetPasswordRequest.Email);
            if (user == null)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "User Not Found.",
                        "Check your Email.",
                    }
                };
            }
            // creating forget password token
            var forgetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Sending confirmation link for email confirmation
            // string message = $"Please click the link below to reset your password.\nPassowrd Reset Link: <a href ={HelperFunctions.TokenLinkCreated("http://localhost:5235", "ResetPassword", forgetPasswordToken, user.Email!)}>Click</a>";
            string message = $"Please click the link below to reset your password.\nPassowrd Reset Link: <a href ={HelperFunctions.TokenLinkCreated("http://localhost:4200/auth", "resetpassword", forgetPasswordToken, user.Email!)}>Click</a>";
            string subject = "Reset password E-Mail (No Reply)";
            bool emailStatus = await _emailService.SendEmailAsync(user.Email!, subject, message);

            if (!emailStatus)
            {
                // Failed to send confirmation email
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponseData = new List<string>
                        {
                            "Unable to send Reset password email. Please try again later."
                        }
                };
            }
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = new List<string>
                    {
                        $"Reset password link has been sent to your email: {user.Email[..3]}...{user.Email[user.Email.IndexOf("@")..]}",
                        "Please check your inbox."
                    }
            };
        }

        public async Task<ApiResponseDTO> ResetPassword(ResetPasswordDTO resetPassowrdModel)
        {
            var user = await _userManager.FindByEmailAsync(resetPassowrdModel.Email!);
            if (user == null)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status404NotFound,
                    ResponseData = new List<string>
                    {
                        "User Not Found.",
                        "Invalid Password reset request."
                    }
                };
            }
            var passwordResetResult = await _userManager.ResetPasswordAsync(user, resetPassowrdModel.Token!, resetPassowrdModel.NewPassword!);
            if (!passwordResetResult.Succeeded)
            {
                return new ApiResponseDTO
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponseData = new List<string>
                    {
                        "Password reset request failed.",
                        "Try Again latter."
                    },
                    Errors = passwordResetResult.Errors
                };
            }
            return new ApiResponseDTO
            {
                Status = StatusCodes.Status200OK,
                ResponseData = new List<string>
                {
                    "Password reset Successsfull."
                }
            };
        }
    }
}