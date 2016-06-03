using System;
using System.Reactive.Subjects;
using Adaptive.ReactiveTrader.Client.Domain.Transport.Wamp;
using NUnit.Framework;

namespace Adaptive.ReactiveTrader.Tests.ObservableExtensions
{
    public class Service
    {
        [SetUp]
        public void Setup()
        {
            _innerSubject = new Subject<ServiceInstanceStatus>();
            _outerSubject = new Subject<IObservable<ServiceInstanceStatus>>();
        }

        private ISubject<ServiceInstanceStatus> _innerSubject;
        private ISubject<IObservable<ServiceInstanceStatus>> _outerSubject;

        [Test]
        public void Blah()
        {
            var s = _outerSubject.ToLastValueObservableDictionary(o => o);

            s.GetServiceWithMinLoad().Subscribe(o => { Console.WriteLine(o);});

            var i = s.Subscribe(o => { });

            _outerSubject.OnNext(_innerSubject);
            _innerSubject.OnNext(ServiceInstanceStatus.CreateForConnected("price", "a", 0));
            _innerSubject.OnNext(ServiceInstanceStatus.CreateForDisconnected("price", "a"));

            i.Dispose();
        }
    }
}
