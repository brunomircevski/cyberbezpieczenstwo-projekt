using BDwAS_projekt.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Wybór bazy danych jako IDbContext

//builder.Services.AddDbContext<IDbContext, SQLiteContext>(
//    options => options.UseSqlite("Data Source=database.db")
//);



// builder.Services.AddDbContext<IDbContext, PostgreContext>(options =>
//     options.UseNpgsql("Host=localhost;Database=bdwas;Username=postgres;Password=qwerty")
// );



//builder.Services.AddScoped<IDbContext, MongoContext>(provider =>
//    new MongoContext("mongodb://localhost:27017", "bdwas")
//);


// Rejestracja OracleContext z przekazanym connectionString
var oracleConnectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=xe)));User Id=C##test_user;Password=qwerty;";
 builder.Services.AddScoped<IDbContext>(provider => new OracleContext(oracleConnectionString));


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
