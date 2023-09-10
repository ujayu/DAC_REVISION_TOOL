using apidotnet.Helper;
using apidotnet.Service.Class;
using apidotnet.Service.Interface;
using AutoMapper;
using dotnetapi.Helper;
using dotnetapi.Service.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RevisionTool.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Controller register
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IModuleService, ModuleService>();
builder.Services.AddTransient<ITopicService, TopicService>();
builder.Services.AddTransient<IPointService, PointService>();


//Register database credential
builder.Services.AddMvc(option => option.EnableEndpointRouting = false)
    .AddNewtonsoftJson(opt => opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

builder.Services.AddDbContext<RevisionToolContext>(
options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("revisionTool"),
    Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.31-mysql"));
});

//Jwt register
var _authkey = builder.Configuration.GetValue<string>("JwtSettings:securitykey");
builder.Services.AddAuthentication(item =>
{
    item.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    item.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(item =>
{
    item.RequireHttpsMetadata = true;
    item.SaveToken = true;
    item.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authkey)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };

});
builder.Services.AddControllers();

//Auto mapper register
var automapper = new MapperConfiguration(config =>
{
    config.AddProfile(new UserRegisterAutoMapperHandler());
    config.AddProfile(new UserUserInfoAutoMapperHandler()); // Add this line
});
IMapper mapper = automapper.CreateMapper();
builder.Services.AddSingleton(mapper);

//Cros register
builder.Services.AddCors(p => p.AddDefaultPolicy(build =>
{
    build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

//Jwt register
var _jwtsetting = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(_jwtsetting);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
