using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Objects;

namespace Projector
{
    public class EventProcessor : IProcessor
    {
        public string ConnectionString = "ConnectTo=tcp://admin:changeit@localhost:1113; HeartBeatTimeout=500";
        private readonly IEventStoreConnection _connection;

        public EventProcessor()
        {
            _connection = EventStoreConnection.Create(ConnectionString);
            _connection.ConnectAsync().Wait();
        }

        public async Task<bool> SendCommand<TAggregate, TCommand>(TCommand command)
            where TAggregate : Aggregate, new()
            where TCommand : ICommandEvent
        {
            var agg = await GetAggregate<TAggregate>(command.Id).ConfigureAwait(false);

            var handler = agg as IHandleCommand<TCommand>;

            if (handler == null)
            {
                throw new Exception("Unable to apply command to this aggregate");
            }

            var newEvents = handler.Handle(command);

            foreach (var evdo in newEvents)
            {
                var serial = new JsonSerializer();
                var writer = new JTokenWriter();
                serial.Serialize(writer, evdo);
                var s = JsonConvert.SerializeObject(evdo);

                var data = new EventData(Guid.NewGuid(), evdo.GetType().AssemblyQualifiedName, true, Encoding.ASCII.GetBytes(s), null);

                await _connection.AppendToStreamAsync(command.Id.ToString(), agg.EventsLoaded - 1, data).ConfigureAwait(false);
            }

            return true;
        }

        public async Task<TAggregate> GetAggregate<TAggregate>(Guid id)
            where TAggregate : Aggregate, new()
        {
            var agg = new TAggregate();
            

            var eventsSlice = await
                   _connection.ReadStreamEventsForwardAsync(id.ToString(), 0, 100, true,
                       new UserCredentials("admin", "changeit")).ConfigureAwait(false);

            var events = new ConcurrentDictionary<int, object>();

            eventsSlice.Events.AsParallel().ForAll(ev =>
            {
                var serializer = new JsonSerializer();
                var data = new MemoryStream(ev.Event.Data);
                var reader = new JsonTextReader(new StreamReader(data));
                var deserializedEvent = serializer.GetType().GetMethods().First(e => e.Name == "Deserialize" && e.IsGenericMethod)
                    .MakeGenericMethod(Type.GetType(ev.Event.EventType))
                    .Invoke(serializer, new object[] { reader });
                events.AddOrUpdate(ev.OriginalEventNumber, o => deserializedEvent, (i, o) => deserializedEvent);
            });

            var sorted = (from ev in events
                          orderby ev.Key ascending
                          select ev.Value).ToList();

            Debug.Assert(agg != null, "Aggregate cannot be null");

            agg.ApplyEvents(sorted);
            return agg;
        }
    }
}
