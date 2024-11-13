using Scf;
using Scf.Domain;
using Scf.Domain.Services;
using Scf.Utility;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddHttpContextAccessor();


builder.Services.AddControllers(options =>
{
    options.Filters.Add<ExceptionFilter>();
}).AddNewtonsoftJson(x =>
{
    x.SerializerSettings.Converters.Add(new Scf.Utility.JsonConverters.ObjectIdJsonConverter());
    x.SerializerSettings.Converters.Add(new Scf.Utility.JsonConverters.ObjectIdListJsonConverter());
    x.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.Configure<Configuration>(builder.Configuration.GetSection("Configuration"));
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

builder.Services.AddMvc(config => config.ModelBinderProviders.Insert(0, new ObjectIdBinderProvider()));

builder.Services.AddSingleton<IMongoClientService, MongoClientService>();

builder.Services.AddScoped<IHttpContextService, HttpContextService>();
builder.Services.AddScoped<IMongoDbService, MongoDbService>();
builder.Services.AddScoped<DomainContext>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<ILoggerService, LoggerService>();
builder.Services.AddScoped<ILanguageService, LanguageService>();

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<EmployeeService>();
builder.Services.AddScoped<PositionService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<GroupService>();
builder.Services.AddScoped<EmployeeTitleService>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(x => {
    x.AllowAnyHeader();
    x.AllowAnyMethod();
    x.AllowAnyOrigin();
    //var builder = new CorsPolicyBuilder();
    //builder.AllowAnyOrigin();
    //return builder;
});

app.UseHttpsRedirection();
app.MapControllers();

app.Run();