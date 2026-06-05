using TodoList.Infrastructure;
using TodoList.Application.Services;
using Microsoft.OpenApi;
using TodoList.Application.Mappings;
using TodoList.API.Middlewares;
using TodoList.API.Validation.Auth;
using TodoList.API.Validation.Task;
using TodoList.API.Validation.Category;
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<UserProfile>();
}, AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddControllers();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateTaskRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateCategoryRequestValidator>();

// Configure CORS using an environment variable for better security.
var frontendUrl = builder.Configuration["FrontendUrl"] ?? "http://localhost:3000";

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(frontendUrl)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Configure Swagger/OpenAPI for interactive documentation.
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "API 2026", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        In = ParameterLocation.Header,
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement((doc) => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", doc),
            new List<string>()
        }
    });
});

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
        c.EnablePersistAuthorization();
    });

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Apply pending migrations at startup (skipped during integration testing).
if (!app.Environment.IsEnvironment("IntegrationTesting"))
{
    await app.Services.InitializeDatabaseAsync();
}

app.Run();