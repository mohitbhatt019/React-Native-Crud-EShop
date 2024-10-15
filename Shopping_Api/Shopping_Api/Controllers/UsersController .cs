using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shopping_Api.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    //User manager for handling user-related operations.
    private readonly UserManager<IdentityUser> _userManager;

    //Role manager for handling role-related operations.
    private readonly RoleManager<IdentityRole> _roleManager;

    //Sign-in manager for handling user sign-in operations.
    private readonly SignInManager<IdentityUser> _signInManager;

    //Configuration for accessing JWT settings.
    private readonly IConfiguration _configuration;

    private readonly ILogger<UsersController> _logger;

    public UsersController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration, ILogger<UsersController> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _logger = logger;
    }



    // Registers a new user and assigns them a role.
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        _logger.LogInformation("Registering a new user with username: {Username}", model.Username);

        // Ensure the role exists
        if (!await _roleManager.RoleExistsAsync(model.Role))
        {
            _logger.LogInformation("Role {Role} does not exist, creating it.", model.Role);
            await _roleManager.CreateAsync(new IdentityRole(model.Role));
        }

        var user = new IdentityUser
        {
            UserName = model.Username,
            Email = model.Email
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            _logger.LogInformation("User {Username} registered successfully, assigning role {Role}.", model.Username, model.Role);
            await _userManager.AddToRoleAsync(user, model.Role);
            return Ok("User registered successfully");
        }

        _logger.LogWarning("User registration failed for {Username}. Errors: {Errors}", model.Username, string.Join(", ", result.Errors.Select(e => e.Description)));
        return BadRequest(result.Errors);
    }


    // Authenticates a user and returns a JWT token if the login is successful.
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        _logger.LogInformation("Attempting to log in user with username: {Username}", model.Username);

        var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, isPersistent: false, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            _logger.LogInformation("User {Username} logged in successfully.", model.Username);

            var user = await _userManager.FindByNameAsync(model.Username);
            var roles = await _userManager.GetRolesAsync(user);

            _logger.LogInformation("Generating JWT token for user {Username} with roles: {Roles}", model.Username, string.Join(", ", roles));
            var token = GenerateJwtToken(user, roles);

            return Ok(new { Token = token, Roles = roles });
        }

        _logger.LogWarning("Login failed for user {Username}.", model.Username);
        return Unauthorized();
    }


    // Generates a JWT token for the authenticated user with their roles.
    private string GenerateJwtToken(IdentityUser user, IList<string> roles)
    {
        _logger.LogInformation("Generating JWT token for user {Username}.", user.UserName);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
        {
            _logger.LogInformation("Adding role {Role} to JWT token claims for user {Username}.", role, user.UserName);
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var keyString = _configuration["JWT:SecretKey"];
        var issuer = _configuration["JWT:Issuer"];
        var audience = _configuration["JWT:Audience"];

        if (string.IsNullOrEmpty(keyString) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
        {
            _logger.LogError("JWT configuration values are missing for user {Username}.", user.UserName);
            throw new InvalidOperationException("JWT configuration values are missing.");
        }

        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(keyString));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        _logger.LogInformation("JWT token generated successfully for user {Username}.", user.UserName);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}



