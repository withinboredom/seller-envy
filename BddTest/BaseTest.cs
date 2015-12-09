using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.Internal.Commands;
using Objects;

namespace BddTest
{
    public class BaseTest<TAggregate> where TAggregate : Aggregate, new()
    {
        private TAggregate _sut;

        [SetUp]
        public void BaseTestSetup()
        {
            _sut = new TAggregate();
        }

        protected void Test(IEnumerable given, Func<TAggregate, object> when, Action<object> then)
        {
            then(when(ApplyEvents(_sut, given)));
        }

        protected IEnumerable Given(params object[] events)
        {
            return events;
        }

        protected Func<TAggregate, object> When<TCommand>(TCommand command)
        {
            return agg =>
            {
                try
                {
                    return DispatchCommand(command).Cast<object>().ToArray();
                }
                catch (Exception e)
                {
                    return e;
                }
            };
        }

        protected Action<object> Then(params object[] expectedEvents)
        {
            return got =>
            {
                var gotEvents = got as object[];

                if (gotEvents != null)
                {
                    if (gotEvents.Length == expectedEvents.Length)
                    {
                        for (var i = 0; i < gotEvents.Length; i++)
                        {
                            if (gotEvents[i].GetType() == expectedEvents[i].GetType())
                            {
                                Assert.AreEqual(Serialize(expectedEvents[i]), Serialize(gotEvents[i]));
                            }
                            else
                            {
                                Assert.Fail(
                                    $"Incorrect event in results; expected a {expectedEvents[i].GetType().Name} but got a {gotEvents[i].GetType().Name}");
                            }
                        }
                    }
                    else if (gotEvents.Length < expectedEvents.Length)
                    {
                        Assert.Fail(
                            $"Expected event(s) missing: {string.Join(", ", EventDiff(expectedEvents, gotEvents))}");
                    }
                    else
                    {
                        Assert.Fail(
                            $"Unexpected event(s) emitted: {string.Join(", ", EventDiff(gotEvents, expectedEvents))}");
                    }
                }
                else if (got is CommandHandlerNotDefinedException)
                {
                    Assert.Fail(((Exception) got).Message);
                }
                else
                {
                    Assert.Fail("Expected events, but got exception {0}",
                        got.GetType().Name);
                }
            };
        }

        private static string[] EventDiff(IEnumerable<object> a, IEnumerable<object> b)
        {
            var diff = a.Select(e => e.GetType().Name).ToList();
            foreach (var remove in b.Select(e => e.GetType().Name))
            {
                diff.Remove(remove);
            }

            return diff.ToArray();
        }

        protected Action<object> ThenFailWithException<TException>()
        {
            return got =>
            {
                if (got is Exception)
                {
                    Assert.Pass("Got correct exception");
                }
                else if (got is CommandHandlerNotDefinedException)
                {
                    Assert.Fail((got as Exception).Message);
                }
                else
                {
                    Assert.Fail($"Expected exception {typeof (TException).Name}, but got event result");
                }
            };
        }

        private IEnumerable DispatchCommand<TCommand>(TCommand c)
        {
            var handler = _sut as IHandleCommand<TCommand>;

            if (handler == null)
            {
                var subscriber = _sut as ISubscribeTo<TCommand>;
                if (subscriber == null)
                {
                    throw new CommandHandlerNotDefinedException(
                        $"Aggregate {_sut.GetType().Name} does not yet handle command {c.GetType().Name}");
                }
                else
                {
                    return subscriber.HandleExternalEvent(c);
                }
            }
            else
            {
                return handler.Handle(c);
            }
        }

        private static TAggregate ApplyEvents(TAggregate agg, IEnumerable events)
        {
            agg.ApplyEvents(events);
            return agg;
        }

        private string Serialize(object o)
        {
            return JsonConvert.SerializeObject(o);
        }

        private class CommandHandlerNotDefinedException : Exception
        {
            public CommandHandlerNotDefinedException(string mesg) : base(mesg) { }
        }
    }
}
