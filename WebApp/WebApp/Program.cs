using Blazored.LocalStorage;
using Blazr.RenderState.Server;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebApp.Authentication;
using WebApp.Autorization;
using WebApp.Components;
using WebApp.Data;
using WebApp.Services;
using WebApp.Shared;
using WebApp.Shared.DTO;
using WebApp.Shared.Model;
using WebApp.Shared.Services;
using WebApp.Validators;

var builder = WebApplication.CreateBuilder(args);
//Cross origin policy
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy(name: "CORS",
//        policy =>
//        {
//            policy.AllowAnyOrigin()
//            .AllowAnyHeader()
//            .AllowAnyMethod();
//        });
//});

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents()
	.AddInteractiveWebAssemblyComponents();

builder.AddBlazrRenderStateServerServices();

builder.Services.AddLocalization();
builder.Services.AddBlazoredLocalStorage();

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

builder.Services.AddSwaggerGen();
builder.Services.AddSweetAlert2();

builder.Services.AddAntiforgery();
builder.Services.AddHttpClient();

builder.Services.AddDbContext<DataContext>(options =>
	options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

// Adding Authentication
builder.Services.AddAuthentication(x => {
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
    {
        options.SaveToken = true;
        //options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidIssuer = builder.Configuration["AppSettings:Issuer"],
            ValidAudience = builder.Configuration["AppSettings:Audience"],
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Secret"])),
            ValidateIssuerSigningKey = true,
        };
    });

//AES encryption settings
AESUtils.KEY = builder.Configuration["AppSettings:AESKEY"];
AESUtils.IV = builder.Configuration["AppSettings:AESIV"];


builder.Services.AddAuthorization();
//builder.Services.AddAuthorization(options =>
//{
//	options.AddPolicy("RequireLoggedIn", policy =>
//		policy.RequireAuthenticatedUser());
//});

builder.Services.AddControllersWithViews();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<JWTTokenService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJWTUtils, JWTUtils>();
builder.Services.AddScoped<IUserService, UserService>();


//Validators
builder.Services.AddScoped<IValidator<LoginModel>, LoginModelValidator>();
builder.Services.AddScoped<IValidator<RegisterModel>, RegisterModelValidator>();
builder.Services.AddScoped<IValidator<User>, UserValidator>();
builder.Services.AddScoped<IValidator<PasswordChangeModel>, PasswordChangeModelValidator>();

builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseCors();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//var antiforgery = app.Services.GetRequiredService<IAntiforgery>();
//app.Use((context, next) =>
//{
//    var requestPath = context.Request.Path.Value;

//    if (string.Equals(requestPath, "/", StringComparison.OrdinalIgnoreCase)
//        || string.Equals(requestPath, "/index.html", StringComparison.OrdinalIgnoreCase))
//    {
//        var tokenSet = antiforgery.GetAndStoreTokens(context);
//        context.Response.Cookies.Append("XSRF-TOKEN", tokenSet.RequestToken!,
//            new CookieOptions { HttpOnly = false });
//    }

//    return next(context);
//});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode();

app.Run();