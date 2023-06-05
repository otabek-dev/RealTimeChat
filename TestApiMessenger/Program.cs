using Microsoft.EntityFrameworkCore;
using RealTimeMail;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("LocalConnection")
    ?? throw new InvalidOperationException("Connection string 'LocalConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddSignalR();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapHub<ChatHub>("/chatHub");

app.Run();