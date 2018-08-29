using System;

namespace ConsoleApp
{
    class Observer<T>
    {
        private Action<T> onNext;

        public Observer(Action<T> onNext) => this.onNext = onNext;
        public void OnNext(T v) => this.onNext.Invoke(v);
    }

    class Observable<T>
    {
        private Action<Observer<T>> subscribe;

        public Observable(Action<Observer<T>> subscribe) => this.subscribe = subscribe;
        public void Subscribe(Observer<T> observer) => this.subscribe(observer);
    }

    static class Extensions
    {
        public static void Subscribe<T>(this Observable<T> observable, Action<T> subscribe) =>
            observable.Subscribe(new Observer<T>(v => subscribe(v)));

        public static Observable<U> Select<T, U>(this Observable<T> observable, Func<T, U> select)
        {
            return new Observable<U>(o =>
            {
                observable.Subscribe(v =>
                {
                    o.OnNext(select(v));
                });
            });
        }

        public static Observable<T> Where<T>(this Observable<T> observable, Func<T, bool> where)
        {
            return new Observable<T>(o =>
            {
                observable.Subscribe(v =>
                {
                    if (where(v))
                    {
                        o.OnNext(v);
                    }
                });
            });
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Observer<string> source = null;
            var root = new Observable<string>(o => source = o);

            root
                .Select(s => s + s)
                .Where(s => s != "ふがふが")
                .Subscribe(s => Console.WriteLine($"値:{s}"));

            Console.WriteLine("値を送信します");
            source.OnNext("てすと");
            Console.WriteLine("値を送信します(2回目)");
            source.OnNext("ふが");
            Console.WriteLine("値を送信します(3回目)");
            source.OnNext("ほげほげ");
        }
    }
}
