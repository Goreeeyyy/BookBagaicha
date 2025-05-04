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

        private readonly JWTService _jwtService;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, JWTService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
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
                var roleResult = await _userManager.AddToRoleAsync(user, "User");

                if (!roleResult.Succeeded)
                {
                    return BadRequest(roleResult.Errors);
                }

                return Ok("Successful Registration with assigned roles");
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
                return Ok(

                    new
                    {
                        Message = "Login Success",
                        Token = _jwtService.GenerateToken()
                    }

                    );
            }

            return Unauthorized("Password is not valid");
        }


    }
}
