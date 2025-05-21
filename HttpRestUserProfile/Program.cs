using AccountProfileServiceProvider.Contexts;
using AccountProfileServiceProvider.Repos;
using AccountProfileServiceProvider.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<UserProfileContext>(x => x.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection")));
builder.Services.AddScoped<UserProfileServices>();
builder.Services.AddScoped<UserProfileRepo>();


var app = builder.Build();
app.MapOpenApi();
app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();
app.Run();
