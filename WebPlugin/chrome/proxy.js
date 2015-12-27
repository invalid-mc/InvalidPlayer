window.onload = function () {
    window.open("weburl://?url=www.xxx.com", '_self');
    setTimeout(function() {
        document.getElementById("ax").innerText = "test";
    }, 2000);
};