$(document).ready(function () {
    $(".clickable").click(function (e) {
        window.location = $(this).data("link");
    });
});
