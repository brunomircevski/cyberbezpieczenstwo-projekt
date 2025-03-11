using Cyberbezpieczenstwo.Data;
using Cyberbezpieczenstwo.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<SQLiteContext>(
    options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddSingleton<AccountService>();

var app = builder.Build();

//Apply database migrations if needed
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var dbContext = services.GetRequiredService<SQLiteContext>();
        string databasePath = builder.Configuration.GetConnectionString("DefaultConnection")!
            .Replace("Data Source=", string.Empty);

        var pendingMigrations = dbContext.Database.GetPendingMigrations();
        if (pendingMigrations.Any())
        {
            // Create a backup file in the same directory
            string backupFileName = Path.Combine(
                Path.GetDirectoryName(databasePath)!,
                $"database-backup-{DateTime.Now:dd-MM-yyyy-HH:mm}.db");

            if (File.Exists(databasePath))
            {
                File.Copy(databasePath, backupFileName);
                Console.WriteLine($"Database backup created at: {backupFileName}");
            }
            else
            {
                Console.WriteLine($"Database file not found at: {databasePath}");
            }

            // Apply migrations
            Console.WriteLine("Applying pending migrations...");
            dbContext.Database.Migrate();
            Console.WriteLine("Migrations applied successfully.");
        }
        else
        {
            Console.WriteLine("No pending migrations found. Database is up-to-date.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while checking or applying migrations: {ex.Message}");
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
