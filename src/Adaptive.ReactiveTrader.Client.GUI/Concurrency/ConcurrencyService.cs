using System.Reactive.Concurrency;
using System.Windows;

namespace Adaptive.ReactiveTrader.Client.Concurrency
{
    public sealed class ConcurrencyService : IConcurrencyService
    {
        public IScheduler Dispatcher
        {
            get { return new DispatcherScheduler(Application.Current.Dispatcher); }
        }

        public IScheduler TaskPool
        {
            get { return ThreadPoolScheduler.Instance; }
        }

    }
}