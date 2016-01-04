var core = (function() {

    window.debug = false;

    function notify(content) {
        if (debug) {
            var param = content.split("&");
            var cache = {};
            for (var i = 0; i < param.length; i++) {
                var kv = param[i].split("=");
                cache[kv[0]] = decodeURIComponent(kv[1]);
            }
            console.dir(cache);
        } else {
            window.external.notify(content);
        }

    }

    function sendToServer(param) {
        window.location.href = "InvalidPlayer-SendToServer://?param=" + param;
    }

    function invoke(method, param, callback) {
        var content = "method=" + method + "&param=" + encodeURIComponent(param) + "&callback=" + callback;
        notify(content);
    }

    function paly(url) {
        invoke("Play", url);
    }

    function playAll(json) {
        invoke("PlayAll", json);
    }

    window.onerror = function(errorMessage, scriptUrl, lineNumber, columnNumber, errorObj) {
        var message = format("%1 (%2,%3):%4 %5", scriptUrl, lineNumber, columnNumber, errorMessage, errorObj);
        invoke("Error", message);
    };

    function format(string) {
        var args = arguments;
        var pattern = new RegExp("%([1-" + arguments.length + "])", "g");
        return String(string).replace(pattern, function(match, index) {
            return args[index];
        });
    };

    function makeFunc() {
        var args = Array.prototype.slice.call(arguments);
        var func = args.shift();
        return function() {
            return func.apply(null, args.concat(Array.prototype.slice.call(arguments)));
        };
    }

    window.execute = function() {
        var args = Array.prototype.slice.call(arguments);
        var method = args.shift();
        var dotIndex = method.indexOf(".");
        if (dotIndex !== -1) {
            var namespace = method.substring(0, dotIndex);
            var name = method.substring(dotIndex + 1);
            window[namespace][name].apply(window, args);
        } else {
            window[method].apply(window, args);
        }
    };
    return {
        invokeServerMethod: invoke,
        play: paly,
        format: format,
        makeFunc: makeFunc
    };
})();

core.invokeServerMethod("Log", "core complete");