using BookBagaicha.Models;
using BookBagaicha.Models.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BookBagaicha.Services;

namespace BookBagaicha.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        private readonly SignInManager<User> _signInManager;

        private readonly RoleManager<IdentityRole<long>> _roleManager;

        private readonly JWTService _jwtService;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, JWTService jwtService, RoleManager<IdentityRole<long>> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _roleManager = roleManager;


        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(UserRegisterDto userdto)
        {
            User user = new User
            {
                UserName = userdto.Email!.Split("@")[0],
                FirstName = userdto.FirstName!,
                LastName = userdto.LastName!,
                Email = userdto.Email,
                Address = userdto.Address,
            };

            var result = await _userManager.CreateAsync(user, userdto.Password!);

            if (result.Succeeded)
            {
                string roleToAssign = string.IsNullOrEmpty(userdto.Role) ? "User" : userdto.Role;

                var roleExists = await _roleManager.RoleExistsAsync(roleToAssign);
                if (!roleExists)
                {
                    return BadRequest(new { Error = $"Role '{roleToAssign}' does not exist." });
                }

                var roleResult = await _userManager.AddToRoleAsync(user, roleToAssign);

                if (!roleResult.Succeeded)
                {
                    return BadRequest(roleResult.Errors);
                }

                return Ok(new { Message = "Successful Registration with assigned roles", Role = roleToAssign });
            }


            return BadRequest(result.Errors);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto userdto)
        {
            User? user = await _userManager.FindByEmailAsync(userdto.Email!);

            if (user == null) return Unauthorized("Email has not been Registered");

            var result = await _signInManager.CheckPasswordSignInAsync(user, userdto.Password!, true);

            if (result.Succeeded)
            {
                // Get the user's roles
                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault(); // Assuming a user has one primary role

                return Ok(
                    new
                    {
                        Message = "Login Success",
                        Token = _jwtService.GenerateToken(user), // Pass user to generate token with claims
                        Role = role // Include the user's role in the response
                    }
                );
            }

            return Unauthorized("Password is not valid");
        }


    }
}
