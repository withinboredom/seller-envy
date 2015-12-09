using System.Collections;

namespace Objects
{
    /// <summary>
    /// Subscriptions handle events like commands
    /// </summary>
    /// <typeparam name="TEvent">The event to handle</typeparam>
    public interface ISubscribeTo<in TEvent>
    {
        IEnumerable HandleExternalEvent(TEvent e);
    }
}