using AnimeStreamerV2.DbContextFile;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using test.Models.AdminSystem;
using test.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<AnimeDbContext>(options =>
    options.UseSqlServer(connectionString));



// Konfigurace Identity
builder.Services.Configure<IdentityOptions>(options =>
{
    // Nastaven� hesla
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Nastaven� uzam�en� ��tu
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // Nastaven� u�ivatele
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
});
builder.Services.AddRazorPages();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AnimeDbContext>();
/**/
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();



app.MapGet("/video", async (HttpContext context, AnimeDbContext dbContext, IWebHostEnvironment env) =>
{
    string id = context.Request.Query["id"].ToString();
    try
    {
        var episode = await dbContext.Episodes.FindAsync(int.Parse(id));
        if (episode == null)
        {
            return Results.NotFound($"Episode with ID {id} not found.");
        }

        string path = Path.Combine(env.WebRootPath, "anime", $"{id}", $"{episode.VideoPath}{episode.VideoType}");//episode.VideoPath;
        if (!System.IO.File.Exists(path))
        {
            return Results.NotFound($"File {path} not found.");
        }

        var fileInfo = new FileInfo(path);
        long fileLength = fileInfo.Length;
        const int bufferSize = 1024 * 1024; // 1MB buffer

        context.Response.Headers.Add("Content-Type", $"video/{episode.VideoType}");
        context.Response.Headers.Add("Content-Length", fileLength.ToString());
        context.Response.Headers.Add("Accept-Ranges", "bytes");

        using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, FileOptions.Asynchronous);

        if (context.Request.Headers.Range.Count > 0)
        {
            // Handle range requests for resuming playback
            var rangeHeader = context.Request.Headers.Range.ToString();
            var range = rangeHeader.Replace("bytes=", "").Split('-');
            var start = long.Parse(range[0]);
            var end = range.Length > 1 && !string.IsNullOrEmpty(range[1]) ? long.Parse(range[1]) : fileLength - 1;

            context.Response.StatusCode = 206;
            context.Response.Headers.Add("Content-Range", $"bytes {start}-{end}/{fileLength}");
            fileStream.Seek(start, SeekOrigin.Begin);
            await fileStream.CopyToAsync(context.Response.Body, bufferSize, context.RequestAborted);
        }
        else
        {
            // Stream the entire file
            await fileStream.CopyToAsync(context.Response.Body, bufferSize, context.RequestAborted);
        }

        return Results.Empty;
    }
    catch (Exception ex)
    {
        return Results.Problem($"An error occurred: {ex.Message}");
    }
});
app.MapGet("/allSsubtitles", async (HttpContext context, AnimeDbContext dbContext) =>
{
    string id = context.Request.Query["id"].ToString();
    var subtitles = await dbContext.Subtitles
            .Where(s => s.AnimeEpisodeModelId == int.Parse(id))
            .Select(s => new { s.Id, s.Language })
            .ToListAsync();

    return Results.Json(subtitles);
});

app.MapGet("/subtitles", async (HttpContext context, AnimeDbContext dbContext, IWebHostEnvironment env) =>
{
    string id = context.Request.Query["id"].ToString();
    string lang = context.Request.Query["lang"].ToString();

    try
    {
        var subtitle = await dbContext.Subtitles
            .FirstOrDefaultAsync(s => s.Id == int.Parse(id));

        if (subtitle == null)
        {
            return Results.NotFound($"Subtitles for episode ID {id} and language {lang} not found.");
        }

        if (!System.IO.File.Exists(subtitle.Path))
        {
            return Results.NotFound($"Subtitle file not found at path: {subtitle.Path}");
        }

        context.Response.Headers.Add("Content-Type", "text/plain");
        await context.Response.SendFileAsync(subtitle.Path);
        return Results.Empty;
    }
    catch (Exception ex)
    {
        return Results.Problem($"An error occurred: {ex.Message}");
    }
});

/**/
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    await DbInitializer.SeedRoles(roleManager);
}
/**/
app.UseAuthentication()/*.AddCokie(IdentityConstants.ApplicationScheme)*/;
app.UseAuthorization();
/*
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Anime}/{action=Index}/{id?}");
*/
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Anime}/{action=Index}/{id?}");
    endpoints.MapRazorPages();
});
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    await RoleInitializer.InitializeAsync(roleManager);
}
app.Run();
