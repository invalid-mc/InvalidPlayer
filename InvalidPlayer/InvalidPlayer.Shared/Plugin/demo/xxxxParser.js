var xxxxParser = (function() {
    var pattern = "https://github.com[^\\s]?";

    var api = "https://raw.githubusercontent.com/saki-saki/InvalidPlayer/master/InvalidPlayer/InvalidPlayer.Shared/Plugin/demo/json.json";

    function parse(url) {
        httpClientService.getString(api, "xxxxParser.testCallback");
    }

    function testCallback(data) {
        var json = $.parseJSON(data);
        var url = json.url;
        core.play(url);
    }

    return {
        pattern: pattern,
        parse: parse,
        testCallback: testCallback
    };
})();

parserManager.register(xxxxParser);