using TechVault.UI.Components;
using TechVault.UI.Data; // This is required to access our Data Managers

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ==========================================
// DEPENDENCY INJECTION REGISTRATIONS
// ==========================================
// This tells Blazor to create one instance (Singleton) of these classes
// and provide them to any page that asks for them via @inject
builder.Services.AddSingleton<DatabaseManager>();
builder.Services.AddSingleton<MockDataManager>();
// ==========================================

var app = builder.Build();

// Initialize the SQLite database on startup
using (var scope = app.Services.CreateScope())
{
    var dbManager = scope.ServiceProvider.GetRequiredService<DatabaseManager>();
    dbManager.InitializeDatabase();
    dbManager.SeedDataIfEmpty();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();