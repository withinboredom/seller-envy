using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Objects
{
    public interface IRequire
    {
        Task Initialize(IProcessor processor);
    }
}