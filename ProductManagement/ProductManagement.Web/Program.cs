using Microsoft.EntityFrameworkCore;
using ProductManagement.Application.Services;
using ProductManagement.Application.Users;
using ProductManagement.Domain.Repositories;
using ProductManagement.Infrastructure.Database;
using ProductManagement.Infrastructure.Repositories;
using ProductManagement.Web;
using ProductManagement.Web.Exceptions;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddAutoMapper(
    AppDomain.CurrentDomain.GetAssemblies()
);

builder.Services.AddDbContext<ProductDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("ProductDbContext"));
});

builder.Services.AddUserHttpClient(builder.Configuration);

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IUserServiceClient, UserServiceClient>();

builder.Services.AddHttpContextAccessor();


builder.Services.AddAuthentication(builder.Configuration);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwagger();

var app = builder.Build();

app.MigrateDatabase();

//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseExceptionHandler();
app.Run();
