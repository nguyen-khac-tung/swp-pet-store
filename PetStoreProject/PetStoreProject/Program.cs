using Microsoft.EntityFrameworkCore;
using PetStoreProject.Models;
using PetStoreProject.Repositories.Accounts;
using PetStoreProject.Repositories.Cart;
using PetStoreProject.Repositories.Product;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<PetStoreDBContext>(option =>
    option.UseSqlServer(builder.Configuration.GetConnectionString("PetStoreDBContext"))
);

builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromMinutes(30);
    option.Cookie.HttpOnly = true;
    option.Cookie.IsEssential = true;
});

builder.Services.AddScoped<IAccountRepository, AccountRepository>();

builder.Services.AddTransient<IProductRepository, ProductRepository>();

builder.Services.AddTransient<ICartRepository, CartRepository>();


var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}"
    );

app.Run();
