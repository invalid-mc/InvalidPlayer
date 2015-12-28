var self = require('sdk/self');
var browserWindows = require("sdk/windows").browserWindows;

function dummy(text, callback) {
    callback(text);
}
exports.dummy = dummy;

var buttons = require('sdk/ui/button/action');
var tabs = require("sdk/tabs");

var button = buttons.ActionButton({
    id: "invalid-player-link",
    label: "open url in InvalidPlayer",
    icon: {
        "16": "./icon-16.png",
        "32": "./icon-32.png",
        "64": "./icon-64.png"
    },
    onClick: handleClick
});

function handleClick(state) {
    var currentTab = tabs.activeTab;

    currentTab.attach({
        contentScriptFile: self.data.url("script.js")
    });

    // var url= currentTab.url;
    //url= "weburl://?url="+encodeURI(url)+"&cookie=";
    //tabs.open({
    //  url: url,
    //   inBackground: false
    // });
    // tabs.activeTab.close();
    // currentTab.activate();
}


var contextMenu = require("sdk/context-menu");
var menuItem = contextMenu.Item({
    label: "Open via Player",
    context: contextMenu.SelectorContext("a[href]"),
    contentScriptFile: self.data.url("script2.js"),
    onMessage: function (selectionText) {
        console.log(selectionText);
    }
});



