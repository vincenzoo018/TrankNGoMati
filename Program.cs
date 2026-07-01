var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

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

app.UseAuthorization();

// Custom routes for all pages
app.MapControllerRoute(name: "dashboard",    pattern: "Dashboard/{action=Index}/{id?}",    defaults: new { controller = "Dashboard" });
app.MapControllerRoute(name: "documents",    pattern: "Documents/{action=Index}/{refNo?}",  defaults: new { controller = "Documents" });
app.MapControllerRoute(name: "workflow",     pattern: "Workflow/{action=Index}/{id?}",     defaults: new { controller = "Workflow" });
app.MapControllerRoute(name: "qr",           pattern: "QR/{action=Index}/{id?}",           defaults: new { controller = "QR" });
app.MapControllerRoute(name: "signatures",   pattern: "Signatures/{action=Index}/{id?}",   defaults: new { controller = "Signatures" });
app.MapControllerRoute(name: "sms",          pattern: "SMS/{action=Index}/{id?}",          defaults: new { controller = "Sms" });
app.MapControllerRoute(name: "reports",      pattern: "Reports/{action=Index}/{id?}",      defaults: new { controller = "Reports" });
app.MapControllerRoute(name: "users",        pattern: "Users/{action=Index}/{id?}",        defaults: new { controller = "Users" });
app.MapControllerRoute(name: "departments",  pattern: "Departments/{action=Index}/{id?}",  defaults: new { controller = "Departments" });
app.MapControllerRoute(name: "escalation",   pattern: "Escalation/{action=Index}/{id?}",   defaults: new { controller = "Escalation" });
app.MapControllerRoute(name: "audit",        pattern: "Audit/{action=Index}/{id?}",        defaults: new { controller = "Audit" });

// Default route — login page
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
