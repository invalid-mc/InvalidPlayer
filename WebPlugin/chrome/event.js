var regex = "^(\\w+://)?" +
            "([^/]*acfun\\.tv/.*ac=?\\d+)|" +
            "(live\\.bilibili\\.com/\\d+)|" +
            "([^/]+/video/av\\d+/(index(?:_\\d+)?\\.html)?)|" +
            "([^/]*douyutv\\.com/[^\\s]+)|" +
            "([^/]*hunantv\\.com/[^\\s]+)|" +
            "([^/]*iqiyi\\.com/[^\\s]+)|" +
            "([^/]*letv\\.com/[^\\s]+)|" +
            "([^/]*pptv\\.com/.*/\\w+\\.html)|" +
            "([^/]*v\\.qq\\.com/.*?\\?vid=\\w+)|" +
            "([^/]*sohu\\.com/[^\\s]+)|" +
            "([^/]*tudou\\.com/[^\\s]+)|" +
            "([^/]*youku\\.com/.*id_\\w+.*)" +
            "([^/]*youtube\\.com/[^\\s]+)$";

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
    chrome.tabs.sendMessage(tab.id, { url : tab.url });
    //var url = "weburl://?url=" + encodeURI(tab.url);
    //chrome.tabs.executeScript(tab.id, { file : "proxy.js" });
});
