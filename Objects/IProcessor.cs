using System;
using System.Threading.Tasks;

namespace Objects
{
    public interface IProcessor
    {
        Task<bool> SendCommand<TAggregate, TCommand>(TCommand command)
            where TAggregate : Aggregate, new()
            where TCommand : ICommandEvent;

        Task<TAggregate> GetAggregate<TAggregate>(Guid id)
            where TAggregate : Aggregate, new();
    }
}