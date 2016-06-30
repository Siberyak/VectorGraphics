using System;
using System.Collections.Generic;
using System.Linq;

namespace WindowsFormsApplication1
{
    public static class TagsStorage
    {
        static readonly Dictionary<Type, Dictionary<object, Dictionary<object, object>>> _tags = new Dictionary<Type, Dictionary<object, Dictionary<object, object>>>();

        public static object Tag<TTaggedObject, TKey>(this TTaggedObject obj, TKey key)
        {
            var dictionary = TagsByKey(obj);
            if (!dictionary.ContainsKey(key))
                return null;

            var value = dictionary[key];
            return value;
        }

        public static TValue As<TValue>(this object value)
        {
            return value is TValue ? (TValue)value : default(TValue);
        }

        public static void Tag<TTaggedObject, TKey, TValue>(this TTaggedObject obj, TKey key, TValue value)
        {
            var dictionary = TagsByKey(obj);
            dictionary[key] = value;
        }

        public static void RemoveTag<TTaggedObject, TKey>(this TTaggedObject obj, TKey key)
        {
            var dictionary = TagsByKey(obj);
            if (dictionary.ContainsKey(key))
                dictionary.Remove(key);
        }

        public static void ClearTags<TTaggedObject>(this TTaggedObject obj)
        {
            var byObj = Tags(obj.GetType());
            if (byObj.ContainsKey(obj))
                byObj.Remove(obj);
        }

        public static Dictionary<object, object> TagsByKey<TTaggedObject>(this TTaggedObject obj)
        {
            var byObj = Tags(obj.GetType());

            if (!byObj.ContainsKey(obj))
                byObj.Add(obj, new Dictionary<object, object>());

            return byObj[obj];
        }

        public static Dictionary<TKey, object> OfKeyType<TKey>(this Dictionary<object, object> dictionary)
        {
            return dictionary.Keys.OfType<TKey>().ToDictionary(x => x, x => dictionary[x]);
        }

        public static Dictionary<object, TValue> OfValueType<TValue>(this Dictionary<object, object> dictionary)
        {
            return dictionary.Where(x => x.Value is TValue).ToDictionary(x => x.Key, x => (TValue)x.Value);
        }

        public static Dictionary<TKey, TValue> OfKeyValueTypes<TKey, TValue>(this Dictionary<object, object> dictionary)
        {
            return dictionary.OfType<KeyValuePair<TKey, TValue>>().ToDictionary(x => x.Key, x => x.Value);
        }

        private static Dictionary<object, Dictionary<object, object>> Tags(Type type)
        {
            if (!_tags.ContainsKey(type))
                _tags.Add(type, new Dictionary<object, Dictionary<object, object>>());

            var byObj = _tags[type];
            return byObj;
        }
    }
}