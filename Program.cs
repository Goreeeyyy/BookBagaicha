using BookBagaicha.Database;
using Microsoft.EntityFrameworkCore;
using BookBagaicha.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BookBagaicha.Services;
using BookBagaicha.IService;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Adding Controllers Class which identifies ControllerBase
builder.Services.AddControllers(); 


// Registering DB Context using Postgres Database with the connection strings of Postgres DB created in PgAdmin 4
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));


// Registering User manager which acts as a middleware between AppDbContext and Frontend
builder.Services.AddIdentity<User, IdentityRole<long>>()
    .AddEntityFrameworkStores<AppDbContext>();



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

builder.Services.AddScoped<IBookService, BookService>();


builder.Services.AddScoped<IWishlistService, WishlistService>();

builder.Services.AddScoped<ICartService, CartService>();


builder.Services.AddScoped<IImageService, ImageService>();


builder.Services.AddAuthorization();

builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost:7147", "http://localhost:5215", "https://localhost:44351" /* Replace with your actual backend URL */)
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

// Start building the app (Here, Start BookBagaicha)
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseCors();

app.UseStaticFiles();
app.UseRouting();


app.UseStaticFiles();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
