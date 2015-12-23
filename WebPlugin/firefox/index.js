var self = require('sdk/self');

// a dummy function, to show how tests work.
// to see how to test this function, look at test/test-index.js
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
  var currentTab=tabs.activeTab;
  var url= currentTab.url;
  url= "weburl://?url="+encodeURI(url);
  tabs.open({
    url: url,
    inBackground: false
  });
  tabs.activeTab.close();
  currentTab.activate();
}
