using API_WEB_BIBLIOTECA.Models;
using API_WEB_BIBLIOTECA.Models.Custom;

namespace API_WEB_BIBLIOTECA.Services
{
    public interface IAutorizacionService
    {
        Task<AutorizacionResponse> DevolverToken(Usuario autorizacion);

    }
}
