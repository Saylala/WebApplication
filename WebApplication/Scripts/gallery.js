var images;
var previews;
var maxImages;
var maximized = false;
var startIndex = 0;

var leftArrow = '<button type="button" class="gallery-button previous-button" id="previous">' +
    '<svg width="44" height="60">' +
    '<polyline points="30 10 10 30 30 50" stroke="#999999" stroke-width="4" stroke-linecap="butt" fill="none" stroke-linejoin="round"/>' +
    '</svg></button>';
var rightArrow = '<button type="button" class="gallery-button next-button" id="next">' +
    '<svg width="44" height="60">' +
    '<polyline points="14 10 34 30 14 50" stroke="#999999" stroke-width="4" stroke-linecap="butt" fill="none" stroke-linejoin="round"/>' +
    '</svg></button>';
var closeX = '<button type="button" class="gallery-button close-button" id="close">' +
    '<svg width="30" height="30">' +
    '<g stroke="#999999" stroke-width="4"><line x1="5" y1="5" x2="25" y2="25"/><line x1="5" y1="25" x2="25" y2="5"/></g>' +
    '</svg></button>';

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

function getImageWithSource(src) {
    return "<img id='image-large' src='" + src + "' onload='imageLoaded();' style='visibility: hidden;'/>" +
        "<div id='loading' class='loader'></div>";
}

function preloadImages(arrayOfImages) {
    $(arrayOfImages).each(function () {
        new Image().src = this;
    });
}

function imageLoaded() {
    $("#loading").remove();
    $("#image-large").css("visibility", "inherit");
}

function switchPreview(delta) {
    var index = startIndex + delta;
    if (index < 0 || index + maxImages > images.length)
        return;
    startIndex = index;
    var imagePreviews = $(".image-previews > .row > div > .thumbnail > img");
    for (var i = 0; i < maxImages; i++)
        $(imagePreviews[i]).attr("src", previews[i + startIndex]);
}

function switchImage(delta) {
    var imageLarge = $("#image-large");
    var nextImage = indexOf(images, imageLarge.attr("src")) + delta;
    if (nextImage < 0 || nextImage >= images.length)
        return;
    $("#image-large").remove();
    $("#holder").append(getImageWithSource(images[nextImage]));
    imageLarge.css("visibility", "hidden");
    imageLarge.attr("src", images[indexOf(images, imageLarge.attr("src")) + delta]);
}

function closeOverlay() {
    $("#overlay").remove();
    $("#holder").remove();
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
    switchPreview(delta);
};

function bindEvents() {
    $(".image-previews > .row > div > .thumbnail > img").click(function (event) {
        maximized = true;
        $(document.body).append(
            "<div id='overlay'></div>" +
            "<div id='holder'>" +
            leftArrow +
            getImageWithSource(images[indexOf(previews, event.target.src)]) +
            rightArrow +
            closeX +
            "</div>"
        );
        $("#overlay").click(closeOverlay);
        $("#close").click(closeOverlay);
        $("#previous").click(function () {
            switchImage(-1);
            switchPreview(-1);
        });
        $("#next").click(function () {
            switchImage(1);
            switchPreview(1);
        });
    });

    $("#preview-previous").click(function () {
        switchPreview(-1);
    });

    $("#preview-next").click(function () {
        switchPreview(1);
    });
}

$(document).ready(function () {
    preloadImages(previews);
    var imagePreviews = $(".image-previews > .row > div > .thumbnail > img");
    for (var i = 0; i < maxImages; i++)
        $(imagePreviews[i]).attr("src", previews[i + startIndex]);
    bindEvents();
});
