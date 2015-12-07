using System.Collections;
using System.Collections.Generic;

namespace Objects
{
    public interface IHandleCommand<in TCommand>
    {
        IEnumerable Handle(TCommand c);
    }
}