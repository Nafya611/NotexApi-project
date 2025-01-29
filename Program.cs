using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Any;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "NotexApi",
            ValidAudience = "NotexClient",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("S3cureRandomSecretKey12345!@#$54321"))
        };
    });

builder.Services.AddControllers();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Note API", Version = "v1" });

    c.MapType<Note>(() => new OpenApiSchema
    {
        Type = "object",
        Properties = new Dictionary<string, OpenApiSchema>
        {
            ["id"] = new OpenApiSchema { Type = "string", Example = new OpenApiString("65b1234567abcd8912345678") },
            ["title"] = new OpenApiSchema { Type = "string", Example = new OpenApiString("My Note") },
            ["content"] = new OpenApiSchema { Type = "string", Example = new OpenApiString("This is my note content.") },
            ["createdAt"] = new OpenApiSchema { Type = "string", Format = "date-time" },
            ["updatedAt"] = new OpenApiSchema { Type = "string", Format = "date-time" }
        }
    });
});


// Bind MongoDbSettings from configuration
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

// Register the class as a singleton
builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IOptions<MongoDbSettings>>().Value);

builder.Services.AddScoped<UserService>();

builder.Services.AddScoped<INoteService, NoteService>();

var app = builder.Build();

// Enable Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
