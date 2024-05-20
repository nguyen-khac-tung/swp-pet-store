using Microsoft.EntityFrameworkCore;
using PetStoreProject.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<PetStoreDBContext>(option =>
    option.UseSqlServer(builder.Configuration.GetConnectionString("PetStoreDBContext"))
);

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}"
    );

app.Run();
