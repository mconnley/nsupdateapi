using NLog;
using NLog.Web;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Configuration.AddJsonFile("secrets/secrets.json", optional: false);

    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddSingleton(builder.Configuration.GetSection("customConfiguration").Get<CustomConfiguration>());
    builder.Services.AddHealthChecks();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.MapHealthChecks("/healthz");

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    IHostApplicationLifetime lifetime = app.Lifetime;

    lifetime.ApplicationStopping.Register(() =>
    {
        Thread.Sleep(5000);
    });

    app.Run();
}
catch (System.Exception ex)
{
    logger.Error(ex, "Stopped program because of exception");
}
finally
{
    NLog.LogManager.Shutdown();
}

