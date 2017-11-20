var maxImages;
var images;
var previews;
var galleryHelp;
var isLoaded;
var maximized = false;
var showingHelp = false;

var leftArrow = "<div id='previous' class='gallery-button previous-button fa fa-chevron-left'></div>";
var rightArrow = "<div id='next' class='gallery-button next-button fa fa-chevron-right'></div>";
var closeX = "<div id='close' class='gallery-button close-button fa fa-times'></div>";

var cookieName = "CurrentImageIndex";

function initGallery(model) {
    maxImages = model.MaxImages;
    images = model.Images;
    previews = model.ImagePreviews;
    galleryHelp = model.GalleryHelp;
    isLoaded = new Array(images.length);
    for (var i = 0; i < images.length; i++)
         isLoaded[i] = false;
}

function indexOf(arr, x) {
    for (var i = 0; i < arr.length; i++)
        if (x.indexOf(arr[i]) !== -1)
            return i;
    return -1;
}

function remove(element) {
    if (element && element.parentNode)
        element.parentNode.removeChild(element);
}

function getOverlay() {
    var div = document.createElement("div");
    div.id = "overlay";
    return div;
}

function getHelp() {
    var helpBody = "";
    for (var i = 0; i < galleryHelp.length; i++)
        helpBody += "<li>" + galleryHelp[i] + "</li>";
    var modal =
        "<div id='gallery-help'>" +
        "<div class='modal-content modal-sm center-help'>" +
        "<div class='modal-header'>" +
        "<div id='close' class='gallery-button close-button fa fa-times'></div>" +
        "<h4 class='modal-title'>Gallery Controls:</h4>" +
        "</div>" +
        "<div class='modal-body'>" +
        "<ul>" + helpBody + "</ul>" +
        "</div>" +
        "</div>" +
        "</div>";
    var div = document.createElement("div");
    div.id = "holder";
    div.innerHTML += modal;
    return div;
}

function getToggleBackground(index) {
    var current = getCookie("background");
    var active = current && Number(current) === index;
    var icon = "fa-star-o";
    if (active) {
        icon = "fa-star";
    }
    var toggleBackground = "<div id='toggle' class='gallery-button toggle-button fa " + icon + "'></div>";
    return toggleBackground;
}

function getHolder(src) {
    var div = document.createElement("div");
    var index = indexOf(previews, src);
    div.id = "holder";
    div.innerHTML += leftArrow + rightArrow + closeX + getToggleBackground(index);
    var loading = getLoading();
    div.appendChild(loading);
    div.appendChild(getImageWithSource(index, loading));
    return div;
}

function getImageWithSource(srcIndex, loading) {
    var img = document.createElement("img");
    img.style.visibility = "hidden";
    img.onload = function() {
        imageLoaded(img, srcIndex, loading);
    }
    img.id = "image-large";
    img.src = images[srcIndex];
    return img;
}

function getLoading() {
    var div = document.createElement("div");
    div.id = "loading";
    return div;
}

function preloadThumbnails(arrayOfImages) {
    for (var i = 0; i < arrayOfImages.length; i++)
        new Image().src = arrayOfImages[i];
}

function preloadImage(imageIndex) {
    var image = new Image();
    image.onload = function () {
        imageLoaded(null, imageIndex, null);
    }
    image.src = images[imageIndex];
}

function getClosest(index) {
    var delta = Math.pow(2, 53); // Max int
    var closest = null;
    for (var i = 0; i < images.length; i++) {
        if (isLoaded[i] || i === index)
            continue;
        var newDelta = Math.abs(index - i);
        if (newDelta > delta)
            continue;
        delta = newDelta;
        closest = i;
    }
    return closest;
}

function imageLoaded(image, imageIndex, loading) {
    loading = loading || document.querySelector("#loading");
    remove(loading);
    image = image || document.querySelector("#image-large");
    if (image)
        image.style.visibility = "visible";
    isLoaded[imageIndex] = true;
    var imageToPreload = getClosest(imageIndex);
    if (imageToPreload)
        preloadImage(imageToPreload);
}

function switchImage(delta) {
    var imageLarge = document.querySelector("#image-large");
    var nextImage = indexOf(images, imageLarge.getAttribute("src")) + delta;
    if (nextImage < 0 || nextImage >= images.length)
        return;
    remove(document.querySelector("#image-large"));
    remove(document.querySelector("#loading"));
    var loading = getLoading();
    document.querySelector("#holder").appendChild(loading);
    document.querySelector("#holder").appendChild(getImageWithSource(nextImage, loading));
    setHash(nextImage);
}

function closeOverlay(keepHash) {
    remove(document.querySelector("#overlay"));
    remove(document.querySelector("#holder"));
    maximized = false;
    if (keepHash !== true)
        deleteHash();
};

function closeHelp(keepHash) {
    remove(document.querySelector("#overlay"));
    remove(document.querySelector("#holder"));
    showingHelp = false;
    if (keepHash !== true)
        deleteHash();
}

