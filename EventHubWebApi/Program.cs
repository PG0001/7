using EventHubLibrary.Models;
using EventHubLibrary.Repositories;
using EventHubLibrary.Repositories.Interfaces;
using EventHubWebApi.Models;
using EventHubWebApi.Services;
using EventHubWebApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// =======================================================================
// 1. DATABASE
// =======================================================================
builder.Services.AddDbContext<DBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// =======================================================================
// 2. CONFIGURATION BINDINGS (Email & SMS)
// =======================================================================
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection("TwilioSettings"));

builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<EmailSettings>>().Value);
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<TwilioSettings>>().Value);
builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection("TwilioSettings"));
var twilioSettings = new TwilioSettings
{
    AccountSID = Environment.GetEnvironmentVariable("TWILIO_SID"),
    AuthToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN"),
    FromNumber = Environment.GetEnvironmentVariable("TWILIO_FROM")
};


// =======================================================================
// 3. REPOSITORIES
// =======================================================================
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationTemplateRepository, NotificationTemplateRepository>();
builder.Services.AddScoped<ISettingsRepository, SettingsRepository>();
builder.Services.AddScoped<IScheduledJobRepository, ScheduledJobRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// =======================================================================
// 4. SERVICES (Business Logic)
// =======================================================================
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<SchedulerService>();
builder.Services.AddScoped<TemplateRendererService>();

// =======================================================================
// 5. BACKGROUND WORKER
// =======================================================================
builder.Services.AddHostedService<NotificationWorker>();

// =======================================================================
// 6. CONTROLLERS / SWAGGER
// =======================================================================
builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.PropertyNamingPolicy = null;
        opt.JsonSerializerOptions.DictionaryKeyPolicy = null;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// =======================================================================
// 7. CORS (Allow frontend)
// =======================================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        b => b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console() // optional: also log to console
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// =======================================================================
// 8. BUILD APP
// =======================================================================
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.Run();
