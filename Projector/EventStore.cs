using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Data.Common;
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
    public class EventStoreStuff
    {
        public string ConnectionString = "ConnectTo=tcp://admin:changeit@localhost:1113; HeartBeatTimeout=500";
        private IEventStoreConnection _connection;

        public EventStoreStuff()
        {
            _connection = EventStoreConnection.Create(ConnectionString);
            _connection.ConnectAsync().Wait();
        }

        public async Task SendCommand<TAggregate, TCommand>(TCommand command)
            where TAggregate : Aggregate, new()
            where TCommand : ICommand
        {
            var agg = await GetAggregate<TAggregate>(command.Id);

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
                string s = JsonConvert.SerializeObject(evdo);

                var data = new EventData(Guid.NewGuid(), evdo.GetType().AssemblyQualifiedName, true, System.Text.Encoding.ASCII.GetBytes(s), null);

                await _connection.AppendToStreamAsync(command.Id.ToString(), agg.EventsLoaded - 1, data);
            }
        }

        public async Task<TAggregate> GetAggregate<TAggregate>(Guid Id)
            where TAggregate : Aggregate, new()
        {
            var agg = new TAggregate();

            var eventsSlice = await
                   _connection.ReadStreamEventsForwardAsync(Id.ToString(), 0, 100, true,
                       new UserCredentials("admin", "changeit"));

            var events = new ConcurrentDictionary<int, object>();

            eventsSlice.Events.AsParallel().ForAll(ev =>
            {
                var serializer = new JsonSerializer();
                var data = new MemoryStream(ev.Event.Data);
                var reader = new JsonTextReader(new StreamReader(data));
                var deserializedEvent = serializer.GetType().GetMethods().First(e => e.Name == "Deserialize" && e.IsGenericMethod == true)
                    .MakeGenericMethod(Type.GetType(ev.Event.EventType))
                    .Invoke(serializer, new object[] { reader });
                events.AddOrUpdate(ev.OriginalEventNumber, o => deserializedEvent, (i, o) => deserializedEvent);
            });

            var sorted = (from ev in events
                          orderby ev.Key ascending
                          select ev.Value).ToList();

            agg.ApplyEvents(sorted);
            return agg;
        }
    }
}
