var httpClientService = (function() {
    var ex = {};
    ex.getString = function(url, callback) {
        core.invokeServerMethod("HttpClientService.GetStringAsync", url, callback);
    };
    return ex;
})();


var securityKit = (function() {
    var ex = {};
    ex.computeMd5 = function(param, callback) {
        core.invokeServerMethod("SecurityKit.ComputeMd5", param, callback);
    };
    return ex;
})();

core.invokeServerMethod("Log", "Service complete");