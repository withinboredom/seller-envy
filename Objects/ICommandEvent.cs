using System;

namespace Objects
{
    public interface ICommandEvent
    {
        Guid Id { get; set; } 
    }
}