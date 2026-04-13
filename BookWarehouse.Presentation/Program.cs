using BookWarehouse.Application.Comman.Settings;
using BookWarehouse.Application.ServicesExtension.cs;
using BookWarehouse.Domain.Entities;
using BookWarehouse.Infrastructure.Persistence.Context;
using BookWarehouse.Infrastructure.Persistence.Seeders;
using BookWarehouse.Infrastructure.ServicesExtention;
using BookWarehouse.Presentation.ServicesExtention;
using Microsoft.AspNetCore.Identity;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();



builder.Services.AddPresentationServices()
                .AddApplicationServices()
                .AddInfrastructureServices(builder.Configuration);

StripeConfiguration.ApiKey=builder.Configuration.GetSection("Stripe:SecretKey").Get<string>()
                                    ?? throw new InvalidOperationException("Stripe Secret Key not found in configuration.");



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
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    
    pattern: "{controller=CustomerProduct}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
