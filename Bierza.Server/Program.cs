using System.Runtime.CompilerServices;
using Bierza.Business.Models;
using Bierza.Data;
using Bierza.Data.Models;
using Bierza.Business.PasswordUtils;
using Bierza.Business.UserManagement;
using Bierza.Business.UserManagement.Models;
using Bierza.Data.Users;
using Bierza.Server.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"];

builder.Services.AddSingleton(new ConnectionString(connectionString));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
// builder.Services.AddData(connectionString);
builder.Services.AddDbContext<DataDbContext>();
    
// Add services to the container.

// builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo()
    {
        Version = "v1",
        Title = "Beirza Api",
        Description = "An API for managing brewing resources",
        Contact = new OpenApiContact()
        {
            Name = "Jeffrey Drake",
            Url = new Uri("https://ideaplex.ca/")
        },
        License = new OpenApiLicense()
        {
            Name = "Private",
        }
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
});

builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"], // article has Issuer here
        IssuerSigningKey = new SymmetricSecurityKey(Convert.FromHexString(builder.Configuration["Jwt:Key"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };
});

builder.Services.AddTransient<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IUserCommands, UserCommands>();
builder.Services.AddScoped<IUserQueries, UserQueries>();
builder.Services.AddScoped<IUserManager, UserManager>();


// builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.UseStaticFiles(new StaticFileOptions()
{
    HttpsCompression = HttpsCompressionMode.Compress,
    RequestPath = "/",
    ServeUnknownFileTypes = false,
    RedirectToAppendTrailingSlash = true,
    FileProvider = app.Environment.WebRootFileProvider
});

app.Use(async (context, next) =>
{
    IUserManager userManager = context.RequestServices.GetService<IUserManager>()!;
    CancellationToken cancellationToken = CancellationToken.None;
    
    var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

    if (token != null)
    {
        UserReadModel? user = await userManager.ValidateToken(token, cancellationToken);

        if (user != null)
        {
            context.Items["User"] = user;
        }
    }

    await next(context);
});

app.MapPost("/login", async (AuthenticationRequest model, IUserManager userManager, CancellationToken token) =>
{
    AuthenticationResponse? response = await userManager.AuthenticateUser(model, token);

    if (response != null)
    {
        return Results.Json(response);
    }
    else
    {
        return Results.Unauthorized();
    }
});

app.MapPost("/users", [Authorize] async (IUserManager userManager, CancellationToken token) =>
{
    var users = await userManager.GetAllUsers(token);

    return Results.Json(users);
});

app.MapPost("/create-user", async (CreateUserRequestModel model, IUserManager userManager, CancellationToken cancellationToken) =>
{
    try
    {
        Guid guid = await userManager.CreateUser(model, cancellationToken);

        return Results.Json(new
        {
            userId = guid.ToString(),
        });
    }
    catch (UserDataException)
    {
        return Results.Problem("Unable to create user");
    }
});



app.Run();