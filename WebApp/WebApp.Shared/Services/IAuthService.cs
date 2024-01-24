using WebApp.Shared.DTO;

namespace WebApp.Shared.Services
{
    public interface IAuthService
    {
        public Task<Response> RegisterAsync(RegisterModel model);
        public Task<Response<LoginResponse>> LoginAsync(LoginModel model);
        public Task<Response<LoginResponse>> Authenticate(LoginModel model);
        
        public Task Logout();
    }
}
