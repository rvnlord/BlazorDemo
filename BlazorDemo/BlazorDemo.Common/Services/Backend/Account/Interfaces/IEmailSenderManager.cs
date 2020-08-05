using System.Threading.Tasks;
using BlazorDemo.Common.Models.Interfaces;

namespace BlazorDemo.Common.Services.Backend.Account.Interfaces
{
    public interface IEmailSenderManager
    {
        Task<IApiResponse> SendConfirmationEmailAsync(string email, string code, string returnUrl);
        Task<IApiResponse> SendPasswordResetEmailAsync(string email, string code, string returnUrl);
    }
}
