using Microsoft.EntityFrameworkCore;
using task15_11fronttoback.DAL;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
var app = builder.Build();
app.UseRouting();
app.UseStaticFiles();
app.MapControllerRoute(
    "task_front_to_back",
    "{controller=home}/{action=index}/{id?}"
    );

app.Run();
