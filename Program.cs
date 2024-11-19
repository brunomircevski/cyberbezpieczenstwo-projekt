using BDwAS_projekt.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

//Wybór bazy danych jako IDbContext
builder.Services.AddDbContext<IDbContext, SQLiteContext>(
    options => options.UseSqlite("Data Source=database.db")
);

/*
builder.Services.AddScoped<IDbContext, MongoContext>(provider =>
    new MongoContext("mongodb://localhost:27017", "database")
);
*/

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
