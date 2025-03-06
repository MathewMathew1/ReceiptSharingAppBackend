using ReceiptSharing.Api.Models;
using ReceiptSharing.Api.Repositories;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Globalization;
using ReceiptSharing.Api.MiddleWare;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddUserSecrets<Program>()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

// Add services to the container

builder.Services.AddLogging(builder =>
{
    builder.AddConsole(); // Use the Console logging provider
});

builder.Services.AddHealthChecks();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme; // Set the default challenge to Google

})
.AddCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.None; 
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
})
.AddGoogle(googleOptions =>
{
    googleOptions.ClientId = configuration["GoogleClientId"]!;
    googleOptions.ClientSecret = configuration["GoogleClientSecret"]!;
    googleOptions.ClaimActions.MapJsonKey("image", "picture");
})
.AddDiscord(options =>
{
    options.ClientId = configuration["DiscordClientId"]!;
    options.ClientSecret = configuration["DiscordClientSecret"]!;
    options.Scope.Add("identify"); // Request access to the user's identity
    options.Scope.Add("email");
    options.ClaimActions.MapJsonKey("image", "picture");
    options.ClaimActions.MapCustomJson("urn:discord:avatar:url", user =>
        string.Format(
            CultureInfo.InvariantCulture,
            "https://cdn.discordapp.com/avatars/{0}/{1}.{2}",
            user.GetString("id"),
            user.GetString("avatar"),
            user.GetString("avatar")!.StartsWith("a_") ? "gif" : "png"));
});


builder.Services.AddControllers(options =>
{

}).AddJsonOptions(options =>
{
     options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<IReceiptRepository, ReceiptRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISubscriptionReceiptRepository, SubscriptionReceiptRepository>();
builder.Services.AddScoped<ISubscriptionUserRepository, SubscriptionUserRepository>();
builder.Services.AddScoped<IRatingRepository, RatingRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddSingleton<IImageStorage, IgmurImageStorage>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
if (builder.Environment.IsDevelopment())
    {
        builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
        {
            builder
                .WithOrigins("http://localhost:5173") // Add other allowed origins for development
                .AllowCredentials()
                .AllowAnyMethod()
                .AllowAnyHeader();
        }));
    }
    else // Production
    {
        builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
        {
            builder
                .WithOrigins("https://receptao.netlify.app") 
                .AllowCredentials()
                .AllowAnyMethod()
                .AllowAnyHeader();
        }));
    }

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();

app.UseCors("corsapp");
app.UseMiddleware<JwtMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    // Health check endpoint
    endpoints.MapHealthChecks("/health");
    endpoints.MapControllers();
});

app.Run();

