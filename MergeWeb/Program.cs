using MergeWeb.Hubs;
using FileMergeLibrary;
using Microsoft.AspNetCore.Identity;
using MergeWeb.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure Identity
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();

// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddFileMergeService();
builder.Services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");

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

app.UseAuthentication(); // Ensure Authentication is called before Authorization
app.UseAuthorization();

app.MapHub<UploadHub>("/uploadhub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=FileUpload}/{action=Index}/{id?}");

app.MapRazorPages(); // Ensure Razor Pages are mapped

app.Run();