function showHelp() {
    if (showingHelp)
        return;
    document.body.appendChild(getOverlay());
    document.body.appendChild(getHelp());
    document.querySelector("#overlay").onclick = closeHelp;
    document.querySelector("#gallery-help").onclick = closeHelp;
    document.querySelector("#close").onclick = closeHelp;
    showingHelp = true;
    setHash("help");
}

function changeToggleButton() {
    var button = document.querySelector("#toggle");
    if (!button)
        return;
    var active = "fa-star-o";
    var disabled = "fa-star";
    var newClasses = Array(button.classList.length);
    for (var i = 0; i < button.classList.length; i++) {
        if (button.classList[i] === active || button.classList[i] === disabled)
            newClasses[i] = button.classList[i] === active ? disabled : active;
        else
            newClasses[i] = button.classList[i];
    }
    button.classList = newClasses.join(" ");
}

function toggleBackground(index) {
    var cookie = getCookie("background");
    if (cookie && Number(cookie) === index) {
        removeBackground();
        deleteCookie("background");
    } else {
        setBackground(index);
        setCookie("background", index);
    }
    changeToggleButton();
}

function setBackground(index) {
    var image = images[index];
    document.querySelector("#background").style.backgroundImage = "url(" + image + ")";
}

function removeBackground() {
    document.querySelector("#background").style.backgroundImage = "none";
}

function bindEvents() {
    var imagePreviews = document.querySelectorAll(".row > div > img.img-thumbnail");
    for (var i = 0; i < imagePreviews.length; i++) {
        imagePreviews[i].onclick = function (event) {
            event = event || window.event;
            var target = event.target || event.srcElement;
            var index = indexOf(previews, target.src);
            closeOverlay(true);
            document.body.appendChild(getOverlay());
            document.body.appendChild(getHolder(target.src));
            setHash(index);
            maximized = true;

            document.querySelector("#overlay").onclick = closeOverlay;
            document.querySelector("#close").onclick = closeOverlay;
            document.querySelector("#previous").onclick = function () {
                switchImage(-1);
            };
            document.querySelector("#next").onclick = function () {
                switchImage(1);
            };
            document.querySelector("#toggle").onclick = function () {
                toggleBackground(index);
            };
        };
    }
}

function loadImage(hash) {
    var selector = "img[src='" + previews[hash.substring(1)] + "']";
    var thumbnail = document.querySelector(selector);
    thumbnail.click();
}

var keyHandler = function (event) {
    if (event.keyCode === 112 && !maximized)
        showHelp();
    if (event.keyCode === 27 && maximized)
        closeOverlay();
    if (event.keyCode === 27 && showingHelp)
        closeHelp();
    if (event.keyCode !== 37 && event.keyCode !== 39)
        return;
    var delta = event.keyCode === 37 ? -1 : 1;
    if (maximized)
        switchImage(delta);
};

function setHash(value) {
    document.location.hash = value;
}

function getHash() {
    return document.location.hash;
}

function deleteHash() {
    //Leaves # in URL
    document.location.hash = "";
}

var historyHandler = function() {
    var hash = getHash();
    if (hash === "#help")
        showHelp();
    else if (hash !== "" && !isNaN(hash.substring(1)))
        loadImage(hash);
    else if (hash === "") {
        closeOverlay(true);
        closeHelp(true);
    }
}

function setCookie(name, value) {
    document.cookie = name + "=" + value + "; path=/";
}

function getCookie(name) {
    var extendedName = name + "=";
    var parts = document.cookie.split(";");
    for (var i = 0; i < parts.length; i++) {
        var part = parts[i];
        while (part.charAt(0) === " ")
            part = part.substring(1, part.length);
        if (part.indexOf(extendedName) === 0)
            return part.substring(extendedName.length, part.length);
    }
    return null;
}

function deleteCookie(name) {
    document.cookie = name + "=; expires=Thu, 01 Jan 1970 00:00:01 GMT;";
}

var cookieHandler = function() {
    var cookie = getCookie("background");
    if (cookie && !isNaN(cookie))
        setBackground(cookie);
}

var loadHandler = function () {
    preloadThumbnails(previews);
    var imagePreviews = document.querySelectorAll(".row > div > img.img-thumbnail");
    for (var i = 0; i < maxImages; i++)
        imagePreviews[i].setAttribute("src", previews[i]);
    bindEvents();
    historyHandler();
    cookieHandler();
};

if (document.addEventListener) {
    // Mozilla, Opera and WebKit
    document.addEventListener("DOMContentLoaded", loadHandler);
    document.addEventListener("keydown", keyHandler);
    window.addEventListener("popstate", historyHandler);
}
else if (document.attachEvent) {
    // If Internet Explorer, the event model is used
    document.attachEvent("onreadystatechange", function () {
        if (document.readyState === "complete") {
            loadHandler();
        }
    });
    document.attachEvent("onkeydown", keyHandler);
}
