using Cysharp.Threading.Tasks;

namespace Initialization
{
    public interface IGameEntry
    {
        UniTask InitializeAsync();
    }
}