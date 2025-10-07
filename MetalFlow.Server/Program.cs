using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MetalFlow.Server;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 1. Configure the database context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=localhost;Initial Catalog=MetalFlow;User ID=sa;Password=YourStrong(!)Password123;TrustServerCertificate=true";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. Add Identity services
builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// 3. Configure JWT Bearer authentication
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

// 4. Add CORS policy
const string mauiClientPolicy = "MauiClientPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: mauiClientPolicy,
                      policy =>
                      {
                          policy.AllowAnyOrigin()
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(mauiClientPolicy);

// 5. Map Identity API endpoints
app.MapIdentityApi<ApplicationUser>();

// 6. Map the secure data endpoint
app.MapGet("/api/securedata", (ClaimsPrincipal user) => $"Hello {user.Identity?.Name}! This is secure data.")
   .RequireAuthorization();

app.Run();