function setCookie(name, value) {
    document.cookie = name + "=" + value + "; path=/";
}

function saveResolution() {
    setCookie("width", window.outerWidth);
    setCookie("height", window.outerHeight);
}

if (document.addEventListener) {
    // Mozilla, Opera and WebKit
    document.addEventListener("DOMContentLoaded", saveResolution);
}
else if (document.attachEvent) {
    // If Internet Explorer, the event model is used
    document.attachEvent("onreadystatechange", function () {
        if (document.readyState === "complete") {
            saveResolution();
        }
    });
}