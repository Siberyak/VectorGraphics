using System;
using System.Collections.Generic;

namespace Shapes
{
    public class Variable<T>
    {
        static readonly Dictionary<object, Variable<T>> Variables = new Dictionary<object, Variable<T>>();

        public static Variable<T> Get<TKey>(TKey key)
        {
            return Variables.ContainsKey(key) ? Variables[key] : new Variable<T>(key, default(T));
        }

        private readonly object _key;

        public Variable()
            : this(Guid.NewGuid(), default(T))
        { }

        public Variable(object key, T initialValue)
        {
            _key = key;
            Variables.Add(key, this);

            Value = initialValue;
        }

        public T Value
        {
            get { return this.Tag(_key).As<T>(); }
            set { this.Tag(_key, value); }
        }
    }
}