using Microsoft.JSInterop;
using OneOf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleMapsComponents.Maps.TheData
{
    /// <summary>
    /// A feature has a geometry, an id, and a set of properties
    /// </summary>
    public class Feature : GoogleMapObjectRef
    {
        /// <summary>
        /// Constructs a Feature with the given options.
        /// </summary>
        /// <param name="options"></param>
        public async static Task<Feature> CreateAsync(IJSRuntime jsRuntime, FeatureOptions options = null)
        {
            var jsObjectRef = await JsObjectRef.CreateAsync(jsRuntime, "google.maps.Data.Feature", options);
            var obj = new Feature(jsObjectRef);

            return obj;
        }

        /// <summary>
        /// Constructs a Feature with the given options.
        /// </summary>
        /// <param name="options"></param>
        //private Feature(JsObjectRefFeatureOptions options = null)
        //{
        //    throw new NotImplementedException();
        //}

        internal Feature(JsObjectRef jsObjectRef)
            : base(jsObjectRef)
        {
        }

        //public IEnumerable<KeyValuePair<string, object>> Properties { get; private set; }

        /// <summary>
        /// Returns the feature's geometry.
        /// </summary>
        /// <returns></returns>
        public Geometry GetGeometry()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the feature ID.
        /// </summary>
        /// <returns></returns>
        public OneOf<int, string>? GetId()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the value of the requested property, or undefined if the property does not exist.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object GetProperty(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the property with the given name.
        /// </summary>
        /// <param name="name"></param>
        public void RemoveProperty(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the feature's geometry.
        /// </summary>
        /// <param name="newGeometry"></param>
        public void SetGeometry(OneOf<Geometry, LatLngLiteral> newGeometry)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the value of the specified property. 
        /// If newValue is undefined this is equivalent to calling removeProperty.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="newValue"></param>
        public void SetProperty(string name, object newValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Exports the feature to a GeoJSON object.
        /// </summary>
        /// <returns></returns>
        public Task<object> ToGeoJson()
        {
            throw new NotImplementedException();
        }

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
