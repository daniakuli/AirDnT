$(function () {
    $("#makeTweet").click(function () {
        var message = $("#tweetID").val();
        if (message != "") {
            $.ajax({
                url: "/Apartments/MakeTweet",
                data: { tweet: message },
                success: function (response) {
                    alert("Twit have been published!");
                    $("#tweetID").val("");
                }
            })
        }
    })
})