using Application.DTOs.AuthenticationDTOs;
using Application.DTOs.EmailSettingsDTOs;
using Application.Interfaces;
using Application.IServices.IAuthenticationServices;
using Application.IServices.IEmailServices;
using Application.IServices.ITodoManagementServices;
using Domain.Entities.ApplicationEntities;
using Infrastructure.Data;
using Infrastructure.Services.AuthenticationServices;
using Infrastructure.Services.EmailServices;
using Infrastructure.Services.TodoManagementServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Connection")));

        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultProvider;
        })
          .AddEntityFrameworkStores<ApplicationDbContext>()
          .AddDefaultTokenProviders();


        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IEmailService,EmailService>();
        services.AddScoped<ITodoManagementService, TodoManagementService>();
        services.AddScoped<IAuthService, AuthService>();

        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
           .AddJwtBearer(options =>
           {
               var key = Encoding.UTF8.GetBytes(configuration["JwtSettings:Secret"]);

               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = true,
                   ValidateAudience = true,
                   ValidateLifetime = true,
                   ValidateIssuerSigningKey = true,
                   ValidIssuer = configuration["JwtSettings:Issuer"],
                   ValidAudience = configuration["JwtSettings:Audience"],
                   IssuerSigningKey = new SymmetricSecurityKey(key)
               };
           });
        return services;
    }
}
