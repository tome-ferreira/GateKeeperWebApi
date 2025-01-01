using GateKeeperWebApiV1.Data;
using GateKeeperWebApiV1.Models;
using GateKeeperWebApiV1.Objects;
using GateKeeperWebApiV1.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace GateKeeperWebApiV1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowedDomainsPolicy")]
    public class AccountController : _BaseApiController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext dbContext;

        public AccountController(UserManager<ApplicationUser> userManager, IConfiguration configuration, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _configuration = configuration;
            this.dbContext = dbContext;
        }

        /// <summary>
        /// Login user and return JWT token.
        /// </summary>
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    // Generate JWT token upon successful login
                    var token = await GenerateJwtToken(user);
                    var roles = await _userManager.GetRolesAsync(user); // Get user's roles



                    return Ok(new { token, roles }); // Return token and roles
                }

                return Unauthorized("Invalid login attempt");
            }

            return BadRequest(ModelState);
        }

        private async Task<bool> CheckPasswordAsync(Company company, string password) 
        {
            // Convert the stored salt and hashed password from base64 strings
            byte[] storedSalt = Convert.FromBase64String(company.Salt);
            byte[] storedHashedPassword = Convert.FromBase64String(company.Password);

            // Hash the entered password with the stored salt
            byte[] enteredPasswordHash = HashPassword(password, storedSalt);

            if (enteredPasswordHash.SequenceEqual(storedHashedPassword))
            {
                return true;
            }
            return false;
        }
        private byte[] HashPassword(string password, byte[] salt)
        {
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(password).Concat(salt).ToArray());
            }
        }



        /// <summary>
        /// Generate JWT Token for authenticated companies.
        /// </summary>
        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            // Retrieve JWT settings from configuration
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings.GetValue<string>("SecretKey");
            var issuer = jwtSettings.GetValue<string>("Issuer");
            var audience = jwtSettings.GetValue<string>("Audience");
            var expiryInMinutes = jwtSettings.GetValue<int>("ExpiryInMinutes");

            // Initialize a List of claims, not an array
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            // Add roles to the claims
            var roles = await _userManager.GetRolesAsync(user); // Await the Task to get roles

            // Add each role as a claim
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Create the signing key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Generate the token
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(expiryInMinutes),
                signingCredentials: creds
            );

            // Return the JWT token as a string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("validate-token")]
        [Authorize] // Requer que o token seja enviado no cabeçalho
        public IActionResult ValidateToken()
        {
            return Ok(new { valid = true });
        }

    }
}
