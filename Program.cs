using LifeSure.Data;
using LifeSure.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "DefaultConnection bağlantı bilgisi bulunamadı.");

builder.Services.AddDbContext<LifeSureDbContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddCqrsHandlers();
builder.Services.AddMediatorHandlers();
builder.Services.AddAdminIdentity();
var app = builder.Build();

if (app.Configuration.GetValue<bool>("SeedAdmin:Enabled"))
{
    if (!app.Environment.IsDevelopment())
    {
        throw new InvalidOperationException(
            "Bu hesap oluşturma komutu yalnızca Development ortamında çalışır.");
    }

    using var scope = app.Services.CreateScope();

    await AdminAccountSeeder.SeedAsync(
        scope.ServiceProvider,
        app.Configuration);

    Console.WriteLine("Yönetici hesabı hazır.");

    return;
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
