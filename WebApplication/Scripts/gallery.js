var images;
var previews;
var maxImages;
var maximized = false;
var startIndex = 0;

var leftArrow = "<button type='button' class='gallery-button previous-button' id='previous'>" +
    "<svg width='44' height='60'>" +
    "<polyline points='30 10 10 30 30 50' stroke='#999999' stroke-width='4' stroke-linecap='butt' fill='none' stroke-linejoin='round'/>" +
    "</svg></button>";
var rightArrow = "<button type='button' class='gallery-button next-button' id='next'>" +
    "<svg width='44' height='60'>" +
    "<polyline points='14 10 34 30 14 50' stroke='#999999' stroke-width='4' stroke-linecap='butt' fill='none' stroke-linejoin='round'/>" +
    "</svg></button>";
var closeX = "<button type='button' class='gallery-button close-button' id='close'>" +
    "<svg width='30' height='30'>" +
    "<g stroke='#999999' stroke-width='4'><line x1='5' y1='5' x2='25' y2='25'/><line x1='5' y1='25' x2='25' y2='5'/></g>" +
    "</svg></button>";

function initGallery(model) {
    maxImages = model.MaxImages;
    images = model.Images;
    previews = model.ImagePreviews;
}

function indexOf(arr, x) {
    for (var i = 0; i < arr.length; i++)
        if (x.indexOf(arr[i]) !== -1)
            return i;
    return -1;
}

function getOverlay() {
    var div = document.createElement("div");
    div.id = "overlay";
    return div;
}

function getHolder(src) {
    var div = document.createElement("div");
    div.id = "holder";
    div.innerHTML += leftArrow + rightArrow + closeX;
    div.appendChild(getImageWithSource(images[indexOf(previews, src)]));
    div.appendChild(getLoading());
    return div;
}

function getImageWithSource(src) {
    var img = document.createElement("img");
    img.id = "image-large";
    img.src = src;
    img.onload = imageLoaded;
    img.style.visibility = "hidden";
    return img;
}

function getLoading() {
    var div = document.createElement("div");
    div.id = "loading";
    return div;
}

function preloadImages(arrayOfImages) {
    for (var i = 0; i < arrayOfImages.length; i++) {
        new Image().src = arrayOfImages[i];
    }
}

function remove(element) {
    if (element)
        element.parentNode.removeChild(element);
}

function imageLoaded() {
    remove(document.querySelector("#loading"));
    document.querySelector("#image-large").style.visibility = "visible";
}

function switchImage(delta) {
    var imageLarge = document.querySelector("#image-large");
    var nextImage = indexOf(images, imageLarge.getAttribute("src")) + delta;
    if (nextImage < 0 || nextImage >= images.length)
        return;
    remove(document.querySelector("#image-large"));
    remove(document.querySelector("#loading"));
    document.querySelector("#holder").appendChild(getImageWithSource(images[nextImage]));
    document.querySelector("#holder").appendChild(getLoading());
}

function closeOverlay() {
    remove(document.querySelector("#overlay"));
    remove(document.querySelector("#holder"));
    maximized = false;
};

window.onkeydown = function (event) {
    if (event.keyCode === 27 && maximized)
        closeOverlay();
    if (event.keyCode !== 37 && event.keyCode !== 39)
        return;
    var delta = event.keyCode === 37 ? -1 : 1;
    if (maximized)
        switchImage(delta);
};

function bindEvents() {
    var imagePreviews = document.querySelectorAll(".row > div > img.img-thumbnail");
    for (var i = 0; i < imagePreviews.length; i++) {
        imagePreviews[i].onclick = function (event) {
            event = event || window.event;
            var target = event.target || event.srcElement;
            maximized = true;
            document.body.appendChild(getOverlay());
            document.body.appendChild(getHolder(target.src));
            document.querySelector("#overlay").onclick = closeOverlay;
            document.querySelector("#close").onclick = closeOverlay;
            document.querySelector("#previous").onclick = function () {
                switchImage(-1);
            };
            document.querySelector("#next").onclick = function () {
                switchImage(1);
            };
        };
    }
}

var handler = function () {
    preloadImages(previews);
    var imagePreviews = document.querySelectorAll(".row > div > img.img-thumbnail");
    for (var i = 0; i < maxImages; i++)
        imagePreviews[i].setAttribute("src", previews[i + startIndex]);
    bindEvents();
};

if (document.addEventListener) {
    // Mozilla, Opera and WebKit
    document.addEventListener("DOMContentLoaded", handler);
}
else if (document.attachEvent) {
    // If Internet Explorer, the event model is used
    document.attachEvent("onreadystatechange", function () {
        if (document.readyState === "complete") {
            handler();
        }
    });
} else {
    // A fallback to window.onload, that will always work
    var oldOnload = window.onload;
    window.onload = function () {
        oldOnload && oldOnload();
        handler();
    }
}
