﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Skarp.HubSpotClient.Extensions
{
    internal static class ReflectionExtensions
    {
        /// <summary>
        /// Returns the name of a given property either by name of <see cref="DataMemberAttribute"/>
        /// </summary>
        /// <remarks>
        /// If the <see cref="DataMemberAttribute"/> is defined it will try to use the prop name defined there. If no name is explictily defined in the attribute the 
        /// name of the actual property will be returned.
        /// </remarks>
        /// <param name="prop"></param>
        /// <returns></returns>
        internal static string GetPropSerializedName(this PropertyInfo prop)
        {
            if (prop == null) return null;

            var propName = prop.Name;

            var dataMemberAttr = prop.GetCustomAttribute<DataMemberAttribute>();
            if (dataMemberAttr == null) return propName;
            if (string.IsNullOrWhiteSpace(dataMemberAttr.Name)) return propName;

            return dataMemberAttr.Name;
        }

        /// <summary>
        /// Finds the method recursively by searching in the given type and all implemented interfaces.
        /// </summary>
        /// <param name="prop">The property.</param>
        /// <param name="name">The name.</param>
        /// <param name="typeArgs">The type arguments.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        internal static MethodInfo FindMethodRecursively(this Type prop, string name, params Type[] typeArgs)
        {
            if (prop == null) return null;

            // GetMethod searches base types...
            var method = prop.GetMethod(name, typeArgs);
            if (method != null) return method;


            foreach (var iface in prop.GetInterfaces())
            {
                method = iface.FindMethodRecursively(name, typeArgs);
                if (method != null) return method;
            }

            // TODO better bailout exception?
            throw new ArgumentException($"Unable to locate method with name: {name}", nameof(name));
        }

        /// <summary>
        /// Determines whether the given property has the <see cref="IgnoreDataMemberAttribute"/>
        /// </summary>
        /// <param name="prop">The property.</param>
        /// <returns>
        ///   <c>true</c> if [has ignore data member attribute] [the specified property]; otherwise, <c>false</c>.
        /// </returns>
        internal static bool HasIgnoreDataMemberAttribute(this PropertyInfo prop)
        {
            if (prop == null) return false;

            return prop.HasAttribute(typeof(IgnoreDataMemberAttribute));
        }

        /// <summary>
        /// Determines whether the given <param name="instance"></param> is a complex type or a simple ValueType
        /// </summary>
        /// <param name="instance">The type.</param>
        /// <returns>
        ///   <c>true</c> if [is complex type] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsComplexType(this Object instance)
        {
            if (instance == null) return false;

            var type = instance.GetType();
            return type.IsComplexType();
        }

        /// <summary>
        /// Determines whether [is complex type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if [is complex type] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsComplexType(this Type type)
        {
            if (type.GetTypeInfo().IsSubclassOf(typeof(ValueType)) || type == (typeof(string)))
                return false;

            return true;
        }

        /// <summary>
        /// Get instances of the given instance
        /// </summary>
        /// <param name="member">The member</param>
        /// <param name="attribute">The attribute to look for</param>
        /// <returns>A list of attribute instances</returns>
        public static List<Attribute> GetSpecificAttribute(this MemberInfo member, Type attribute)
        {
            return (
                from a in member.GetCustomAttributes()
                where a.GetType() == attribute
                select a
            ).ToList();
        }

        /// <summary>
        /// Does the member have any instances of the given attribute?
        /// </summary>
        /// <param name="member">The member</param>
        /// <param name="attribute">The attribute to check for</param>
        /// <returns><c>True</c> if the member has the attribute, <c>false</c> otherwise</returns>
        public static bool HasAttribute(this MemberInfo member, Type attribute)
        {
            return member.GetSpecificAttribute(attribute).Count > 0;
        }
    }
}
