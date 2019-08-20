using Microsoft.JSInterop;
using Newtonsoft.Json.Linq;
using OneOf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleMapsComponents.Maps
{
    public class Marker : GoogleMapObjectRef
    {
        public async static Task<Marker> CreateAsync(IJSRuntime jsRuntime, MarkerOptions opts = null)
        {
            var jsObjectRef = await JsObjectRef.CreateAsync(jsRuntime, "google.maps.Marker", opts);
            var obj = new Marker(jsObjectRef);

            return obj;
        }

        private Marker(JsObjectRef jsObjectRef)
            : base(jsObjectRef)
        {
        }

        public async Task<Animation> GetAnimation()
        {
            var animation = await InvokeAsync<string>(
                "getAnimation");

            return Helper.ToEnum<Animation>(animation);
        }

        public Task<bool> GetClickable() =>
            InvokeAsync<bool>(
                "getClickable");

        public Task<string> GetCursor() =>
            InvokeAsync<string>(
                "getCursor");

        public Task<bool> GetDraggable() =>
            InvokeAsync<bool>(
                "getDraggable");

        public async Task<OneOf<string, Icon, Symbol>> GetIcon()
        {
            var result = await InvokeAsync<string, Icon, Symbol>(
                "getIcon");

            return result;
        }

        public Task<OneOf<string, MarkerLabel>> GetLabel() =>
            InvokeAsync<OneOf<string, MarkerLabel>>(
                "getLabel");

        public async Task<Map> GetMap()
        {
            var jsObjectRef = await InvokeWithReturnedObjectRefAsync(
                "getMap");

            return GoogleMapObjectRefInstances.GetInstance<Map>(jsObjectRef.Guid.ToString());
        }

        public Task<LatLngLiteral> GetPosition() =>
            InvokeAsync<LatLngLiteral>(
                "getPosition");

        public Task<MarkerShape> GetShape() =>
            InvokeAsync<MarkerShape>(
                "getShape");

        public Task<string> GetTitle() =>
            InvokeAsync<string>(
                "getTitle");

        public Task<bool> GetVisible() =>
            InvokeAsync<bool>(
                "getVisible");

        public Task<int> GetZIndex() =>
            InvokeAsync<int>(
                "getZIndex");

        /// <summary>
        /// Start an animation. 
        /// Any ongoing animation will be cancelled. 
        /// Currently supported animations are: BOUNCE, DROP. 
        /// Passing in null will cause any animation to stop.
        /// </summary>
        /// <param name="animation"></param>
        public Task SetAnimation(Animation animation) =>
            InvokeAsync(
                "setAnimation",
                animation);

        public Task SetClickable(bool flag) =>
            InvokeAsync(
                "setClickable",
                flag);

        public Task SetCursor(string cursor) =>
            InvokeAsync(
                "setCursor",
                cursor);

        public Task SetDraggable(bool flag) =>
            InvokeAsync(
                "setDraggable",
                flag);

        public Task SetIcon(string icon) =>
            InvokeAsync(
                "setIcon",
                icon);

        public Task SetIcon(Icon icon) =>
            InvokeAsync(
                "setIcon",
                icon);

        public Task SetLabel(Symbol label) =>
            InvokeAsync(
                "setLabel",
                label);

        /// <summary>
        /// Renders the marker on the specified map or panorama. 
        /// If map is set to null, the marker will be removed.
        /// </summary>
        /// <param name="map"></param>
        public Task SetMap(Map map) =>
            InvokeAsync(
                "setMap",
                map);

        public Task SetOpacity(float opacity) =>
            InvokeAsync(
                "setOpacity",
                opacity);

        public Task SetOptions(MarkerOptions options) =>
            InvokeAsync(
                "setOptions",
                options);

        public Task SetPosition(LatLngLiteral latLng) =>
            InvokeAsync(
                "setPosition",
                latLng);

        public Task SetShape(MarkerShape shape) =>
            InvokeAsync(
                "setShape",
                shape);

        public Task SetTiltle(string title) =>
            InvokeAsync(
                "setTiltle",
                title);

        public Task SetVisible(bool visible) =>
            InvokeAsync(
                "setVisible",
                visible);

        public Task SetZIndex(int zIndex) =>
            InvokeAsync(
                "setZIndex",
                zIndex);

        public async Task<MapEventListener> AddListener(string eventName, Action handler)
        {
            var listenerRef = await InvokeWithReturnedObjectRefAsync(
                "addListener", eventName, handler);

            return new MapEventListener(listenerRef);
        }

        public async Task<MapEventListener> AddListener<T>(string eventName, Action<T> handler)
        {
            var listenerRef = await InvokeWithReturnedObjectRefAsync(
                "addListener", eventName, handler);

            return new MapEventListener(listenerRef);
        }
    }
}
