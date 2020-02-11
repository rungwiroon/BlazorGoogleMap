using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleMapsComponents.Maps
{
    /// <summary>
    /// A service for computing directions between two or more places.
    /// </summary>
    public class DirectionsService : IAsyncDisposable
    {
        private readonly string jsObjectName = "googleMapDirectionServiceFunctions";
        private readonly JsObjectRef _jsObjectRef;

        /// <summary>
        /// Creates a new instance of a DirectionsService that sends directions queries to Google servers.
        /// </summary>
        public async static Task<DirectionsService> CreateAsync(IJSRuntime jsRuntime)
        {
            var jsObjectRef = await JsObjectRef.CreateAsync(jsRuntime, "google.maps.DirectionsService");

            var obj = new DirectionsService(jsObjectRef);

            return obj;
        }

        /// <summary>
        /// Creates a new instance of a DirectionsService that sends directions queries to Google servers.
        /// </summary>
        private DirectionsService(JsObjectRef jsObjectRef)
        {
            _jsObjectRef = jsObjectRef;
        }

        public ValueTask DisposeAsync() =>
            _jsObjectRef.DisposeAsync();

        /// <summary>
        /// Issue a directions search request.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="callback"></param>
        public async Task<DirectionResponse> Route(DirectionsRequest request)
        {
            var json = await _jsObjectRef.InvokeAsync<string>(
                    $"{jsObjectName}.route",
                    request);

            var directionResponse = JsonConvert.DeserializeObject<DirectionResponse>(json);

            return directionResponse;
        }
    }
}
