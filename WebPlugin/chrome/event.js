var regex = "^(\\w+://)?([^/]*douyutv\\.com/[^\\s]+)|" +
            "(live\\.bilibili\\.com/\\d+)|" +
            "([^/]+/video/av\\d+/(index(?:_\\d+)?\\.html)?)|" +
            "([^/]*hunantv\\.com/[^\\s]+)|" +
            "([^/]*iqiyi\\.com/[^\\s]+)|" +
            "([^/]*letv\\.com/[^\\s]+)|" +
            "([^/]*v\\.qq\\.com/.*?\\?vid=\\w+)|" +
            "([^/]*sohu\\.com/[^\\s]+)|" +
            "([^/]*tudou\\.com/[^\\s]+)|" +
            "([^/]*youku\\.com/.*id_\\w+.*)$";

// When the extension is installed or upgraded
chrome.runtime.onInstalled.addListener(function(){
	chrome.declarativeContent.onPageChanged.removeRules(undefined, function(){
		// With a new rule ...
		chrome.declarativeContent.onPageChanged.addRules([
			{
				conditions: [
					// match html5 video element
					new chrome.declarativeContent.PageStateMatcher({
						pageUrl : { originAndPathMatches : regex },
						css : ["video"]
					}),
					// match chinese video website... only test --embed-- element, flash?
                    // change --embed-- to object type. query flash... (for 271)
					new chrome.declarativeContent.PageStateMatcher({
						pageUrl : { originAndPathMatches : regex },
						css : ["object"]
					})
				],
				actions : [ new chrome.declarativeContent.ShowPageAction() ]
			}
		]);
	});
});

// When page action button clicked
chrome.pageAction.onClicked.addListener(function(tab) {
    var url = "weburl://?url=" + encodeURI(tab.url);
    chrome.tabs.create({ url : url, index : tab.index + 1, openerTabId : tab.id });
    
    // chrome.windows.create({ url : url, focused : true, type : "popup" });
    
    // var proxyUrl = chrome.runtime.getURL("proxy.html");
	// chrome.tabs.create({ url : proxyUrl }, function(newTab){
	// 	// chrome.tabs.executeScript(newTab.id, {
    //     //     code : "window.location.href=" + url + "; window.close();"
    //     // });
	// });
});
