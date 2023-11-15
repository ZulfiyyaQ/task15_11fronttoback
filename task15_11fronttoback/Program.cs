var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
var app = builder.Build();
app.UseRouting();
app.UseStaticFiles();
app.MapControllerRoute(
    "task_front_to_back",
    "{controller=home}/{action=index}/{id?}"
    );

app.Run();
