using dentistAi_api.Controllers;
using dentistAi_api.Data;
using dentistAi_api.Services;
using MongoDB.Driver;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json").Build();
builder.Services.AddScoped<MongoDbContext>(sp =>
    new MongoDbContext(builder.Configuration.GetConnectionString("MongoDb"), "DentistAi"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.AllowAnyOrigin()// Replace with your Angular app's URL
                   .AllowAnyHeader()
                   .AllowAnyMethod();
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
});



var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowSpecificOrigin");
app.Use(async (context, next) =>
{
    var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
    var logger = loggerFactory.CreateLogger("CORSMiddleware");
    logger.LogInformation($"CORS middleware hit. Request path: {context.Request.Path}");
    await next();
});


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
