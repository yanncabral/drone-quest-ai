using Core.Agent;

namespace Core;

public interface IStoreDispatcher
{
    public void Dispatch(AgentStore store);
}