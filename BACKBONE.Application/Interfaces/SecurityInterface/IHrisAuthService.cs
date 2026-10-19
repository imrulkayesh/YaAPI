using System.Threading.Tasks;

namespace BACKBONE.Application.Interfaces.SecurityInterface
{
    public interface IHrisAuthService
    {
        Task<string> Login(string username, string password);
    }
}