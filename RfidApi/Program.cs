using System.Security.Claims;
using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RfidApi.Authentication;
using RfidApi.Data;

var builder = WebApplication.CreateBuilder(args);

// --- Database registration ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=rfid.db") // You can use builder.Configuration if you want to move this to appsettings.json
);

// Add dummy authentication

builder
    .Services.AddAuthentication("DummyScheme")
    .AddScheme<AuthenticationSchemeOptions, DummyAuthHandler>("DummyScheme", options => { });

// More realistic auth

// builder
//     .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//         options.Authority = "https://dev-frxd7pfagyp0onvq.us.auth0.com/";
//         options.Audience = "https://tagsapp.com/";
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             NameClaimType = ClaimTypes.NameIdentifier,
//         };
//         options.Events = new JwtBearerEvents
//         {
//             OnAuthenticationFailed = context =>
//             {
//                 // Log or inspect the exception here
//                 Console.WriteLine("Authentication failed: " + context.Exception.Message);
//                 return Task.CompletedTask;
//             },
//             OnTokenValidated = context =>
//             {
//                 // Inspect the token claims here
//                 var claims = context.Principal!.Claims;
//                 foreach (var claim in claims)
//                 {
//                     Console.WriteLine($"Claim: {claim.Type} = {claim.Value}");
//                 }
//                 return Task.CompletedTask;
//             },
//             OnChallenge = context =>
//             {
//                 // Inspect challenge details
//                 Console.WriteLine("Challenge error: " + context.ErrorDescription);
//                 return Task.CompletedTask;
//             },
//         };
//     });

// builder
//     .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//         options.Authority = "https://dev-frxd7pfagyp0onvq.us.auth0.com/";
//         options.Audience = "tags-scope";
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             NameClaimType = ClaimTypes.NameIdentifier,
//         };
//     });

// Security settings
// builder.Services.AddAuth0WebAppAuthentication(options =>
// {
//     options.Domain = "dev-frxd7pfagyp0onvq.us.auth0.com";
//     options.ClientId = "DbZJ2LjNy2THMqwiGKpqVuz8bORDDbYC";
//     options.OpenIdConnectEvents =
//         new Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectEvents
//         {
//             OnTokenValidated = context =>
//             {
//                 var identity = context.Principal!.Identity as ClaimsIdentity;
//                 var roleClaims = context.Principal.FindAll("https://yourdomain.com/roles").ToList();
//                 if (identity != null)
//                 {
//                     foreach (var roleClaim in roleClaims)
//                     {
//                         identity.AddClaim(new Claim(ClaimTypes.Role, roleClaim.Value));
//                     }
//                 }
//                 return Task.CompletedTask;
//             },
//         };
// });

builder.Services.AddItemServices(relativePath: "");
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAll",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
    );
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- Ensure database is created at runtime ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// --- Middleware configuration ---

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.MapControllers();

app.Run();
