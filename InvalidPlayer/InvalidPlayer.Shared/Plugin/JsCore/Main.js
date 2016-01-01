var core = (function() {
    function sendToServer(param) {
        window.location.href = "InvalidPlayer-SendToServer://?param=" + param;
    }

    function invokeServerMethod(method, param, callback) {
        var content = "method=" + method + "&param=" + encodeURIComponent(param) + "&callback=" + callback;
        window.external.notify(content);
    }

    //TODO where?
    function paly(url) {
        var content = "method=Play&param=" + encodeURI(url) + "&callback=";
        window.external.notify(content);
    }


    /**
 *  errorMessage   错误信息
 *  scriptURI      出错的文件
 *   lineNumber     出错代码的行号
 *  columnNumber   出错代码的列号
 *  errorObj       错误的详细信息
 */
    window.onerror = function(errorMessage, scriptURI, lineNumber, columnNumber, errorObj) {
        var content = "method=Error&param=" + encodeURIComponent(errorMessage + scriptURI + lineNumber + columnNumber + errorObj) + "&callback=";
        window.external.notify(content);
    };
    return {
        invokeServerMethod: invokeServerMethod,
        play: paly
    };
})();