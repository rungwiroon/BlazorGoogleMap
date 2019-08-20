function stringToFunction(str) {
    let arr = str.split(".");

    let fn = window || this;
    for (let i = 0, len = arr.length; i < len; i++) {
        fn = fn[arr[i]];
    }

    if (typeof fn !== "function") {
        throw new Error("function not found");
    }

    return fn;
}

function tryParseJson(item) {
    //console.log(item);

    if (item !== null
        && typeof item === "object"
        && "invokeMethodAsync" in item) {
        //console.log("wrap dotnet object ref");

        return async function (...args) {
            if (args === null || typeof args === "undefined" || args.length === 0) {
                item.invokeMethod("Invoke", []);
                return;
            }
                
            const guids = [];
            //console.dir(args[0]);

            args.map(arg => {
                if (Array.isArray(arg)) {
                    console.dir(arg);
                    const guids1 = arg.map(obj => googleMapsObjectManager.addObject(obj));
                    guids.push(googleMapsObjectManager.addObject(guids1));
                } else {
                    guids.push(googleMapsObjectManager.addObject(arg));
                }
            });

            await item.invokeMethod("Invoke", guids);
        };
    }

    if (typeof item !== "string")
        return item;

    let item2 = null;

    try {
        item2 = JSON.parse(item);
    } catch (e) {
        return item.replace(/['"]+/g, '');
    }

    if (typeof item2 === "object" && item2 !== null) {
        if ("guidString" in item2) {
            //console.log("Found object has Guid property.");
            return window._blazorGoogleMapsObjects[item2.guidString];
        } else {
            for (let propertyName in item2) {
                let propertyValue = item2[propertyName];
                if (typeof propertyValue === "object"
                    && propertyValue !== null
                    && "guidString" in propertyValue) {
                    //console.log("Found object has Guid property.");
                    item2[propertyName] = window._blazorGoogleMapsObjects[propertyValue.guidString];
                }
            }

            return item2;
        }
    }

    return item.replace(/['"]+/g, '');
}

function uuidv4() {
    return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, c =>
        (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
    );
}

window.googleMapsObjectManager = {
    createObject: function (...args) {
        //console.dir(args);

        let args2 = args.slice(2).map(arg => tryParseJson(arg));
        //console.dir(args2);
        let functionName = args[1];
        let constructor = stringToFunction(functionName);
        let obj = new constructor(...args2);

        //console.dir(obj);

        let guid = args[0];

        if ("set" in obj) {
            obj.set("guidString", guid);
        }
        
        googleMapsObjectManager.addObject(obj, guid);

        //console.dir(window._blazorGoogleMapsObjects);
    },

    addObject: function (obj, guid) {
        if (guid === null || typeof guid === "undefined") {
            guid = uuidv4();
        }

        window._blazorGoogleMapsObjects = window._blazorGoogleMapsObjects || [];
        window._blazorGoogleMapsObjects[guid] = obj;

        return guid;
    },

    getObject: function (guid) {
        return obj = window._blazorGoogleMapsObjects[guid];
    },

    disposeObject: function (guid) {
        delete window._blazorGoogleMapsObjects[guid];
    },

    invoke: function (...args) {
        //console.dir(args);

        let args2 = args.slice(2).map(arg => tryParseJson(arg));
        let obj = googleMapsObjectManager.getObject(args[0]);

        //console.log("Invoke " + args[1] + " " + args[0]);
        //console.dir(args2);
        //console.dir(window._blazorGoogleMapsObjects);
        //console.dir(args);
        //console.dir(args2);

        let result = obj[args[1]](...args2);

        //console.log(result);

        if (result !== null
            && typeof result === "object") {
            if ("get" in result) {
                return result.get("guidString");
            } else if ("dotnetTypeName" in result) {
                return JSON.stringify(result);
            } else {
                return result;
            }
        } else {
            return result;
        }
    },

    invokeWithReturnedObjectRef: function (...args) {
        //console.log(args);

        let result = googleMapsObjectManager.invoke(...args);
        let uuid = uuidv4();

        //console.log("invokeWithReturnedObjectRef " + uuid);

        window._blazorGoogleMapsObjects[uuid] = result;

        return uuid;
    },

    readObjectPropertyValue: function (...args) {
        let obj = googleMapsObjectManager.getObject(args[0]);

        return obj[args[1]];
    },

    readObjectPropertyValueWithReturnedObjectRef: function (...args) {
        let result = googleMapsObjectManager.readObjectPropertyValue(...args);

        return googleMapsObjectManager.addObject(result);
    },

    readObjectPropertyValueWithReturedJson: function (...args) {
        let result = googleMapsObjectManager.readObjectPropertyValue(...args);

        return JSON.stringify(result);
    },

    getEnumerator: function (guid) {
        return new Enumerator(googleMapsObjectManager.getObject(guid));
    }

    //invokeAsync: async function (guid, methodName, jsonArgs) {
    //    console.log("Invoke : route " + guid);

    //    //console.log("Invoke " + methodName);
    //    //console.dir(args);

    //    let args = [];

    //    if (typeof args === 'undefined') {
    //        args = JSON.parse(jsonArgs);
    //    }

    //    let obj = window._blazorGoogleMapsObjects[guid];

    //    let promise = new Promise((resolve, reject) => {
    //        return obj[methodName](...args, function (...args2) {

    //            //console.dir(args2);

    //            resolve(JSON.stringify(args2));
    //        });
    //    });

    //    return await promise;
    //}
};