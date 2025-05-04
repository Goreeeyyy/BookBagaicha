using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Database;
using StudentManagement.Database.Entities;
using Microsoft.IdentityModel.Tokens;
using StudentManagement.Models;
using System.Text;
using StudentManagement.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers(); // Error fixed --> The Controller was not registered previously but now is. 

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection"))); 


builder.Services.AddIdentity<User, IdentityRole<long>>()
    .AddEntityFrameworkStores<AppDbContext>();
// Registering User manager which acts as a middleware between AppDbContext and Frontend



// getting jwttokeninfo object from appsettings
var jwtTokeInfo = builder.Configuration.GetSection("jwt").Get<JWTTokenInfo>();

// JWT Registration
builder.Services.AddAuthentication(

    (options) =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    }

    ).AddJwtBearer(
        (options) =>
        {
            options.TokenValidationParameters = new TokenValidationParameters()
            {

                ValidateIssuer = true,
                ValidIssuer = jwtTokeInfo!.Issuer,

                ValidateAudience = true,
                ValidAudience = jwtTokeInfo!.Audience,

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokeInfo!.Key))

            }; 
        }
    
    );


builder.Services.Configure<JWTTokenInfo>(builder.Configuration.GetSection("jwt"));

//Registering JWTService in DI container for dependency injection. 
builder.Services.AddScoped<JWTService>();

builder.Services.AddAuthorization();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
