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
    .AddUserSecrets<Program>()
    .Build();

// Add services to the container
Console.WriteLine("");

builder.Services.AddHealthChecks();
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme; // Set your desired default scheme
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme; // Set the desired challenge scheme
    }).AddCookie(options =>
        {
      
        })
    .AddGoogle(GoogleDefaults.AuthenticationScheme,googleOptions =>
    {
        googleOptions.ClientId = configuration["Authentication__Google__ClientId"]!;
        googleOptions.ClientSecret = configuration["Authentication__Google__ClientSecret"]!;
        googleOptions.ClaimActions.MapJsonKey("image", "picture");
    });

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddDiscord(options =>
    {
        options.ClientId = "1139169569206976634";
        options.ClientSecret = "PMva8Q9krDw87KcsTKmOHKOT31YLwzTB";
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

builder.Services.AddLogging(builder =>
    {
        builder.AddConsole(); // Use the Console logging provider
    });
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
builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder
        .WithOrigins("http://localhost:5173") // Add other allowed origins if needed
        .AllowCredentials()
        .AllowAnyMethod()
        .AllowAnyHeader();
}));

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

