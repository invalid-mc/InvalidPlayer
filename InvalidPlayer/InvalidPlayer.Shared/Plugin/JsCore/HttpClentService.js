var httpClientService = (function() {
    var ex = {};
    ex.getString = function(url, callback) {
        //TODO 
        core.invokeServerMethod("HttpClientService.GetStringAsync", url, callback);
    };
    return ex;
})();