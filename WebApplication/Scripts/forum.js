$(document).ready(function () {
    $(".clickable").click(function (e) {
        window.location = $(this).data("link");
    });

    var refreshButton = $("#refresh");
    var interval = setInterval(function () {
        $.ajax({
            type: "POST",
            url: refreshButton.data("url"),
            data: {
                threadId: refreshButton.data("thread-id"),
                currentCount: $(".post").length
            }
        }).success(function (ans) {
            console.log(ans);
            for (var i = 0; i < ans.Posts.length; i++)
                $(".post").last().after(
                    "<div class='post list-group-item clickable' data-link='#'>" +
                        "<div class='head-container'>" +
                            "<h2 class='left-side'>" + ans.Posts[i].Topic + "</h2>" +
                            "<div class='right-side'>" +
                                "<p>" + (ans.Posts[i].Username == null ? "DELETED" : ans.Posts[i].Username) + "</p>" +
                                "<p>" + ans.Posts[i].Timestamp + "</p>" +
                            "</div>" +
                        "</div>" +
                        "<hr/>" +
                        "<p>" + ans.Posts[i].Text + "</p>" +
                        (ans.Posts[i].UserId === ans.UserId || ans.IsAdmin ? "<hr/>" +
                        "<form action='" + ans.DeleteUrl + "' method='post'>" +
                            "<input type='hidden' name='threadId' value='" + ans.Posts[i].ThreadId + "'>" +
                            "<input type='hidden' name='postId' value='" + ans.Posts[i].Id + "'>" +
                            "<input class='btn btn-danger' type='submit' value='Удалить'>" +
                        "</form>"
                        : "") +
                    "</div>"
                );
        });
    }, 10000);
});
