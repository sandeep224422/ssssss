using Microsoft.EntityFrameworkCore;
using YouTubeDownloader.Contexts;
using YouTubeDownloader.Services;
using YouTubeDownloader.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddControllers();

// Add API services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register YouTube service
builder.Services.AddScoped<IYouTubeService, YouTubeService>();

// Database context
builder.Services.AddDbContext<SongsDbContext>(options =>
{
    options.UseSqlite("Data Source=songs.db");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Add API key authentication middleware
app.UseMiddleware<ApiKeyMiddleware>();

app.UseAuthorization();
app.UseRouting();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Song}/{action=Index}/{id?}");

app.Run();