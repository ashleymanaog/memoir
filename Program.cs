using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using ThomasianMemoir.Data;
using ThomasianMemoir.Configuration;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using ThomasianMemoir.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Configuration.AddJsonFile("appsettings.json");
var httpSettings = builder.Configuration.GetSection("HttpSettings").Get<HttpSettings>();
var securitySettings = builder.Configuration.GetSection("SecuritySettings").Get<SecuritySettings>();

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });

// Configure maximum request length and execution timeout
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = httpSettings.MaxRequestLength; // in bytes
});

// Configure maximum allowed content length
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = httpSettings.MaxRequestLength; // in bytes
});

// Database Connection Service
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("MyConnectionString"))
);

// Identity Settings
builder.Services.AddDefaultIdentity<User>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(30); // expiration time
    options.LoginPath = "/Home/Login";
    options.SlidingExpiration = true;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAny",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

builder.Services.AddLogging();

builder.Services.AddScoped<ProfanityFilterService>();

builder.Services.AddScoped<EmailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>();
context.Database.EnsureCreated(); //create database if not exists

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseCors("AllowAny");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "dashboard",
    pattern: "{controller=Dashboard}/{action=Homepage}/{id?}");

app.MapControllerRoute(
    name: "profile",
    pattern: "{controller=Profile}/{action=Profile}/{id?}");

app.Run();
