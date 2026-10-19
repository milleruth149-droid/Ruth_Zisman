using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MuniServices.Core.Interfaces;
using MuniServices.Infrastructure.Clients;
using MuniServices.Infrastructure.Repositories;
using MuniServices.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularClient", policy =>
    {
        policy
            .WithOrigins(
                "https://ominous-capybara-4j57rq97xjg4fw9g-4200.app.github.dev",
                "http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// מאפשר לאפליקציית Angular מהכתובות שהוגדרו לשלוח בקשות לשרת.

builder.Services.AddScoped<IRequestService, RequestService>();
builder.Services.AddScoped<IFormsClient, MockFormsClient>();
builder.Services.AddScoped<
    IServiceCatalogRepository,
    MockServiceCatalogRepository>();

// קורא מההגדרות את פרטי אימות הטוקן
var issuer = builder.Configuration["Authentication:Issuer"]
    ?? throw new InvalidOperationException(
        "Missing Authentication:Issuer");

var audience = builder.Configuration["Authentication:Audience"]
    ?? throw new InvalidOperationException(
        "Missing Authentication:Audience");

var signingKeyValue = builder.Configuration["Authentication:SigningKey"]
    ?? throw new InvalidOperationException(
        "Missing Authentication:SigningKey");



// // המפתח נשמר כטקסט Base64 בהגדרות ומומר לבתים לצורך אימות חתימת ה-JWT.
var signingKey = Convert.FromBase64String(signingKeyValue);

// מגדיר איך השרת בודק שהטוקן תקין ולא פג תוקפו.
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // שומר על שמות ה-claims כפי שהם מופיעים בטוקן,
        // למשל idNumber.
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,

            ValidateAudience = true,
            ValidAudience = audience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(signingKey),

            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// הוספת Swagger ו-Swagger UI בסביבת פיתוח
if (app.Environment.IsDevelopment())
{
   
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AngularClient");
// תחילה מזהים את המשתמש לפי הטוקן, ואז בודקים אם הוא מורשה לגשת לנתיב.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();