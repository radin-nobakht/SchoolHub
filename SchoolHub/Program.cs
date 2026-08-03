using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SchoolHub.Adapter;
using SchoolHub.Interface;
using SchoolHub.Service;

var builder = WebApplication.CreateBuilder(args);
var loggerFactory = LoggerFactory.Create(builder => { });

// Add services to the container.
builder.Services.AddControllersWithViews();
var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
}, loggerFactory);
builder.Services.AddSingleton(mapperConfig.CreateMapper());

builder.Services.AddDbContext<MyContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("SqlCs"));
});
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ISchoolService, SchoolService>();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
        options.SlidingExpiration = true;
    });
builder.Services.AddSignalR();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=School}/{action=SchoolPage}/{id?}")
    .WithStaticAssets();


app.Run();
