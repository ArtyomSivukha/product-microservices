using Microsoft.EntityFrameworkCore;
using UserManagement.Application;
using UserManagement.Application.LinkURI;
using UserManagement.Application.Products;
using UserManagement.Application.Services.EmailConfirmService;
using UserManagement.Application.Services.UserService;
using UserManagement.Domain.Repositories;
using UserManagement.Infrastructure.Database;
using UserManagement.Infrastructure.Email;
using UserManagement.Infrastructure.Products;
using UserManagement.Infrastructure.Repositories;
using UserManagement.Web;
using UserManagement.Web.Exceptions;
using UserManagement.Web.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddAutoMapper(
    AppDomain.CurrentDomain.GetAssemblies()
);
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<UserDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("UserDbContext"));
});

builder.Services.AddProductHttpClient(builder.Configuration);

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<ILinkService, LinkService>();
builder.Services.AddScoped<IEmailConfirmRepository, EmailConfirmRepository>();
builder.Services.AddScoped<IEmailConfirmService, EmailConfirmService>();
builder.Services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();
builder.Services.AddScoped<IProductServiceClient, ProductServiceClient>();

builder.Services.Configure<Settings>(
    builder.Configuration.GetSection("EmailSettings")
);
builder.Services.Configure<EmailConfiguration>(
    builder.Configuration.GetSection("EmailConfiguration")
);

builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection("Jwt")
);

builder.Services.AddAuthentication(builder.Configuration);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddSwagger();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000") // React dev server
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

var app = builder.Build();

app.MigrateDatabase();

// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI();
// }

app.UseRouting();
app.UseAuthentication(); 
app.UseAuthorization();
app.UseMiddleware<UserAccessorMiddleware>();

app.UseExceptionHandler();
app.UseCors("ReactApp");

app.MapControllers();
app.Run();
