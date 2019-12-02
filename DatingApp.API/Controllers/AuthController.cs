using Microsoft.AspNetCore.Mvc;
using DatingApp.API.Data;
using System.Threading.Tasks;
using DatingApp.API.Models;
using DatingApp.API.Dtos;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Configuration;
using System;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController:ControllerBase
    {
        public readonly IAuthRepository _repo;
        public readonly IConfiguration _config;
        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _repo=repo;  
            _config=config;          
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            userForRegisterDto.Username=userForRegisterDto.Username.ToLower();
            if(await _repo.UserExists(userForRegisterDto.Username))
              return  BadRequest("Username Exists");
            var userToCreate= new User
            {
                Username=userForRegisterDto.Username,

            };
            var createdUser= await _repo.Register(userToCreate,userForRegisterDto.Password);

            return StatusCode(201);
        }


        [HttpPost("login")]

        public async Task<IActionResult> Longin(UserForLoginDto userForLoginDto)
        {
            var userFormRepo= await _repo.Login(userForLoginDto.Username.ToLower(),userForLoginDto.Password);
            if(userFormRepo==null)
            {
                return Unauthorized();
            }
            var claim = new []
            {
                new Claim(ClaimTypes.NameIdentifier,userFormRepo.Id.ToString()),
                new Claim(ClaimTypes.Name,userFormRepo.Username),

            };
            var key= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds =new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);

            var tokenDescripter= new SecurityTokenDescriptor
            {
                Subject= new ClaimsIdentity(claim),
                Expires= DateTime.Now.AddDays(1),
                SigningCredentials=creds
            };


            var tokenHandler= new JwtSecurityTokenHandler();
            var token= tokenHandler.CreateToken(tokenDescripter);
            return Ok(new{
                token=tokenHandler.WriteToken(token)
            });
        }
    }

    
}