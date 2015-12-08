using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Objects
{
    public class Aggregate
    {
        public int EventsLoaded { get; private set; }

        public Guid Id { get; internal set; }

        public void ApplyEvents(IEnumerable events)
        {
            foreach (var e in events)
                GetType().GetMethod("ApplyOneEvent")
                    .MakeGenericMethod(e.GetType())
                    .Invoke(this, new object[] { e });
        }

        public void ApplyOneEvent<TEvent>(TEvent ev)
        {
            var applier = this as IApplyEvent<TEvent>;
            if (applier == null)
            {
                throw new InvalidOperationException(
                    $"Aggregate {GetType().Name} does not know how to apply event {ev.GetType().Name}");
            }
            else
            {
                applier.Apply(ev);
                EventsLoaded++;
            }
        }
    }
}
