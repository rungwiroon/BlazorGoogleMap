using Dasync.Collections;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using OneOf;
using System;
using System.Collections;
//using System.Collections.Async;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleMapsComponents
{
    internal class JsObjectRef1 : IJsObjectRef
    {
        protected Guid _guid;

        public Guid Guid
        {
            get { return _guid; }
        }

        public string GuidString
        {
            get { return _guid.ToString(); }
        }

        public JsObjectRef1(Guid guid)
        {
            _guid = guid;
        }

        [JsonConstructor]
        public JsObjectRef1(string guidString)
        {
            _guid = new Guid(guidString);
        }

        public override bool Equals(object obj)
        {
            var other = obj as JsObjectRef;

            if (other == null)
            {
                return false;
            }
            else
            {
                return other.Guid == _guid;
            }
        }

        public override int GetHashCode()
        {
            return _guid.GetHashCode();
        }
    }

    public class JsObjectRef : IJsObjectRef, System.IAsyncDisposable
    {
        protected readonly Guid _guid;
        protected readonly IJSRuntime _jsRuntime;

        public Guid Guid
        {
            get { return _guid; }
        }

        public IJSRuntime JSRuntime
        {
            get { return _jsRuntime; }
        }

        public JsObjectRef(
            IJSRuntime jsRuntime,
            Guid guid)
        {
            _jsRuntime = jsRuntime;
            _guid = guid;
        }

        public static Task<JsObjectRef> CreateAsync(
            IJSRuntime jsRuntime,
            string constructorFunctionName,
            params object[] args)
        {
            return CreateAsync(jsRuntime, Guid.NewGuid(), constructorFunctionName, args);
        }

        public async static Task<JsObjectRef> CreateAsync(
            IJSRuntime jsRuntime,
            Guid guid,
            string functionName,
            params object[] args)
        {
            var jsObjectRef = new JsObjectRef(jsRuntime, guid);

            await jsRuntime.MyInvokeAsync<object>(
                "googleMapsObjectManager.createObject",
                new object[] { guid.ToString(), functionName }
                    .Concat(args).ToArray()
            );

            return jsObjectRef;
        }

        public ValueTask DisposeAsync() =>
            _jsRuntime.InvokeVoidAsync(
                "googleMapsObjectManager.dispose",
                _guid.ToString()
            );

        public Task InvokeAsync(string functionName, params object[] args)
        {
            return _jsRuntime.MyInvokeAsync(
                "googleMapsObjectManager.invoke",
                new object[] { _guid.ToString(), functionName }
                    .Concat(args).ToArray()
            );
        }

        public Task<T> InvokeAsync<T>(string functionName, params object[] args)
        {
            return _jsRuntime.MyInvokeAsync<T>(
                "googleMapsObjectManager.invoke",
                new object[] { _guid.ToString(), functionName }
                    .Concat(args).ToArray()
            );
        }

        /// <summary>
        /// Use when returned result will be one of defined types
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="jsRuntime"></param>
        /// <param name="identifier"></param>
        /// <param name="args"></param>
        /// <returns>Discriminated union of specified types</returns>
        public Task<OneOf<T, U>> InvokeAsync<T, U>(string functionName, params object[] args)
        {
            return _jsRuntime.MyInvokeAsync<T, U>(
                "googleMapsObjectManager.invoke",
                new object[] { _guid.ToString(), functionName }
                    .Concat(args).ToArray()
            );
        }

        /// <summary>
        /// Use when returned result will be one of defined types
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="jsRuntime"></param>
        /// <param name="identifier"></param>
        /// <param name="args"></param>
        /// <returns>Discriminated union of specified types</returns>
        public Task<OneOf<T, U, V>> InvokeAsync<T, U, V>(string functionName, params object[] args)
        {
            return _jsRuntime.MyInvokeAsync<T, U, V>(
                "googleMapsObjectManager.invoke",
                new object[] { _guid.ToString(), functionName }
                    .Concat(args).ToArray()
            );
        }

        public async Task<JsObjectRef> InvokeWithReturnedObjectRefAsync(string functionName, params object[] args)
        {
            var guid = await _jsRuntime.MyInvokeAsync<string>(
                "googleMapsObjectManager.invokeWithReturnedObjectRef",
                new object[] { _guid.ToString(), functionName }
                    .Concat(args).ToArray()
            );

            return new JsObjectRef(_jsRuntime, new Guid(guid));
        }

        public async Task<JsObjectRef[]> InvokeWithReturnedObjectRefArrayAsync(string functionName, params object[] args)
        {
            var guids = await _jsRuntime.MyInvokeAsync<string[]>(
                "googleMapsObjectManager.invokeWithReturnedObjectRefArray",
                new object[] { _guid.ToString(), functionName }
                    .Concat(args).ToArray()
            );

            return guids.Select(guid => new JsObjectRef(_jsRuntime, new Guid(guid))).ToArray();
        }

        public Task<T> GetPropertyValue<T>(string propertyName)
        {
            return _jsRuntime.MyInvokeAsync<T>(
                "googleMapsObjectManager.readObjectPropertyValue",
                 _guid.ToString(),
                 propertyName);
        }

        public async Task<JsObjectRef> GetPropertyObjectByReference(string propertyName)
        {
            var guid = await _jsRuntime.MyInvokeAsync<string>(
                "googleMapsObjectManager.readObjectPropertyValueWithReturnedObjectRef",
                 _guid.ToString(),
                 propertyName);

            return new JsObjectRef(_jsRuntime, new Guid(guid));
        }

        public async Task<T> GetPropertyObjectByReference<T>(string propertyName, Func<JsObjectRef, T> constructor)
        {
            return constructor(await GetPropertyObjectByReference(propertyName));
        }

        public async Task<T> GetPropertyObjectByJson<T>(string propertyName)
        {
            var json = await _jsRuntime.MyInvokeAsync<string>(
                "googleMapsObjectManager.readObjectPropertyValueWithReturedJson",
                 _guid.ToString(),
                 propertyName);

            //Debug.WriteLine($"{nameof(GetPropertyObjectByJson)} {json}");

            return JsonConvert.DeserializeObject<T>(json);
        }

        public override bool Equals(object obj)
        {
            if (obj is JsObjectRef other)
            {
                return other.Guid == this.Guid;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_guid);
        }
    }

    public class GoogleMapObjectRef : System.IAsyncDisposable, IJsObjectRef
    {
        private readonly JsObjectRef _jsObjectRef;

        public Guid Guid => _jsObjectRef.Guid;

        public GoogleMapObjectRef(JsObjectRef jsObjectRef)
        {
            _jsObjectRef = jsObjectRef;
        }

        public static Task<JsObjectRef> CreateAsync(
            IJSRuntime jsRuntime,
            string constructorFunctionName,
            params object[] args)
        {
            return CreateAsync(jsRuntime, Guid.NewGuid(), constructorFunctionName, args);
        }

        public async static Task<JsObjectRef> CreateAsync(
            IJSRuntime jsRuntime,
            Guid guid,
            string functionName,
            params object[] args)
        {
            var jsObjectRef = new JsObjectRef(jsRuntime, guid);

            await jsRuntime.MyInvokeAsync<object>(
                "googleMapsObjectManager.createObject",
                new object[] { guid.ToString(), functionName }
                    .Concat(args).ToArray()
            );

            return jsObjectRef;
        }

        public ValueTask DisposeAsync()
        {
            return _jsObjectRef.DisposeAsync();
        }

        internal Task InvokeAsync(string functionName, params object[] args)
        {
            return _jsObjectRef.InvokeAsync(functionName, args);
        }

        internal Task<T> InvokeAsync<T>(string functionName, params object[] args)
        {
            return _jsObjectRef.InvokeAsync<T>(functionName, args);
        }

        /// <summary>
        /// Use when returned result will be one of defined types
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="jsRuntime"></param>
        /// <param name="identifier"></param>
        /// <param name="args"></param>
        /// <returns>Discriminated union of specified types</returns>
        internal Task<OneOf<T, U>> InvokeAsync<T, U>(string functionName, params object[] args)
        {
            return _jsObjectRef.InvokeAsync<T, U>(functionName, args);
        }

        /// <summary>
        /// Use when returned result will be one of defined types
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="jsRuntime"></param>
        /// <param name="identifier"></param>
        /// <param name="args"></param>
        /// <returns>Discriminated union of specified types</returns>
        internal Task<OneOf<T, U, V>> InvokeAsync<T, U, V>(string functionName, params object[] args)
        {
            return _jsObjectRef.InvokeAsync<T, U, V>(functionName, args);
        }

        internal Task<JsObjectRef> InvokeWithReturnedObjectRefAsync(string functionName, params object[] args)
        {
            return _jsObjectRef.InvokeWithReturnedObjectRefAsync(functionName, args);
        }

        internal Task<JsObjectRef[]> InvokeWithReturnedObjectRefArrayAsync(string functionName, params object[] args)
        {
            return _jsObjectRef.InvokeWithReturnedObjectRefArrayAsync(functionName, args);
        }

        internal Task<T> GetValue<T>(string propertyName)
        {
            return _jsObjectRef.GetPropertyValue<T>(propertyName);
        }

        internal Task<JsObjectRef> GetObjectReference(string propertyName)
        {
            return _jsObjectRef.GetPropertyObjectByReference(propertyName);
        }

        internal Task<T> GetObject<T>(string propertyName, Func<JsObjectRef, T> constructor)
        {
            return _jsObjectRef.GetPropertyObjectByReference<T>(propertyName, constructor);
        }

        internal Task<T> GetJson<T>(string propertyName)
        {
            return _jsObjectRef.GetPropertyObjectByJson<T>(propertyName);
        }

        public override bool Equals(object obj)
        {
            if (obj is GoogleMapObjectRef other)
            {
                return _jsObjectRef.Equals(other._jsObjectRef);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return _jsObjectRef.GetHashCode();
        }
    }

    public class JsArrayEnumerator<T> : GoogleMapObjectRef
    {
        internal JsArrayEnumerator(JsObjectRef jsOjbectRef)
            : base(jsOjbectRef)
        {

        }

        public Task<bool> AtEnd()
        {
            throw new NotImplementedException();
        }

        public Task<T> Item()
        {
            throw new NotImplementedException();
        }

        public Task MoveFirst()
        {
            throw new NotImplementedException();
        }

        public Task MoveNext()
        {
            throw new NotImplementedException();
        }
    }

    public class JsArrayRef<T> : AsyncEnumerable<T>
    {
        private AsyncEnumerable Create(JsArrayEnumerator<T> enumerator)
        {
            return new AsyncEnumerable<T>(async yield => {
                await enumerator.MoveFirst();

                while (!await enumerator.AtEnd())
                {
                    await yield.ReturnAsync(await enumerator.Item());

                    await enumerator.MoveNext();
                }
            });
        }

        public JsArrayRef(Func<AsyncEnumerator<T>.Yield, Task> enumerationFunction) 
            : base(enumerationFunction)
        {
        }
    }
}
