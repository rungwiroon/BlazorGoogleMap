using GoogleMapsComponents.Maps;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using OneOf;
using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace GoogleMapsComponents
{
    internal static class Helper
    {
        internal static Task MyInvokeAsync(
            this IJSRuntime jsRuntime,
            string identifier,
            params object[] args)
        {
            return jsRuntime.MyInvokeAsync<object>(identifier, args);
        }

        private static string SerializeObject(object obj)
        {
            return JsonConvert.SerializeObject(
                        obj,
                        Formatting.None,
                        new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                            ContractResolver = new CamelCasePropertyNamesContractResolver()
                        });
        }

        internal static async Task<TRes> MyInvokeAsync<TRes>(
            this IJSRuntime jsRuntime,
            string identifier, 
            params object[] args)
        {
            var jsFriendlyArgs = args
                //.Where(arg =>
                //{
                //    var argType = arg.GetType();

                //    if (arg is Action action
                //        && arg == null)
                //    {
                //        return false;
                //    }
                //    else if (argType.IsGenericType
                //        && (argType.GetGenericTypeDefinition() == typeof(Action<>))
                //        && arg == null)
                //    {
                //        return false;
                //    }

                //    return true;
                //})
                .Select(arg =>
                {
                    if (arg == null)
                        return arg;

                    if (arg is IOneOf oneof)
                    {
                        arg = oneof.Value;
                    }

                    var argType = arg.GetType();

                    if (arg is ElementReference
                        || arg is string
                        || arg is int
                        || arg is long
                        || arg is double
                        || arg is float
                        || arg is decimal
                        || arg is DateTime)
                    {
                        return arg;
                    }
                    else if (arg is Action action)
                    {
                        return DotNetObjectReference.Create(
                            new JsCallableAction(jsRuntime, action));
                    }
                    else if (argType.IsGenericType
                        && (argType.GetGenericTypeDefinition() == typeof(Action<>)))
                    {
                        var genericArguments = argType.GetGenericArguments();

                        //Debug.WriteLine($"Generic args : {genericArguments.Count()}");

                        return DotNetObjectReference.Create(new JsCallableAction(jsRuntime, (Delegate)arg, genericArguments));
                    }
                    else if (arg is JsCallableAction)
                    {
                        return DotNetObjectReference.Create(arg);
                    }
                    else if (arg is IJsObjectRef jsObjectRef)
                    {
                        //Debug.WriteLine("Serialize IJsObjectRef");

                        var guid = jsObjectRef.Guid;
                        return SerializeObject(new JsObjectRef1(guid));
                    }
                    else
                    {
                        return SerializeObject(arg);
                    }
                });

            //if (typeof(IJsObjectRef).IsAssignableFrom(typeof(TRes)))
            //{
            //    var guid = await jsRuntime.InvokeAsync<string>(identifier, jsFriendlyArgs);

            //    return (TRes)JsObjectRefInstances.GetInstance(guid);
            //}
            if (typeof(IOneOf).IsAssignableFrom(typeof(TRes)))
            {
                var resultObject = await jsRuntime.InvokeAsync<string>(identifier, jsFriendlyArgs);
                object result = null;

                if (resultObject is string someText)
                {
                    try
                    {
                        var jo = JObject.Parse(someText);

                        if (jo.ContainsKey("dotnetTypeName"))
                        {
                            var typeName = jo.SelectToken("dotnetTypeName").Value<string>();
                            var asm = typeof(Map).Assembly;
                            var type = asm.GetType(typeName);
                            result = jo.ToObject(type);
                        }
                        else
                        {
                            result = someText;
                        }
                    }
                    catch
                    {
                        result = someText;
                    }
                }

                return (TRes)result;
            }
            else
            {
                return await jsRuntime.InvokeAsync<TRes>(identifier, jsFriendlyArgs);
            }
        }

        private static async Task<object> InvokeAsync(
            this IJSRuntime jsRuntime,
            string identifier,
            params object[] args)
        {
            var resultObject = await jsRuntime.MyInvokeAsync<string>(identifier, args);
            object result = null;

            if (resultObject is string someText)
            {
                try
                {
                    var jo = JObject.Parse(someText);

                    if (jo.ContainsKey("dotnetTypeName"))
                    {
                        var typeName = jo.SelectToken("dotnetTypeName").Value<string>();
                        var asm = typeof(Map).Assembly;
                        var type = asm.GetType(typeName);
                        result = jo.ToObject(type);
                    }
                    else
                    {
                        result = someText;
                    }
                }
                catch
                {
                    result = someText;
                }
            }

            return result;
        }

        /// <summary>
        /// For use when returned result will be one of multiple type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="jsRuntime"></param>
        /// <param name="identifier"></param>
        /// <param name="args"></param>
        /// <returns>Discriminated union of specified types</returns>
        internal static async Task<OneOf<T, U>> MyInvokeAsync<T, U>(
            this IJSRuntime jsRuntime,
            string identifier,
            params object[] args)
        {
            var result = await jsRuntime.InvokeAsync(identifier, args);

            switch (result)
            {
                case T t:
                    return t;
                case U u:
                    return u;
                default:
                    return default;
            }
        }

        /// <summary>
        /// For use when returned result will be one of multiple type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="jsRuntime"></param>
        /// <param name="identifier"></param>
        /// <param name="args"></param>
        /// <returns>Discriminated union of specified types</returns>
        internal static async Task<OneOf<T, U, V>> MyInvokeAsync<T, U, V>(
            this IJSRuntime jsRuntime,
            string identifier,
            params object[] args)
        {
            var result = await jsRuntime.InvokeAsync(identifier, args);

            switch (result)
            {
                case T t:
                    return t;
                case U u:
                    return u;
                case V v:
                    return v;
                default:
                    return default;
            }
        }

        internal static T ToEnum<T>(string str)
        {
            var enumType = typeof(T);
            foreach (var name in Enum.GetNames(enumType))
            {
                var enumMemberAttribute = ((EnumMemberAttribute[])enumType.GetField(name).GetCustomAttributes(typeof(EnumMemberAttribute), true)).Single();
                if (enumMemberAttribute.Value == str) return (T)Enum.Parse(enumType, name);
            }

            //throw exception or whatever handling you want
            return default;
        }
    }
}
