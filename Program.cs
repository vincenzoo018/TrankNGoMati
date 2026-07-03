using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.EntityFrameworkCore;
using TrackNGoMati.Filters;
using TrackNGoMati.Models;
using TrackNGoMati.Services;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------
// Services
// ---------------------------------------------------------------
builder.Services.AddControllersWithViews();

// EF Core
builder.Services.AddDbContext<TrackNgoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TrackNGoContext")));

// Session (for auth)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout        = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly    = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name        = "TrackNGo.Session";
});

// HttpContextAccessor (so services can read the session)
builder.Services.AddHttpContextAccessor();

// Application services
builder.Services.AddScoped<SmsService>();
builder.Services.AddScoped<ExportService>();
builder.Services.AddScoped<WorkflowEngine>();
builder.Services.AddScoped<OcrService>();
builder.Services.AddHostedService<ArtaEscalationService>();
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));



var app = builder.Build();

// ---------------------------------------------------------------
// Seed Database
// ---------------------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context  = services.GetRequiredService<TrackNgoDbContext>();
    context.Database.EnsureDeleted(); // FORCE RESET DB
    TrackNGoMati.Data.DbInitializer.Initialize(context);
}

// ---------------------------------------------------------------
// Middleware Pipeline
// ---------------------------------------------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();        // Must be before auth
app.UseAuthorization();

// ---------------------------------------------------------------
// Routes
// ---------------------------------------------------------------
app.MapControllerRoute(
    name:    "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name:    "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
