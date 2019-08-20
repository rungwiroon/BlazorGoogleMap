using Microsoft.JSInterop;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.Reflection;

namespace GoogleMapsComponents
{
    public class JsCallableAction
    {
        private readonly Delegate _delegate;
        private readonly Type[] _argumentTypes;
        private readonly IJSRuntime _jsRuntime;

        public JsCallableAction(IJSRuntime jsRuntime, Delegate @delegate, params Type[] argumentTypes)
        {
            _jsRuntime = jsRuntime;
            _delegate = @delegate;
            _argumentTypes = argumentTypes;
        }

        [JSInvokable]
        public void Invoke(string[] guids)
        {
            //Debug.WriteLine($"Invoke GUID count : {guids.Length}");

            if (_delegate == null)
                return;

            if (guids == null || !guids.Any() || !_argumentTypes.Any())
            {
                _delegate.DynamicInvoke();
                return;
            }

            //var jArray = JArray.Parse(args);
            var arguments = _argumentTypes.Zip(guids, (type, guid) => new { guid, type })
                .Select(x =>
                {
                    var ctor = x.type.GetConstructor(new[] { typeof(JsObjectRef) });

                    Debug.WriteLine($"type {x.type} Ctor {ctor} arg : {x.guid}");

                    var obj = ctor.Invoke(new object[] { new JsObjectRef(_jsRuntime, new Guid(x.guid)) });

                    return obj;
                })
                .ToArray();

            //Debug.WriteLine(arguments.FirstOrDefault()?.GetType());

            _delegate.DynamicInvoke(arguments);
        }
    }
}
