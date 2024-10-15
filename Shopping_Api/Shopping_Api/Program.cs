using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Shopping_Api.ApplicationDb;
using Shopping_Api.AutoMapping;
using Shopping_Api.Repository.IRepository;
using Shopping_Api.Repository;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configure the database context with SQL Server
string Connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Connection));

// Configure Identity for user authentication and authorization
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Register AutoMapper with the mapping profile
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Register repository services with dependency injection
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductImageRepository, ProductImageRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// Add controllers to the services collection
builder.Services.AddControllers();

// Configure CORS to allow requests from the React application
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000", "http://10.0.2.2", "http://localhost:8081", "http://192.168.11.220:8081") // Allow requests from this origin
                   .AllowAnyHeader() // Allow any header
                   .AllowAnyMethod(); // Allow any HTTP method
        });
});

// Configure authentication with JWT
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // Set default authentication scheme to JWT
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // Set default challenge scheme to JWT
})
.AddJwtBearer(opt =>
{
    opt.RequireHttpsMetadata = false; // Disable HTTPS requirement for JWT tokens (for development purposes)
    opt.SaveToken = true; // Save the token in the Authentication properties
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true, // Validate the signing key
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JWT:SecretKey"])), // Set the signing key
        ValidateIssuer = true, // Validate the issuer
        ValidIssuer = builder.Configuration["JWT:Issuer"], // Set the valid issuer
        ValidateAudience = true, // Validate the audience
        ValidAudience = builder.Configuration["JWT:Audience"] // Set the valid audience
    };
});

// Add health checks to the application
builder.Services.AddHealthChecks();

// Add endpoint explorer and Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "JWT Auth Sample",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer jhfdkj.jkdsakjdsa.jkdsajk\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});


// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();
builder.Host.UseSerilog(); // Use Serilog for logging


var app = builder.Build();



// Enable Swagger and Swagger UI for API documentation
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection(); 

app.UseStaticFiles(); // Serve static files (e.g., images, CSS, JavaScript)

app.UseCors("AllowReactApp"); // Apply the CORS policy for React app

app.UseAuthentication(); 

app.UseAuthorization(); 

app.MapControllers(); 

app.Run(); 
