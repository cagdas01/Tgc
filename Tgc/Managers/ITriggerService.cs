using System.Threading.Tasks;

namespace Tgc.Managers;

public interface ITriggerService
{
    Task InitializeAndRun();
}
