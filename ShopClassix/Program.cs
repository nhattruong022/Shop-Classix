using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Shop_Classix.Repository;
using Microsoft.AspNetCore.SignalR;
using Shop_Classix.Service;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Shop_Classix.Helper;


var builder = WebApplication.CreateBuilder(args);

// Cấu hình kết nối VNPay
builder.Services.AddSingleton<IVnPayService, VnPayService>();

// Cấu hình kết nối cơ sở dữ liệu SQL Server
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:ConnectDb"]);
});


// Cấu hình MVC cho Controllers và Views
builder.Services.AddControllersWithViews();



// Cấu hình xác thực với Cookie Authentication
builder.Services.AddAuthentication(options =>
{ 

    //mặc định là UserCookie
    options.DefaultScheme = "UserCookie";
    options.DefaultChallengeScheme ="UserCookie";
})      //thêm cookie User
.AddCookie("UserCookie", options =>
{
    options.Cookie.Name = "UserCookie";
    options.LoginPath = "/KhachHang/Login";  // nếu chưa đăng nhập thì chuyển sang trang đăng nhập
    options.AccessDeniedPath = "/AccessDenied";  // người dùng đã đăng nhập nhưng không có quyền truy cập
})  //thêm cookie Admin
.AddCookie("AdminCookie", options =>
{
    options.Cookie.Name = "AdminCookie";
    options.LoginPath = "/KhachHang/Login";   // nếu admin chưa đăng nhập thì chuyển sang trang đăng nhập
    options.AccessDeniedPath = "/AccessDenied";   // người dùng đã đăng nhập nhưng không có quyền truy cập
});





// Cấu hình session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});



// Cấu hình phân quyền admin
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
});

// Cấu hình dịch vụ email
builder.Services.AddTransient<IEmailService, EmailService>();

// Cấu hình IHttpContextAccessor để hỗ trợ HttpContext trong toàn ứng dụng
builder.Services.AddHttpContextAccessor();


// Thêm SignalR services before building the app
builder.Services.AddSignalR();

//// Đăng ký CustomUserIdProvider
//builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

var app = builder.Build();

//cấu hình endpoint cho SignalR
app.MapHub<ChatHub>("/chatHub");
app.MapHub<ProductHub>("/productHub");




// Cấu hình HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();  // Hiển thị lỗi chi tiết trong môi trường phát triển
}
else
{
    app.UseExceptionHandler("/Home/Error");  // Xử lý lỗi chung cho ứng dụng trong môi trường sản xuất
    app.UseHsts();  // Cấu hình HSTS trong môi trường sản xuất
}

app.UseHttpsRedirection();  // Chuyển hướng từ HTTP sang HTTPS
app.UseStaticFiles();  // Cấu hình các file tĩnh

app.UseRouting();  // Cấu hình routing

app.UseSession();  // Sử dụng session

app.UseAuthentication();  // Cấu hình xác thực
app.UseAuthorization();  // Cấu hình phân quyền


// Cấu hình route cho Admin
app.MapControllerRoute(
    name: "areaRoute",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");


// Cấu hình route cho trang chủ
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Cấu hình route cho Cart
app.MapControllerRoute(
    name: "Cart",
    pattern: "{controller=Cart}/{action=Cart}/{id?}");

// Cấu hình route cho Contact
app.MapControllerRoute(
    name: "Contact",
    pattern: "{controller=Contact}/{action=Contact}/{id?}");

// Cấu hình route cho About
app.MapControllerRoute(
    name: "About",
    pattern: "{controller=About}/{action=About}/{id?}");

// Cấu hình route cho KhachHang
app.MapControllerRoute(
    name: "KhachHang",
    pattern: "{controller=KhachHang}/{action=Register}/{id?}");

app.Run();
