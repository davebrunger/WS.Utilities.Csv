using System;

namespace WS.Utilities.Csv
{
    public abstract class Option<T> : IEquatable<Option<T>>
    {
        public abstract bool IsNone { get; }
        public abstract void Match(Action<T> some, Action none);
        public abstract TResult Match<TResult>(Func<T, TResult> some, Func<TResult> none);
        public abstract bool Equals(Option<T> other);
        public abstract int DoGetHashCode();

        public static Option<T> None { get; } = new NoneOption();

        public static Option<T> Some(T value) => new SomeOption(value);

        public static bool operator ==(Option<T> option1, Option<T> option2)
        {
            return (object)option1 != null ? option1.Equals(option2) : (object)option2 == null;
        }

        public static bool operator !=(Option<T> option1, Option<T> option2)
        {
            return (object)option1 != null ? !option1.Equals(option2) : (object)option2 != null;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Option<T>);
        }

        public override int GetHashCode()
        {
            return DoGetHashCode();
        }

        private sealed class SomeOption : Option<T>
        {
            private readonly T _instance;
            public override bool IsNone => false;

            public SomeOption(T instance)
            {
                _instance = instance;
            }
       
            public override void Match(Action<T> some, Action none)
            {
                some(_instance);
            }

            public override TResult Match<TResult>(Func<T, TResult> some, Func<TResult> none)
            {
                return some(_instance);
            }

            public override bool Equals(Option<T> other)
            {
                var that = other as SomeOption;
                if ((object)that == null)
                {
                    return false;
                }
                else
                {
                    return _instance != null ? _instance.Equals(that._instance) : that._instance == null;
                }
            }

            public override int DoGetHashCode()
            {
                return _instance.GetHashCode();
            }

            public override string ToString()
            {
                return $"Some({_instance})";
            }
        }

        private sealed class NoneOption : Option<T>
        {
            public override bool IsNone => true;

            public override void Match(Action<T> some, Action none)
            {
                none();
            }

            public override TResult Match<TResult>(Func<T, TResult> some, Func<TResult> none)
            {
                return none();
            }

            public override bool Equals(Option<T> other)
            {
                var that = other as NoneOption;
                return (object)that != null;
            }

            public override int DoGetHashCode()
            {
                return 0;
            }

            public override string ToString()
            {
                return "None";
            }
        }
    }
}
