using Blazr.RenderState.WASM;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebApp.Client.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.AddBlazrRenderStateWASMServices();

builder.Services.AddLocalization();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)});

//builder.Services.AddTransient(sp => new HttpClient { 
//    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) 
//}.AddHttpMessageHandler<AuthorizedHandler>());

builder.Services.AddOptions();

await builder.Build().RunAsync();