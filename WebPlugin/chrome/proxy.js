chrome.runtime.onMessage.addListener(function(request, sender, sendResponse) {
    var url = document.location.href;
    if(url != request.url) {
        return;
    }
    
    window.location.href = "weburl://?url=" + encodeURI(url);
})
