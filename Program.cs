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
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost:7147", "http://localhost:5215", "http://localhost:8080", "https://localhost:44351", "http://localhost:5500")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Configure logging
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Information); // Set to LogLevel.Debug for more details
});

// Add services to the container
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register DB Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

// Register Identity
builder.Services.AddIdentity<User, IdentityRole<long>>()
    .AddEntityFrameworkStores<AppDbContext>();

// Get JWT token info
var jwtTokeInfo = builder.Configuration.GetSection("jwt").Get<JWTTokenInfo>();

<<<<<<< HEAD


// JWT Registration
builder.Services.AddAuthentication(

    (options) =>
=======
// JWT Registration with Logger
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
>>>>>>> 17faaceed86e8d33184d627fb7213dea0f26f325
    {
        ValidateIssuer = true,
        ValidIssuer = jwtTokeInfo!.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtTokeInfo!.Audience,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokeInfo!.Key))
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            if (context.Request.Path.StartsWithSegments("/ws/notifications"))
            {
                var token = context.Request.Query["token"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                    logger.LogInformation($"[WS] Token received: {token}");
                }
                else
                {
                    logger.LogWarning("[WS] No token found in query string");
                }
            }
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogInformation($"[WS] Token validated. Claims: {string.Join(", ", context.Principal.Claims.Select(c => $"{c.Type}: {c.Value}"))}");
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError($"[WS] Authentication failed: {context.Exception.Message}");
            context.HttpContext.Items["AuthFailure"] = context.Exception.Message;
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogWarning("[WS] Authentication challenge triggered.");
            return Task.CompletedTask;
        }
    };
});

// Register other services
builder.Services.Configure<JWTTokenInfo>(builder.Configuration.GetSection("jwt"));
builder.Services.AddScoped<JWTService>();
builder.Services.AddScoped<JWTService>(provider =>
    new JWTService(provider.GetRequiredService<IOptions<JWTTokenInfo>>(),
                   provider.GetRequiredService<UserManager<User>>()));
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IWishlistService, WishlistService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IAnnouncementService, AnnouncementService>();
builder.Services.AddScoped<IOrderService, OrderService>();
// In ConfigureServices method
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddSingleton<NotificationWebSocketHandler>();

builder.Services.AddScoped<IEmailService, EmailService>(); // Registering Email Service and it's Interface Service

builder.Services.AddAuthorization();

builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// Build the app
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

<<<<<<< HEAD
// Enable static file serving
app.UseStaticFiles();
=======
var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};
app.UseWebSockets(webSocketOptions);

// Map the /ws/notifications endpoint
app.Map("/ws/notifications", async (HttpContext context) =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        var handler = context.RequestServices.GetRequiredService<NotificationWebSocketHandler>();
        await handler.HandleWebSocketAsync(context, webSocket);
    }
    else
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
    }
});
>>>>>>> 17faaceed86e8d33184d627fb7213dea0f26f325

app.MapControllers();

app.Run();