var parserManager = (function() {
    var regExpCache = {};
    var cache = {};

    function register(parser) {
        var pattern = parser.pattern;
        var rx = new RegExp(pattern, "i");
        regExpCache[pattern] = rx;
        cache[pattern] = parser;
        core.invokeServerMethod("Log", "register: " + pattern);
    }

    function execute(url) {
        core.invokeServerMethod("Log", "execute " + url);
        var parser;
        for (var key in cache) {
            if (cache.hasOwnProperty(key)) {
                core.invokeServerMethod("Log", key);
                var reg = regExpCache[key];
                if (reg.test(url)) {
                    core.invokeServerMethod("Log", url);
                    parser = cache[key];
                    break;
                }
            }
        }
        if (parser) {
            run(parser, url);
        }
    }

    function run(parser, url) {
        try {
            parser.parse(url);
        } catch (e) {
            core.invokeServerMethod("Error", e.toString());
        }
    }

    function load(scripts, callback) {
        core.invokeServerMethod("Log", "scripts:" + scripts);
        LazyLoad.js(scripts.split("|"), function() {
            // core.invokeServerMethod("callback", "LoadEnd");
        });
    }

    return {
        register: register,
        execute: execute,
        load: load
    };
})();
core.invokeServerMethod("Log", "parser complete");