using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Newtonsoft.Json
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class JsonMergeKeyAttribute : Attribute
    {
    }

    public class KeyedIListMergeConverter : JsonConverter
    {
        readonly IContractResolver contractResolver;

        public KeyedIListMergeConverter(IContractResolver contractResolver)
        {
            this.contractResolver = contractResolver ?? throw new ArgumentNullException(nameof(contractResolver));
        }

        public static bool CanConvert(IContractResolver contractResolver, Type objectType, out Type? elementType, out JsonProperty? keyProperty)
        {
            if (objectType.IsArray)
            {
                // Not implemented for arrays, since they cannot be resized.
                elementType = null;
                keyProperty = null;
                return false;
            }
            var elementTypes = objectType.GetIListItemTypes().ToList();
            if (elementTypes.Count != 1)
            {
                elementType = null;
                keyProperty = null;
                return false;
            }
            elementType = elementTypes[0];
            if (contractResolver.ResolveContract(elementType) is not JsonObjectContract contract)
            {
                keyProperty = null;
                return false;
            }
            keyProperty = contract.Properties.SingleOrDefault(p => p.AttributeProvider?.GetAttributes(typeof(JsonMergeKeyAttribute), true).Count > 0);
            return keyProperty != null;
        }

        public override bool CanConvert(Type objectType)
        {
            return CanConvert(contractResolver, objectType, out _, out _);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (contractResolver != serializer.ContractResolver)
                throw new InvalidOperationException("Inconsistent contract resolvers");

            if (!CanConvert(contractResolver, objectType, out Type? elementType, out JsonProperty? keyProperty))
                throw new JsonSerializationException(string.Format("Invalid input type {0}", objectType));

            if (reader.TokenType == JsonToken.Null)
                return existingValue;

            if (elementType is null)    // Bu warning için yaptım.
                throw new NullReferenceException(nameof(elementType));

            var method = GetType().GetMethod("ReadJsonGeneric", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            var genericMethod = method?.MakeGenericMethod(new[] { elementType });
            try
            {
                return genericMethod?.Invoke(this, new object?[] { reader, objectType, existingValue, serializer, keyProperty });
            }
            catch (TargetInvocationException ex)
            {
                // Wrap the TargetInvocationException in a JsonSerializationException
                throw new JsonSerializationException("ReadJsonGeneric<T> error", ex);
            }
        }


        public object ReadJsonGeneric<T>(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer, JsonProperty keyProperty)
        {
            if (objectType == null)
                throw new ArgumentNullException(nameof(objectType));

            var list = existingValue as IList<T>;
            if (list == null || list.Count == 0)
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8604 // Dereference of a possibly null reference.
                list ??= contractResolver.ResolveContract(objectType)?.DefaultCreator() as IList<T>;
                serializer.Populate(reader, target: list);
            }
            else
            {
                var jArray = JArray.Load(reader);
                var comparer = new KeyedListMergeComparer();
                var lookup = jArray.ToLookup(i => i[keyProperty.PropertyName]?.ToObject(keyProperty.PropertyType, serializer), comparer);
                var done = new HashSet<JToken>();

                var silinecek = list.Where(x => !lookup[keyProperty.ValueProvider.GetValue(x)].Any());
                silinecek.ToList().ForEach(x => list.Remove(x));

                // Populate existing items
                foreach (var item in list)
                {
                    var key = keyProperty.ValueProvider.GetValue(item);
                    var replacement = lookup[key].FirstOrDefault(v => !done.Contains(v));
                    if (replacement != null)
                    {
                        using (var subReader = replacement.CreateReader())
                            serializer.Populate(subReader, item);
                        done.Add(replacement);
                    }
                }
                // Populate the NEW items into the list.
                //if (done.Count < jArray.Count)
                foreach (var item in jArray.Where(i => !done.Contains(i)))
                {
                    list.Add(item.ToObject<T>(serializer));
                }
            }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8604 // Dereference of a possibly null reference.
            return list;
        }

        public override bool CanWrite { get { return false; } }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public class KeyedListMergeComparer : IEqualityComparer<object?>
        {
            #region IEqualityComparer<object> Members

            bool IEqualityComparer<object?>.Equals(object? x, object? y)
            {
                return object.Equals(x, y);
            }

            int IEqualityComparer<object?>.GetHashCode(object obj)
            {
                if (obj == null)
                    return 0;
                return obj.GetHashCode();
            }

            #endregion
        }
    }


    internal static class TypeExtensions
    {
        public static IEnumerable<Type> GetInterfacesAndSelf(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (type.IsInterface)
                return new[] { type }.Concat(type.GetInterfaces());
            else
                return type.GetInterfaces();
        }

        public static IEnumerable<Type> GetIListItemTypes(this Type type)
        {
            foreach (Type intType in type.GetInterfacesAndSelf())
            {
                if (intType.IsGenericType
                    && intType.GetGenericTypeDefinition() == typeof(IList<>))
                {
                    yield return intType.GetGenericArguments()[0];
                }
            }
        }
    }


    internal class DeserializeContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);

            properties =
                properties.Where(p => p.Writable).ToList();

            return properties;
        }
    }
}
