using BACKBONE.API.Middleware;
using BACKBONE.API.Services;
using BACKBONE.Application.Interfaces;
using BACKBONE.Infrastructure;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Globalization;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add logging
builder.Services.AddLogging(loggingBuilder => {
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();
});

// Add your service extensions here
builder.Services.AddDataServices(builder.Configuration);

builder.Services.AddScoped<IGatepassRepository, GatepassRepository>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Add Security and XML Comments
builder.Services.AddSwaggerGen(c => {
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme, e.g. \"Bearer token\"",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    c.OperationFilter<SecurityRequirementsOperationFilter>();
    
    // Add XML Comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Add the JWT Middleware before authentication
app.UseMiddleware<JwtMiddleware>(); 

app.UseAuthentication();
// Serve static files from UserImages folder

var imageFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "UserImages");
if (!Directory.Exists(imageFolderPath))
{
    Directory.CreateDirectory(imageFolderPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(imageFolderPath),
    RequestPath = "/UserImages"
});

var feedbackImagesFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "FeedbackImages");
if (!Directory.Exists(feedbackImagesFolderPath))
{
    Directory.CreateDirectory(feedbackImagesFolderPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(feedbackImagesFolderPath),
    RequestPath = "/FeedbackImages"
});

var supportedCultures = new[] { new CultureInfo("en-US") };

var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en-US"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};

app.UseRequestLocalization(localizationOptions);

app.UseAuthorization();

app.MapControllers();

app.Run();