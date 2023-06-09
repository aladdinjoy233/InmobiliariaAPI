using InmobiliariaAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

//builder.WebHost.UseUrls("http://localhost:5000", "https://localhost:5001")//permite escuchar SOLO peticiones locales
builder.WebHost.UseUrls("http://localhost:5200", "http://*:5200"); //permite escuchar peticiones locales y remotas

// Add services to the container.
builder.Services.AddControllers();

// Para evitar el warning de possible null
string secretKey = configuration["TokenAuthentication:SecretKey"] ?? throw new ArgumentNullException(nameof(secretKey));
var signingKey = secretKey != null ? new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey)) : null;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = configuration["TokenAuthentication:Issuer"],
			ValidAudience = configuration["TokenAuthentication:Audience"],
			IssuerSigningKey = signingKey
		};

		options.Events = new JwtBearerEvents
		{
			OnMessageReceived = context =>
			{
				// Read token from query string
				var accessToken = context.Request.Query["access_token"];

				var path = context.HttpContext.Request.Path;
				// if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/propietarios/token"))
				if (!string.IsNullOrEmpty(accessToken))
				{
					context.Token = accessToken;
				}

				return Task.CompletedTask;
			}
		};
	});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DataContext>(
	options => options.UseMySql(
		configuration["ConnectionStrings:MySql"],
		ServerVersion.AutoDetect(configuration["ConnectionStrings:MySql"])
	)
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
