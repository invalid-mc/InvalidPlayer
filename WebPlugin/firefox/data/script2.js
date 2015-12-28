self.on("click", function (node) {

    var link = node.href;

    if (!document.hasOwnProperty("InvalidPlayerTabUrl")) {
        document.InvalidPlayerTabUrl = document.location.href;
    }

    var url = "weburl://?url=" + encodeURI(link);

    if (link.indexOf("bilibili.com") != -1) {
        var getCookie = function getCookie(name) {
            var arr = document.cookie.match(new RegExp("(^| )" + name + "=([^;]*)(;|$)"));
            if (arr != null) {
                return arr[2];
            } else {
                return null;
            }
        }

        var DedeUserID = getCookie("DedeUserID");
        if (DedeUserID) {
            var DedeUserID__ckMd5 = getCookie("DedeUserID__ckMd5");
            var SESSDATA = getCookie("SESSDATA");
            var Cookie = "DedeUserID=" + escape(DedeUserID) + "; DedeUserID__ckMd5=" + escape(DedeUserID__ckMd5) + "; SESSDATA=" + escape(SESSDATA);
            url += "&cookie=" + Cookie;
        }

    }
    //open player
    document.location.href = url;

});











