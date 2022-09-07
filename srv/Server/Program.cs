using Microsoft.AspNetCore.ResponseCompression;
using Server.DbOptions;
using Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<DbContext>(new DbContext(builder.Configuration.GetConnectionString("HANA")));
builder.Services.AddRazorPages();
builder.Services.AddHanaDbSetup();

var app = builder.Build();

try
{
    var dbSetup = app.Services.GetRequiredService<IDbSetup>();
    await dbSetup.SetupDbAsync();
}
catch (InvalidOperationException ex)
{
    app.Logger.LogError($"Couldn't connect to DB: {ex.Message}");
    await app.StopAsync();
    return;
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");
app.Map("/data", async context =>
{
    var dbService = app.Services.GetService<IDbSetup>();
    await context.Response.WriteAsync($"Time: {dbService?.TestMethod()}");
});

app.Run();
