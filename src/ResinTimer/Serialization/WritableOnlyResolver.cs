using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ResinTimer.Serialization
{
    /// <summary>
    /// get-only property に対してシリアル化が必須であることをマークします。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SerializationRequiredAttribute : Attribute
    {
    }

    public class WritableOnlyResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            => base.CreateProperties(type, memberSerialization)
                .Where(p => p.Writable || (p.AttributeProvider?.GetAttributes(typeof(SerializationRequiredAttribute), true).Any() ?? false))
                .ToList();
    }
}
