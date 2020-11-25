﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleMapsComponents.Maps.Extension
{
    public class ListableEntityListBase<TEntityBase, TEntityOptionsBase> : IDisposable
        where TEntityBase : ListableEntityBase<TEntityOptionsBase>
        where TEntityOptionsBase : ListableEntityOptionsBase
    {
        protected readonly JsObjectRef _jsObjectRef;

        public readonly Dictionary<string, TEntityBase> BaseListableEntities;

        protected ListableEntityListBase(JsObjectRef jsObjectRef, Dictionary<string, TEntityBase> baseListableEntities)
        {
            _jsObjectRef = jsObjectRef;
            BaseListableEntities = baseListableEntities;
        }

        public void Dispose()
        {
            if (BaseListableEntities.Count > 0)
            {
                _jsObjectRef.DisposeMultipleAsync(BaseListableEntities.Select(e => e.Value.Guid).ToList());
                BaseListableEntities.Clear();
            }
        }

        /// <summary>
        /// only keys not matching with existent listable entity keys will be created
        /// </summary>
        /// <param name="opts"></param>
        /// <returns></returns>
        public virtual async Task AddMultipleAsync(Dictionary<string, TEntityOptionsBase> opts, string googleMapListableEntityTypeName)
        {
            if (opts.Count > 0)
            {
                Dictionary<string, JsObjectRef> jsObjectRefs = await _jsObjectRef.AddMultipleAsync(
                    googleMapListableEntityTypeName,
                    opts.ToDictionary(e => e.Key, e => (object)e.Value));
                Dictionary<string, TEntityBase> objs = jsObjectRefs.ToDictionary(e => e.Key, e => Activator.CreateInstance(typeof(TEntityBase), e.Value) as TEntityBase);

                //Someone can try to create element yet inside listable entities... really not the best approach... but manage it
                List<string> alreadyCreated = BaseListableEntities.Keys.Intersect(objs.Select(e => e.Key)).ToList();
                await RemoveMultipleAsync(alreadyCreated);

                //Now we can add all required object as NEW object
                foreach (string key in objs.Keys)
                {
                    BaseListableEntities.Add(key, objs[key]);
                }
            }
        }

        /// <summary>
        /// only Marker having keys matching with existent keys will be removed
        /// </summary>
        /// <param name="filterKeys"></param>
        /// <returns></returns>
        public virtual async Task RemoveMultipleAsync(List<string> filterKeys = null)
        {
            if ((filterKeys != null) && (filterKeys.Count > 0))
            {
                List<string> foundKeys = BaseListableEntities.Keys.Intersect(filterKeys).ToList();
                if (foundKeys.Count > 0)
                {
                    List<Guid> foundGuids = BaseListableEntities.Where(e => foundKeys.Contains(e.Key)).Select(e => e.Value.Guid).ToList();
                    await _jsObjectRef.DisposeMultipleAsync(foundGuids);

                    foreach (string key in foundKeys)
                    {
                        //Marker object needs to dispose call due to previous DisposeMultipleAsync call
                        //Probably superfluous, but Garbage Collector may appreciate it... 
                        BaseListableEntities[key] = null;
                        BaseListableEntities.Remove(key);
                    }
                }
            }
        }

        public virtual async Task RemoveMultipleAsync(List<Guid> guids)
        {
            if (guids.Count > 0)
            {
                List<string> foundKeys = BaseListableEntities.Where(e => guids.Contains(e.Value.Guid)).Select(e => e.Key).ToList();
                if (foundKeys.Count > 0)
                {
                    List<Guid> foundGuids = BaseListableEntities.Values.Where(e => guids.Contains(e.Guid)).Select(e => e.Guid).ToList();
                    await _jsObjectRef.DisposeMultipleAsync(foundGuids);

                    foreach (string key in foundKeys)
                    {
                        //Listable entities object needs to dispose call due to previous DisposeMultipleAsync call
                        //Probably superfluous, but Garbage Collector may appreciate it... 
                        BaseListableEntities[key] = null;
                        BaseListableEntities.Remove(key);
                    }
                }
            }
        }

        //Find the eventual match between required keys (if any) and yet stored markers key (if any)
        //If filterKeys is null or empty all keys are returned
        //Otherwise only eventually yet stored marker keys that matches with filterKeys
        protected virtual List<string> ComputeMathingKeys(List<string> filterKeys = null)
        {
            List<string> matchingKeys;

            if ((filterKeys == null) || (!filterKeys.Any()))
            {
                matchingKeys = BaseListableEntities.Keys.ToList();
            }
            else
            {
                matchingKeys = BaseListableEntities.Keys.Where(e => filterKeys.Contains(e)).ToList();
            }

            return matchingKeys;
        }

        //Creates mapping between matching keys and markers Guid
        protected virtual Dictionary<Guid, string> ComputeInternalMapping(List<string> matchingKeys)
        {
            return BaseListableEntities.Where(e => matchingKeys.Contains(e.Key)).ToDictionary(e => BaseListableEntities[e.Key].Guid, e => e.Key);
        }

        //Creates mapping between markers Guid and empty array of parameters (getter has no parameter)
        protected virtual Dictionary<Guid, object> ComputeDictArgs(List<string> matchingKeys)
        {
            return BaseListableEntities.Where(e => matchingKeys.Contains(e.Key)).ToDictionary(e => e.Value.Guid, e => (object)(new object[] { }));
        }

        //Create an empty result of the correct type in case of no matching keys
        protected virtual Task<Dictionary<string, T>> ComputeEmptyResult<T>()
        {
            return Task<Dictionary<string, T>>.Factory.StartNew(() => { return new Dictionary<string, T>(); });
        }

        public virtual Task<Dictionary<string, Map>> GetMaps(List<string> filterKeys = null)
        {
            List<string> matchingKeys = ComputeMathingKeys(filterKeys);

            if (matchingKeys.Any())
            {
                Dictionary<Guid, string> internalMapping = ComputeInternalMapping(matchingKeys);
                Dictionary<Guid, object> dictArgs = ComputeDictArgs(matchingKeys);

                return _jsObjectRef.InvokeMultipleAsync<Map>(
                    "getMap",
                    dictArgs).ContinueWith(e => e.Result.ToDictionary(r => internalMapping[new Guid(r.Key)], r => r.Value));
            }
            else
            {
                return ComputeEmptyResult<Map>();
            }
        }

        public virtual Task<Dictionary<string, bool>> GetDraggables(List<string> filterKeys = null)
        {
            List<string> matchingKeys = ComputeMathingKeys(filterKeys);

            if (matchingKeys.Any())
            {
                Dictionary<Guid, string> internalMapping = ComputeInternalMapping(matchingKeys);
                Dictionary<Guid, object> dictArgs = ComputeDictArgs(matchingKeys);

                return _jsObjectRef.InvokeMultipleAsync<bool>(
                    "getDraggable",
                    dictArgs).ContinueWith(e => e.Result.ToDictionary(r => internalMapping[new Guid(r.Key)], r => r.Value));
            }
            else
            {
                return ComputeEmptyResult<bool>();
            }
        }

        public virtual Task<Dictionary<string, bool>> GetVisibles(List<string> filterKeys = null)
        {
            List<string> matchingKeys = ComputeMathingKeys(filterKeys);

            if (matchingKeys.Any())
            {
                Dictionary<Guid, string> internalMapping = ComputeInternalMapping(matchingKeys);
                Dictionary<Guid, object> dictArgs = ComputeDictArgs(matchingKeys);

                return _jsObjectRef.InvokeMultipleAsync<bool>(
                    "getVisible",
                    dictArgs).ContinueWith(e => e.Result.ToDictionary(r => internalMapping[new Guid(r.Key)], r => r.Value));
            }
            else
            {
                return ComputeEmptyResult<bool>();
            }
        }

        /// <summary>
        /// Renders the listable entity on the specified map or panorama. 
        /// If map is set to null, the marker will be removed.
        /// </summary>
        /// <param name="map"></param>
        public virtual async Task SetMaps(Dictionary<string, Map> maps)
        {
            Dictionary<Guid, object> dictArgs = maps.ToDictionary(e => BaseListableEntities[e.Key].Guid, e => (object)e.Value);
            await _jsObjectRef.InvokeMultipleAsync(
                   "setMap",
                   dictArgs);
        }

        public virtual Task SetDraggables(Dictionary<string, bool> draggables)
        {
            Dictionary<Guid, object> dictArgs = draggables.ToDictionary(e => BaseListableEntities[e.Key].Guid, e => (object)e.Value);
            return _jsObjectRef.InvokeMultipleAsync(
                "setDraggable",
                dictArgs);
        }

        public virtual Task SetOptions(Dictionary<string, TEntityOptionsBase> options)
        {
            Dictionary<Guid, object> dictArgs = options.ToDictionary(e => BaseListableEntities[e.Key].Guid, e => (object)e.Value);
            return _jsObjectRef.InvokeMultipleAsync(
                "setOptions",
                dictArgs);
        }

        public virtual Task SetVisibles(Dictionary<string, bool> visibles)
        {
            Dictionary<Guid, object> dictArgs = visibles.ToDictionary(e => BaseListableEntities[e.Key].Guid, e => (object)e.Value);
            return _jsObjectRef.InvokeMultipleAsync(
                "setVisible",
                dictArgs);
        }

        public async Task AddListener(string eventName, Action<TEntityBase> handler)
        {
            //Just for debugging 
            bool testItems = false;
            if (testItems)
            {
                foreach (TEntityBase item in this.BaseListableEntities.Values)
                {
                    await item.AddListener(eventName, () => handler(item));
                    //await item.AddListener(eventName,handler);
                    //break;
                }
            }

            Action<MouseEvent> _handler=(e) =>
            {
                var key = e.JsObjectRef.Guid.ToString();
                //Seems that the guid is of the click, not the item.
                if (this.BaseListableEntities.TryGetValue(key.ToString(), out var item))
                    handler(item);
            };

            var guids = BaseListableEntities.Select(e => e.Value.Guid).ToList();
            JsObjectRef listenerRef = await _jsObjectRef.InvokeMultipleWithReturnedObjectRefAsync("addListener",
                                                                                                  eventName,
                                                                                                  guids,
                                                                                                  _handler);
            //MapEventListener eventListener = new MapEventListener(listenerRef);

            //if (!EventListeners.ContainsKey(eventName))
            //{
            //    EventListeners.Add(eventName, new List<MapEventListener>());
            //}
            //EventListeners[eventName].Add(eventListener);

            //return eventListener;


            //invokeMultipleWithReturnedObjectRef


        }
    }
}
