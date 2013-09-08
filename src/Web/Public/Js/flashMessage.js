(function ($, window) {

    "use strict";

    var addFlashMessage = function (title, message, type, container) {

        // Default to type 'info' if type is not specified or valid
        if ($.inArray(type, ['success', 'info', 'warning', 'danger']) == -1) {
            type = 'info';
        }

        title = title == '' ? '' : '<strong>' + title + ' - </strong>';

        var messageTemplate =   '<div class="alert alert-' + type + ' alert-block" onclick="$(this).slideUp()">' +
                                    title + 
                                    message +
                                '</div>';

        if (container == '' || container == null) {
            $(messageTemplate).appendTo('#flashMessages');
        } else {
            $(messageTemplate).appendTo(container);
        }
    };

    window.AddFlashMessage = addFlashMessage;

})($, window);