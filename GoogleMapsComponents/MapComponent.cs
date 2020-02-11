using Microsoft.JSInterop;
using GoogleMapsComponents.Maps;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace GoogleMapsComponents
{
    [Obsolete]
    public class MapComponent : ComponentBase, IAsyncDisposable
    {
        [Inject]
        public IJSRuntime JsRuntime { get; protected set; }

        public Map InteropObject { get; private set; }

        public async Task InitAsync(ElementReference element, MapOptions options = null)
        {
            InteropObject = await Map.CreateAsync(JsRuntime, element, options);
        }

        public ValueTask DisposeAsync() =>
            InteropObject.DisposeAsync();
    }
}
