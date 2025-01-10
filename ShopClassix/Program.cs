using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Shop_Classix.Repository;
using Shop_Classix.Service;


var builder = WebApplication.CreateBuilder(args);


//connect VNPay
builder.Services.AddSingleton<IVnPayService, VnPayService>();


//Connect DB
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:ConnectDb"]);
});



// Add services to the container.
builder.Services.AddControllersWithViews();


//AddAuthentication: đăng ký dịch vụ xác thực cho ứng dụng
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/KhachHang/Login"; //  người dùng chưa đăng nhập -> chuyển hướng tới trang đăng nhập
    options.LoginPath = "/KhachHang/LogOut";  //trang xử lý đăng xuất
    options.AccessDeniedPath = "/AccessDenied";  //người dùng đã đăng nhập nhưng không đủ quyền truy cập
});
//session
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

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
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

builder.Services.AddTransient<IEmailService, EmailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

//cấu hình admin
app.MapControllerRoute(
    name: "areaRoute",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}" //id? tùy chọn
);

//cấu hình trang chủ
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


// Cấu hình route Cart
app.MapControllerRoute(
    name: "Cart",
    pattern: "{controller=Cart}/{action=Cart}/{id?}"
);


//cấu hình route Contact

app.MapControllerRoute(
    name: "Contact",
    pattern: "{controller=Contact}/{action=Contact}/{id?}"
);



//cấu hình route About
app.MapControllerRoute(
    name: "About",
    pattern: "{controller=About}/{action=About}/{id?}"
);


//cấu hình route KhachHang
app.MapControllerRoute(
    name: "KhachHang",
    pattern: "{controller=KhachHang}/{action=Register}/{id?}"
);




app.Run();
