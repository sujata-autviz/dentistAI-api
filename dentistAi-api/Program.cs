using dentistAi_api.Controllers;
using dentistAi_api.Data;
using dentistAi_api.Services;
using MongoDB.Driver;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using dentistAi_api.NotificationHub;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json").Build();
builder.Services.AddScoped<MongoDbContext>(sp =>
    new MongoDbContext(builder.Configuration.GetConnectionString("MongoDb"), "DentistAi"));

// Add services to the container.


// Configure CORS
// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder
            .WithOrigins("http://localhost:4200")  // Your Angular app's URL
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});
builder.Services.AddScoped<AuthenticationService>();

// Add IHttpContextAccessor
builder.Services.AddHttpContextAccessor();
var mongoClient = new MongoClient(configuration.GetConnectionString("MongoDb"));

builder.Services.AddSingleton<IMongoClient>(mongoClient);
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IToothService, ToothService>();
builder.Services.AddScoped<IPeriodontalChartService, PeriodontalChartService>();
builder.Services.AddScoped<ITranscriptLogsService, TranscriptLogsService>();

builder.Services.AddControllers().AddApplicationPart(typeof(SessionController).Assembly);
// JWT Authentication configuration


builder.Services.AddHttpClient<SpeechRecognitionService>();
builder.Services.AddScoped<TranscriptLogsService>();
// Ensure this line is present
builder.Services.AddScoped<SpeechRecognitionService>();
builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<NotificationService>();

var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:SecretKey"]);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };

    // Enable SignalR token authentication
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];

            // Check if the request is for the SignalR hub
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/signalr/notificationsHub"))
            {
                context.Token = accessToken; // Assign the token for validation
            }
            return Task.CompletedTask;
        }
    };
});



var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.Use(async (context, next) =>
{
    var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
    var logger = loggerFactory.CreateLogger("CORSMiddleware");
    logger.LogInformation($"CORS middleware hit. Request path: {context.Request.Path}");
    await next();
});

// Apply CORS policy
app.UseCors("CorsPolicy");


// Add routing middleware (must come before UseEndpoints)
app.UseRouting();

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Finally, call UseEndpoints (this is where the routes/endpoints are executed)


app.MapHub<NotificationsHub>("/signalr/notificationsHub");

app.Run();
