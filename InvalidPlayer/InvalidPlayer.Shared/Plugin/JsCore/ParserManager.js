var parserManager = (function () {
    var cache = {};

    function register(parser) {
        cache[new RegExp(parser.pattern, "gi")] = parser;
    }

    function execute(url) {
        for (var key in cache) {
            if (cache.hasOwnProperty(key)) {
                core.invokeServerMethod("Log", key);
                //TODO
                    core.invokeServerMethod("Log", url);
                    cache[key].parse(url);
              
            }
        }
    }


    return {
        register: register,
        execute: execute
    }
})();

