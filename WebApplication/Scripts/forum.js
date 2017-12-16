function setData() {
    var items = document.querySelectorAll(".clickable");
    for (var i = 0; i < items.length; i++) {
        var data = items[i].dataset["link"];
        items[i].onclick = createFunction(data);
    }
}

function createFunction(data) {
    return function (event) { window.location = data; };
}

if (document.addEventListener) {
    // Mozilla, Opera and WebKit
    document.addEventListener("DOMContentLoaded", setData);
}
else if (document.attachEvent) {
    // If Internet Explorer, the event model is used
    document.attachEvent("onreadystatechange", function () {
        if (document.readyState === "complete") {
            setData();
        }
    });
}