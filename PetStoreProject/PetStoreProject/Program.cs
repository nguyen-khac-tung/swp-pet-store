using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.EntityFrameworkCore;
using PetStoreProject.Areas.Admin.Service.Cloudinary;
using PetStoreProject.Helpers;
using PetStoreProject.Models;
using PetStoreProject.Repositories.Accounts;
using PetStoreProject.Repositories.Attribute;
using PetStoreProject.Repositories.Brand;
using PetStoreProject.Repositories.Cart;
using PetStoreProject.Repositories.Category;
using PetStoreProject.Repositories.Customers;
using PetStoreProject.Repositories.FeedBack;
using PetStoreProject.Repositories.Image;
using PetStoreProject.Repositories.Order;
using PetStoreProject.Repositories.OrderService;
using PetStoreProject.Repositories.Product;
using PetStoreProject.Repositories.ProductCategory;
using PetStoreProject.Repositories.ProductOption;
using PetStoreProject.Repositories.Service;
using PetStoreProject.Repositories.Size;
using PetStoreProject.Repositories.WishList;

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
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
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

builder.Services.AddScoped<IWishListRepository, WishListRepository>();

builder.Services.AddTransient<ICustomerRepository, CustomerRepository>();

builder.Services.AddTransient<IProductRepository, ProductRepository>();

builder.Services.AddTransient<ICartRepository, CartRepository>();

builder.Services.AddTransient<ICategoryRepository, CategoryRepository>();

builder.Services.AddTransient<IFeedbackRepository, FeedbackRepository>();

builder.Services.AddTransient<IProductCategoryRepository, ProductCategoryRepository>();

builder.Services.AddTransient<IBrandRepository, BrandRepository>();

builder.Services.AddTransient<IAttributeRepository, AttributeRepository>();

builder.Services.AddTransient<ISizeRepository, SizeRepository>();

builder.Services.AddTransient<IBrandRepository, BrandRepository>();

builder.Services.AddTransient<IProductOptionRepository, ProductOptionRepository>();

builder.Services.AddTransient<IImageRepository, ImageRepository>();

builder.Services.AddTransient<IServiceRepository, ServiceRepository>();

builder.Services.AddTransient<IOrderRepository, OrderRepository>();

builder.Services.AddTransient<IOrderServiceRepository, OrderServiceRepository>();

builder.Services.AddSingleton(new CloudinaryDotNet.Cloudinary(new CloudinaryDotNet.Account(
        builder.Configuration.GetSection("Cloudinary:CloudName").Value,
        builder.Configuration.GetSection("Cloudinary:ApiKey").Value,
        builder.Configuration.GetSection("Cloudinary:ApiSecret").Value
    )));

builder.Services.AddTransient<ICloudinaryService, CloudinaryService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "area",
    pattern: "{area:exists}/{controller}/{action}"
    );

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}"
    );

app.Run();
