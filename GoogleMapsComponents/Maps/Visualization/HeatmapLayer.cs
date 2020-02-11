using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleMapsComponents.Maps.Visualization
{
    /// <summary>
    /// A layer that provides a client-side rendered heatmap, depicting the intensity of data at geographical points.
    /// </summary>
    public class HeatmapLayer : IAsyncDisposable
    {
        private Map _map;

        //private readonly string jsObjectName = "googleMapHeatmapLayerJsFunctions";

        private readonly JsObjectRef _jsObjectRef;

        /// <summary>
        /// Creates a new instance of HeatmapLayer.
        /// </summary>
        /// <param name="opts"></param>
        public async static Task<HeatmapLayer> CreateAsync(IJSRuntime jsRuntime, HeatmapLayerOptions opts = null)
        {
            var jsObjectRef = await JsObjectRef.CreateAsync(jsRuntime, "google.maps.visualization.HeatmapLayer", opts);

            var obj = new HeatmapLayer(jsObjectRef, opts);

            return obj;
        }

        /// <summary>
        /// Creates a new instance of HeatmapLayer.
        /// </summary>
        /// <param name="opts"></param>
        private HeatmapLayer(JsObjectRef jsObjectRef, HeatmapLayerOptions opts = null)
        {
            _jsObjectRef = jsObjectRef;
            _map = opts?.Map;
        }

        public ValueTask DisposeAsync() =>
            _jsObjectRef.DisposeAsync();

        /// <summary>
        /// Returns the data points currently displayed by this heatmap.
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<object>> GetData()
        {
            return _jsObjectRef.InvokeAsync<IEnumerable<object>>(
                "getData");
        }

        public Map GetMap()
        {
            return _map;
        }

        /// <summary>
        /// Sets the data points to be displayed by this heatmap.
        /// </summary>
        /// <param name="data"></param>
        public Task SetData(IEnumerable<LatLngLiteral> data)
        {
            return _jsObjectRef.InvokeAsync(
                "setData",
                data);
        }

        /// <summary>
        /// Sets the data points to be displayed by this heatmap.
        /// </summary>
        /// <param name="data"></param>
        public Task SetData(IEnumerable<WeightedLocation> data)
        {
            return _jsObjectRef.InvokeAsync(
                "setData",
                data);
        }

        /// <summary>
        /// Renders the heatmap on the specified map. If map is set to null, the heatmap will be removed.
        /// </summary>
        /// <param name="map"></param>
        public Task SetMap(Map map)
        {
            _map = map;

            return _jsObjectRef.InvokeAsync(
                "setMap",
                map);
        }

        public Task SetOptions(HeatmapLayerOptions options)
        {
            return _jsObjectRef.InvokeAsync(
                "setOptions",
                options);
        }
    }
}
