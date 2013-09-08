// http://stackoverflow.com/a/10701856/426840
// Automatically cancel unfinished ajax requests 
// when the user navigates elsewhere.
(function ($) {
    var xhrPool = [];

    $(document).ajaxSend(function (e, jqXhr) {
        xhrPool.push(jqXhr);
    });

    $(document).ajaxComplete(function (e, jqXhr) {
        if (jqXhr.getResponseHeader('ForceRedirect') === '1') {
            // On the server side redirects on ajax requests are caught and turned to statuscode 200.
            // Also a header is set 'ForceRedirection'. We use that here so we can redirect on the client side.
            // Primary use case is session timeouts, but also regular redirect after post.
            // Based on this SO answer: http://stackoverflow.com/a/16409097/426840
            window.location.replace(jqXhr.getResponseHeader('Location')); // http://stackoverflow.com/q/503093/426840
        } else {
            xhrPool = $.grep(xhrPool, function (x) { return x != jqXhr; });
        }
    });

    var abort = function () {
        $.each(xhrPool, function (idx, jqXhr) {
            jqXhr.abort();
        });
    };

    var oldbeforeunload = window.onbeforeunload;
    window.onbeforeunload = function () {
        var r = oldbeforeunload ? oldbeforeunload() : undefined;
        if (r == undefined) {
            // only cancel requests if there is no prompt to stay on the page
            // if there is a prompt, it will likely give the requests enough time to finish
            abort();
        }
        return r;
    };
})(jQuery);