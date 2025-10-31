using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProjectManagerApi.Data;
using ProjectManagerApi.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ============================================================================
// 🔹 SERVICES CONFIGURATION
// ============================================================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ============================================================================
// 🔹 SWAGGER CONFIGURATION
// ============================================================================
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Project Manager API",
        Version = "v1",
        Description = "A minimal project management API with JWT authentication."
    });

    // ✅ JWT Support for Swagger UI
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// ============================================================================
// 🔹 CORS (Frontend on Vite + Deployed Frontend on Vercel)
// ============================================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});


// ============================================================================
// 🔹 JWT CONFIGURATION
// ============================================================================
builder.Configuration["Jwt:Key"] ??= "ReplaceWithASecretKeyOfYourChoice_AtLeast_32_Chars";
builder.Configuration["Jwt:Issuer"] ??= "ProjectManagerApi";
builder.Configuration["Jwt:Audience"] ??= "ProjectManagerApiAudience";

var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // ✅ Disable only in dev
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateLifetime = true
    };
});

// ============================================================================
// 🔹 DATABASE CONFIGURATION (SQLite)
// ============================================================================
var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? "Data Source=projectmanager.db";
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(connectionString));

// ============================================================================
// 🔹 DEPENDENCY INJECTION (Custom Services)
// ============================================================================
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ISchedulerService, SchedulerService>();
builder.Services.AddSingleton<IPasswordHasher<ProjectManagerApi.Models.User>, PasswordHasher<ProjectManagerApi.Models.User>>();

// ============================================================================
// 🔹 BUILD THE APPLICATION
// ============================================================================
var app = builder.Build();

// ============================================================================
// 🔹 DATABASE INITIALIZATION
// ============================================================================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// ============================================================================
// 🔹 MIDDLEWARE PIPELINE
// ============================================================================
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Detailed errors in dev
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
