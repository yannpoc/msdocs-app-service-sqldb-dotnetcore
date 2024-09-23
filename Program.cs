using DotNetCoreSqlDb.Data;
using DotNetCoreSqlDb.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add database context and cache
if (builder.Environment.IsDevelopment())
{
    var cs = builder.Configuration.GetConnectionString("MyDbConnection");
    Console.WriteLine($"Connection string: '{cs}'");
    builder.Services.AddDbContext<MyDatabaseContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("MyDbConnection")));
    builder.Services.AddDistributedMemoryCache();
}
else
{
    builder.Services.AddDbContext<MyDatabaseContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING")));
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration["AZURE_REDIS_CONNECTIONSTRING"];
        options.InstanceName = "SampleInstance";
    });
}

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add App Service logging
builder.Logging.AddAzureWebAppDiagnostics();

builder.Services.AddScoped<DataService>();

var app = builder.Build();

// ensure database and tables exist
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<DataService>();
    await context.InitDatabase();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Todos}/{action=Index}/{id?}");

app.Run();