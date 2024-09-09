using Hotel_Reservation_System.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using DinkToPdf;
using DinkToPdf.Contracts;
using System;
using email_sender;  // تأكد من تضمين المسار الصحيح لمكتبة ارسال البريد الإلكتروني

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// تسجيل خدمة DinkToPdf و PdfService
builder.Services.AddSingleton<IConverter>(new SynchronizedConverter(new PdfTools()));  // تم تصحيح التسجيل هنا
builder.Services.AddTransient<PdfService>();

// Add services for email sending
builder.Services.AddTransient<IEmailSender, EmailSender>();

// Configure Entity Framework with Oracle
builder.Services.AddDbContext<ModelContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configure authentication with cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Login"; // Path to the login page
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable session handling
app.UseSession();

// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// Configure the default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
