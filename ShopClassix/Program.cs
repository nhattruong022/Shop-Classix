using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Shop_Classix.Repository;
using Shop_Classix.Service;

var builder = WebApplication.CreateBuilder(args);

// Connect VNPay
builder.Services.AddSingleton<IVnPayService, VnPayService>();

// Connect to the database
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectDb"));
});

// Add services to the container
builder.Services.AddControllersWithViews();

// Configure authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/KhachHang/Login"; // Redirect to login page
        options.LogoutPath = "/KhachHang/LogOut"; // Redirect for logout
        options.AccessDeniedPath = "/AccessDenied"; // Redirect for access denied
    });

// Configure session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

<<<<<<< Updated upstream
// Đăng ký dịch vụ HTTP Context Accessor
builder.Services.AddHttpContextAccessor();

// Đăng ký bộ nhớ cache và session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
  options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


//phân quyền admin
=======
// Authorization policy for admin
>>>>>>> Stashed changes
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

// Register email service
builder.Services.AddTransient<IEmailService, EmailService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication(); // Ensure authentication middleware is added
app.UseAuthorization();

// Configure routes
app.MapControllerRoute(
    name: "areaRoute",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// Configure home route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Configure additional routes
app.MapControllerRoute(
    name: "Cart",
    pattern: "{controller=Cart}/{action=Cart}/{id?}");

app.MapControllerRoute(
    name: "Contact",
    pattern: "{controller=Contact}/{action=Contact}/{id?}");

app.MapControllerRoute(
    name: "About",
    pattern: "{controller=About}/{action=About}/{id?}");

app.MapControllerRoute(
    name: "KhachHang",
    pattern: "{controller=KhachHang}/{action=Register}/{id?}");

app.Run();