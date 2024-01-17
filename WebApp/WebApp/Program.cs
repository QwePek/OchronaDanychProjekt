using Blazored.LocalStorage;
using Blazr.RenderState.Server;
using Bogus;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebApp.Components;
using WebApp.Data;
using WebApp.Services;
using WebApp.Shared.Services;

var builder = WebApplication.CreateBuilder(args);
//Cross origin policy
builder.Services.AddCors(options => {
    options.AddPolicy(name: "CORS",
        policy => {
            policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents()
	.AddInteractiveWebAssemblyComponents();

builder.AddBlazrRenderStateServerServices();

builder.Services.AddLocalization();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllersWithViews();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddSweetAlert2();

builder.Services.AddDbContext<DataContext>(options =>
	options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
           .AddCookie(options =>
           {
               options.LoginPath = "/Login/Login";
               options.AccessDeniedPath = "/Register";
           });

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ILoginService, LoginService>();

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

app.MapControllers();
app.UseRouting();
app.MapControllers();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode();

app.Run();