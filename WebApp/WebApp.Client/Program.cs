using Blazr.RenderState.WASM;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebApp.Shared.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.AddBlazrRenderStateWASMServices();

builder.Services.AddLocalization();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();

await builder.Build().RunAsync();