using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApp.Shared.Login;
using WebApp.Shared.Model;

namespace WebApp.Shared.Services
{
    public interface ILoginService
    {
        Task<Response<bool>> RegisterAsync(RegisterModel model);
        Task<Response<ClaimsPrincipal>> LoginAsync(LoginModel model);
    }
}
