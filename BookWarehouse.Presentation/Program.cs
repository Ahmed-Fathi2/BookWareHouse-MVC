using BookWarehouse.Application.ServicesExtension.cs;
using BookWarehouse.Infrastructure.Persistence.Seeders;
using BookWarehouse.Infrastructure.ServicesExtention;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();



builder.Services.AddApplicationServices()
                .AddInfrastructureServices(builder.Configuration);

var app = builder.Build();


var scope = app.Services.CreateScope();
var seeder= scope.ServiceProvider.GetRequiredService<IDataBaseSeeder>();
await seeder.SeedAsync();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
