using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.EntityFrameworkCore;
using PetStoreProject.Helper;
using PetStoreProject.Models;
using PetStoreProject.Repositories.Accounts;
using PetStoreProject.Repositories.Cart;
using PetStoreProject.Repositories.Product;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PetStoreDBContext>(option =>
    option.UseSqlServer(builder.Configuration.GetConnectionString("PetStoreDBContext"))
);

builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromMinutes(30);
    option.Cookie.HttpOnly = true;
    option.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
    .AddCookie()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration.GetSection("GoogleKeys:ClientId").Value;
        options.ClientSecret = builder.Configuration.GetSection("GoogleKeys:ClientSecret").Value;
        options.CallbackPath = "/signin-google";
        options.Events = new OAuthEvents
        {
            OnRemoteFailure = (context) =>
            {
                context.HandleResponse();
                context.Response.Redirect("/Account/GoogleFailure");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddSingleton<EmailService>();

builder.Services.AddScoped<IAccountRepository, AccountRepository>();

builder.Services.AddTransient<IProductRepository, ProductRepository>();

builder.Services.AddTransient<ICartRepository, CartRepository>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}"
    );

app.Run();
