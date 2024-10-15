using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);


//Configure Orelot Gateway
builder.Configuration.AddJsonFile("ocelotGateway.json", optional: true, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);

// Configure CORS to allow requests from the React application
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000") // Allow requests from this origin
                   .AllowAnyHeader() // Allow any header
                   .AllowAnyMethod(); // Allow any HTTP method
        });
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

// Use CORS policy
app.UseCors("AllowReactApp");

app.MapControllers();

await app.UseOcelot();

app.Run();
