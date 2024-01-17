using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Globalization;

namespace WebApp.Client.Extensions
{
    public static class WebAssemblyHostExtensions
    {
        public static async Task InitializeCultureAsync(this WebAssemblyHost host)
        {
            CultureInfo culture;
            ILocalStorageService localStorageService = host.Services.GetService<ILocalStorageService>()!;
            string language = await localStorageService!.GetItemAsync<string>("Language");

            if (language != null)
            {
                culture = CultureInfo.GetCultureInfo(language);
            }
            else
            {
                culture = CultureInfo.GetCultureInfo("pl-PL");
                await localStorageService.SetItemAsStringAsync("Language", culture.Name);
            }

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }
    }
}
