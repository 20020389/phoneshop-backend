using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using PhoneShop.Service;

var builder = WebApplication.CreateBuilder(args);

// Add configs

// Add services to the container.

builder.Services.Configure<JsonOptions>(options =>
{
  options.SerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
      options.TokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuer = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
              Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"] ?? "")
          ),
      };
    });


builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<StoreService>();
builder.Services.AddSingleton<UploadService>();
builder.Services.AddSingleton<PhoneService>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

System.Console.WriteLine(Path.Combine(Directory.GetCurrentDirectory(), @"Upload"));

// app.UseStaticFiles(new StaticFileOptions()
// {
//   FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "Upload")),
//   RequestPath = "/api/cloud"
// });

// app.Use(
//     async (context, next) =>
//     {
//         System.Console.WriteLine("Before Controller");
//         await next(context);
//     }
// );

app.MapControllers();

app.Run();
