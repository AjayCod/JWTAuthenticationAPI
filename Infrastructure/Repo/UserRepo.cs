using Application.Contracts;
using Application.DTOs;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repo
{
    public class UserRepo : IUser
    {
        private readonly AppDBContext appDBContext;
        private readonly IConfiguration configuration;


        public UserRepo(AppDBContext _appDBContext,IConfiguration _configuration)
        {
            this.appDBContext = _appDBContext;
            this.configuration = _configuration;
        }
        public async Task<LoginResponse> LoginUserAsync(LoginDTO loginDTO)
        {
            var getUser = await FindUserByEmail(loginDTO.Email!);
            if (getUser == null) return new LoginResponse(false,"User Not Found");
            bool checkPassword = BCrypt.Net.BCrypt.Verify(loginDTO.Password, getUser.Password);
            if (checkPassword)
                return new LoginResponse(true, "Login Succesfully", GenrateJWTToken(getUser));
            else
                return new LoginResponse(false, "Invalid Credintail");
        }

        private string GenrateJWTToken(ApplicationUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
            var credintail = new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256);
            var userClams = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Name,user.Name!),
                new Claim(ClaimTypes.Email,user.Email!)
            };
            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims:userClams,
                expires:DateTime.Now.AddDays(5),
                signingCredentials:credintail

            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<ApplicationUser> FindUserByEmail(string email)=>
            await appDBContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        public async Task<RegistrationResponce> RegistrationUserAsync(RigesterUserDTO rigesterUserDTO)
        {
            var getUser = await FindUserByEmail(rigesterUserDTO.Email!);
            if (getUser != null)
                return new RegistrationResponce(false, "User Already Exist");
            appDBContext.Users.Add(new ApplicationUser()
            {
                Name = rigesterUserDTO.Name,
                Email= rigesterUserDTO.Email,
                Password=BCrypt.Net.BCrypt.HashPassword(rigesterUserDTO.Password)
            });
            await appDBContext.SaveChangesAsync();
            return new RegistrationResponce(true,"Registration completed");
        }
    }
}
