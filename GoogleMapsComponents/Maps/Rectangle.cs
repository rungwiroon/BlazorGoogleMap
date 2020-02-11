using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleMapsComponents.Maps
{
    /// <summary>
    /// A rectangle overlay.
    /// </summary>
    public class Rectangle : IAsyncDisposable
    {
        private readonly JsObjectRef _jsObjetRef;
        private Map _map;

        /// <summary>
        /// Create a rectangle using the passed RectangleOptions, which specify the bounds and style.
        /// </summary>
        /// <param name="opts"></param>
        public async static Task<Rectangle> CreateAsync(IJSRuntime jsRuntime, RectangleOptions opts = null)
        {
            var jsObjectRef = await JsObjectRef.CreateAsync(jsRuntime, "google.maps.Rectangle", opts);

            var obj = new Rectangle(jsObjectRef, opts);

            return obj;
        }

        /// <summary>
        /// Create a rectangle using the passed RectangleOptions, which specify the bounds and style.
        /// </summary>
        /// <param name="opts"></param>
        private Rectangle(JsObjectRef jsObjectRef, RectangleOptions opts = null)
        {
            _jsObjetRef = jsObjectRef;
            _map = opts?.Map;
        }

        public ValueTask DisposeAsync() =>
            _jsObjetRef.DisposeAsync();

        /// <summary>
        /// Returns the bounds of this rectangle.
        /// </summary>
        /// <returns></returns>
        public Task<LatLngBoundsLiteral> GetBounds()
        {
            return _jsObjetRef.InvokeAsync<LatLngBoundsLiteral>(
                "getBounds");
        }

        /// <summary>
        /// Returns whether this rectangle can be dragged by the user.
        /// </summary>
        /// <returns></returns>
        public Task<bool> GetDraggable()
        {
            return _jsObjetRef.InvokeAsync<bool>(
                "getDraggable");
        }

        /// <summary>
        /// Returns whether this rectangle can be edited by the user.
        /// </summary>
        /// <returns></returns>
        public Task<bool> GetEditable()
        {
            return _jsObjetRef.InvokeAsync<bool>(
                "getEditable");
        }

        /// <summary>
        /// Returns the map on which this rectangle is displayed.
        /// </summary>
        /// <returns></returns>
        public Map GetMap()
        {
            return _map;
        }

        /// <summary>
        /// Returns whether this rectangle is visible on the map.
        /// </summary>
        /// <returns></returns>
        public Task<bool> GetVisible()
        {
            return _jsObjetRef.InvokeAsync<bool>(
                "getVisible");
        }

        /// <summary>
        /// Sets the bounds of this rectangle.
        /// </summary>
        /// <param name="bounds"></param>
        public Task SetBounds(LatLngBoundsLiteral bounds)
        {
            return _jsObjetRef.InvokeAsync(
                "setBounds",
                bounds);
        }

        /// <summary>
        /// If set to true, the user can drag this rectangle over the map.
        /// </summary>
        /// <param name="draggble"></param>
        public Task SetDraggable(bool draggble)
        {
            return _jsObjetRef.InvokeAsync(
                "setDraggable",
                draggble);
        }

        /// <summary>
        /// If set to true, the user can edit this rectangle by dragging the control points shown at the corners and on each edge.
        /// </summary>
        /// <param name="editable"></param>
        public Task SetEditable(bool editable)
        {
            return _jsObjetRef.InvokeAsync(
                "setEditable",
                editable);
        }

        /// <summary>
        /// Renders the rectangle on the specified map. If map is set to null, the rectangle will be removed.
        /// </summary>
        /// <param name="map"></param>
        public Task SetMap(Map map)
        {
            _map = map;

            return _jsObjetRef.InvokeAsync(
                "setMap",
                map);
        }

        public Task SetOptions(RectangleOptions options)
        {
            return _jsObjetRef.InvokeAsync(
                "setOptions",
                options);
        }

        /// <summary>
        /// Hides this rectangle if set to false.
        /// </summary>
        /// <param name="visible"></param>
        public Task SetVisible(bool visible)
        {
            return _jsObjetRef.InvokeAsync(
                "setVisible",
                visible);
        }
    }
}
