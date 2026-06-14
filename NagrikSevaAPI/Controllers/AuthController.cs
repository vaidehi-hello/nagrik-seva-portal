using Microsoft.AspNetCore.Mvc;
using NagrikSevaAPI.Models;
using NagrikSevaAPI.Repositories.Interfaces;
using NagrikSevaAPI.Helpers;

namespace NagrikSevaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepo;
        private readonly JwtHelper _jwtHelper;
        private readonly EmailService _emailService; //  moved inside class
        private readonly IConfiguration _config;

        public AuthController(
            IAuthRepository authRepo,
            JwtHelper jwtHelper,
            EmailService emailService, //  added to constructor
            IConfiguration config)
        {
            _authRepo = authRepo;
            _jwtHelper = jwtHelper;
            _emailService = emailService; //  assigned here
            _config = config;
        }

        // POST: api/auth/register
        [HttpPost("register")]
public async Task<IActionResult> Register(
    RegisterRequest request)
{
    try
    {
        // Decrypt ALL fields
        string decryptedName = AesEncryptionHelper
            .Decrypt(request.Name);
        string decryptedEmail = AesEncryptionHelper
            .Decrypt(request.Email);
        string decryptedPassword = AesEncryptionHelper
            .Decrypt(request.Password);
        string decryptedPhone = AesEncryptionHelper
            .Decrypt(request.Phone);

        // Validate
        if (!ValidationHelper.IsValidEmail(decryptedEmail))
            return BadRequest(new {
                message = "Invalid email format!"
            });

        if (!ValidationHelper.IsValidPassword(decryptedPassword))
            return BadRequest(new {
                message = ValidationHelper
                    .GetPasswordError(decryptedPassword)
            });

        // Check duplicate
        var existing = await _authRepo
            .GetUserByEmailAsync(
                decryptedEmail.ToLower().Trim());

        if (existing != null)
            return BadRequest(new {
                message = "Email already registered!"
            });

        // Create token
        var verificationToken = Guid.NewGuid().ToString();

        // Create user
        var user = new User
        {
            Name = decryptedName,
            Email = decryptedEmail.ToLower().Trim(),
            Password = BCrypt.Net.BCrypt
                .HashPassword(decryptedPassword),
            Phone = decryptedPhone,
            IsEmailVerified = false,
            VerificationToken = verificationToken,
            VerificationTokenExpiry =
                DateTime.Now.AddHours(24)
        };

        var created = await _authRepo.RegisterAsync(user);

        // Assign role
        var roleId = request.Role switch
        {
            "Officer" => 2,
            "Admin" => 3,
            _ => 1
        };

        await _authRepo.AddUserRoleAsync(new UserRole
        {
            UserId = created.Id,
            RoleId = roleId
        });

        //  Try email separately - don't fail if email fails
        bool emailSent = false;
        string emailError = "";

        try
        {
            var baseUrl = _config["AppSettings:BaseUrl"];
            var verificationLink =
                $"{baseUrl}/api/auth/verify" +
                $"?token={verificationToken}";

            await _emailService
                .SendVerificationEmailAsync(
                    created.Email,
                    created.Name,
                    verificationLink
                );

            emailSent = true;
        }
        catch (Exception emailEx)
        {
            // ✅ Just log it, don't fail the registration
            emailError = emailEx.Message;
            Console.WriteLine(
                "Email failed: " + emailEx.Message);
        }

        // ✅ Return success regardless of email
        var responseMessage = emailSent
            ? "Registration successful! Please check your email to verify your account."
            : "Registration successful! However, verification email could not be sent. Please contact support or use register-plain for testing.";

        return Ok(new {
            data = AesEncryptionHelper.Encrypt(
                System.Text.Json.JsonSerializer
                    .Serialize(new {
                        message = responseMessage,
                        email = created.Email,
                        emailSent = emailSent
                    })
            )
        });
    }
    catch (Exception ex)
    {
        return BadRequest(new {
            message = "Registration failed: " + ex.Message
        });
    }
}
        // GET: api/auth/verify
        [HttpGet("verify")]
        public async Task<IActionResult> VerifyEmail(
            [FromQuery] string token)
        {
            var user = await _authRepo
                .GetUserByVerificationTokenAsync(token);

            if (user == null)
                return BadRequest(new {
                    message = "Invalid link!"
                });

            if (user.VerificationTokenExpiry < DateTime.Now)
                return BadRequest(new {
                    message = "Link expired!"
                });

            user.IsEmailVerified = true;
            user.VerificationToken = null;
            user.VerificationTokenExpiry = null;

            await _authRepo.UpdateUserAsync(user);
            return Redirect(
                "http://localhost:4200/login?verified=true");
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            try
            {
                // Decrypt email and password
                string decryptedEmail = AesEncryptionHelper
                    .Decrypt(request.Email);
                string decryptedPassword = AesEncryptionHelper
                    .Decrypt(request.Password);

                if (!ValidationHelper.IsValidEmail(decryptedEmail))
                    return BadRequest(new {
                        message = "Invalid email!"
                    });

                var user = await _authRepo
                    .GetUserByEmailAsync(
                        decryptedEmail.ToLower().Trim());

                if (user == null)
                    return Unauthorized(new {
                        message = "Invalid email or password!"
                    });

                if (!user.IsEmailVerified)
                    return Unauthorized(new {
                        message = "Please verify your email first!"
                    });

                bool isValid = BCrypt.Net.BCrypt
                    .Verify(decryptedPassword, user.Password);

                if (!isValid)
                    return Unauthorized(new {
                        message = "Invalid email or password!"
                    });

                // Get role
                string role = await _authRepo
                    .GetUserRoleAsync(user.Id);

                // Generate JWT token
                var token = _jwtHelper.GenerateToken(user, role);

                var responseData = new
                {
                    message = "Login successful!",
                    token = token,
                    id = user.Id,
                    name = user.Name,
                    email = user.Email,
                    role = role
                };

                return Ok(new {
                    data = AesEncryptionHelper.Encrypt(
                        System.Text.Json.JsonSerializer
                            .Serialize(responseData))
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new {
                    message = "Login failed! " + ex.Message
                });
            }
        }

        // POST: api/auth/login-plain (Testing only!)
        [HttpPost("login-plain")]
        public async Task<IActionResult> LoginPlain(
            LoginRequest request)
        {
            var user = await _authRepo
                .GetUserByEmailAsync(request.Email.ToLower());

            if (user == null)
                return Unauthorized(new {
                    message = "Invalid credentials!"
                });

            if (!user.IsEmailVerified)
                return Unauthorized(new {
                    message = "Please verify email first!"
                });

            bool isValid = BCrypt.Net.BCrypt
                .Verify(request.Password, user.Password);

            if (!isValid)
                return Unauthorized(new {
                    message = "Invalid credentials!"
                });

            string role = await _authRepo
                .GetUserRoleAsync(user.Id);
            var token = _jwtHelper.GenerateToken(user, role);

            return Ok(new {
                token,
                role,
                name = user.Name,
                email = user.Email,
                id = user.Id
            });
        }

        // POST: api/auth/register-plain (Testing only!)
        [HttpPost("register-plain")]
        public async Task<IActionResult> RegisterPlain(
            RegisterRequest request)
        {
            // Check duplicate
            var existing = await _authRepo
                .GetUserByEmailAsync(
                    request.Email.ToLower().Trim());

            if (existing != null)
                return BadRequest(new {
                    message = "Email already registered!"
                });

            var user = new User
            {
                Name = request.Name,
                Email = request.Email.ToLower().Trim(),
                Password = BCrypt.Net.BCrypt
                    .HashPassword(request.Password),
                Phone = request.Phone,
                IsEmailVerified = true, // Auto verified for testing
                VerificationToken = null,
                VerificationTokenExpiry = null
            };

            var created = await _authRepo.RegisterAsync(user);

            var roleId = request.Role switch
            {
                "Officer" => 2,
                "Admin" => 3,
                _ => 1
            };

            await _authRepo.AddUserRoleAsync(new UserRole
            {
                UserId = created.Id,
                RoleId = roleId
            });

            return Ok(new {
                message = "Registered successfully!",
                name = created.Name,
                email = created.Email,
                role = request.Role
            });
        }
    }
}