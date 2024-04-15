
using System;
using System.Linq;

namespace EShop.BuildingBlocks.EventBus.EventBus.Extensions {
    public static class GenericTypeExtensions {
        public static string GetGenericTypeName(this Type type) {
            if (!type.IsGenericType) {
                return type.Name;
            }

            var genericTypes = string.Join(",", type.GetGenericArguments().Select(x => x.Name).ToArray());
            return $"{type.Name.Remove(type.Name.IndexOf('`'))}<{genericTypes}>";
        }

        public static string GetGenericTypeName(this object obj) {
            return obj.GetType().GetGenericTypeName();
        }
    }
}