using System;

namespace Navi.Util
{
    internal class AnonymousObservable<T> : IObservable<T>
    {
        private Func<IObserver<T>, IDisposable> subscribeAction;

        public AnonymousObservable(Func<IObserver<T>, IDisposable> subscribeAction)
        {
            this.subscribeAction = subscribeAction;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return subscribeAction(observer);
        }
    }
}
