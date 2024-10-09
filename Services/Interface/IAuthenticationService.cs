using budget_api.Models.DTOs;
using budget_api.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace budget_api.Services.Interface
{
    public interface IAuthenticationService
    {
        Task<ActionResult<String?>?> AuthenticateAsync(AuthenticationRequestBodyDto authRequestBody);
        Task<ActionResult<User>> RegisterAsync(UserCreationDto user);
    }
}
