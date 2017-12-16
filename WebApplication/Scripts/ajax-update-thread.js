function ajaxUpdate() {
    var refreshButton = document.querySelector("#refresh");
    try {
        ajax.post(
            refreshButton.dataset.url,
            {
                threadId: refreshButton.dataset.threadId,
                currentCount: document.querySelectorAll(".post").length
            },
            function (rawResponse) {
                var answer = JSON.parse(rawResponse);
                var posts = document.querySelectorAll(".post");
                for (var i = 0; i < answer.Posts.length; i++) {
                    var header = answer.Posts[i].Username + " " + answer.Posts[i].Timestamp + " #" + answer.Posts[i].Index + " " + "No." + answer.Posts[i].Id;
                    posts[posts.length - 1].insertAdjacentHTML("afterend",
                        "<div class='post-container'>" +
                        "<div class='post'>" +
                        "<div class='post-header'>" + header + "</div>" +
                        "<div class='post-message'>" + answer.Posts[i].Text + "</div>" +
                        "</div>" +
                        "</div>"
                    );
                }
            }
        );
    }
    catch (e) {
    }
}

function reset(form) {
    form.reset();
    grecaptcha.reset();
}

function postReply(event) {
    event.preventDefault();
    var refreshButton = document.querySelector("#refresh");
    ajax.post(
        "AddPost",
        {
            threadId: refreshButton.dataset.threadId,
            name: event.target[2].value,
            text: event.target[3].value,
            captchaResponse: grecaptcha.getResponse()
        },
        function (rawResponse) {
            reset(event.target);
            ajaxUpdate(rawResponse);
        });
    console.log(event);
}

function handler() {
    setInterval(ajaxUpdate, 10000);
    document.querySelector("#form").onsubmit = postReply;
}

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
}