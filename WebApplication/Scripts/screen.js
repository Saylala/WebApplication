function setCookie(name, value) {
    document.cookie = name + "=" + value + "; path=/";
}

$(document).ready(function () {
    setCookie("width", $(window)[0].outerWidth);
    setCookie("height", $(window)[0].outerHeight);
});