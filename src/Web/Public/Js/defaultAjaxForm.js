// Wrapper for jquery.form plugin. Provides out-of-the-box overlay, ajax-loader and flashmessage on error
// API from jquery.form is still applicable

(function ($) {
    $.fn.defaultAjaxForm = function (options) {

        var defaults = {
            ajaxLoaderUrl: '/Public/Images/ajax-loader.gif',
            ajaxLoaderClass: 'ajaxLoader',
            ajaxFormOverlayClass: 'ajaxFormOverlay',
            errorMessage: 'An unexpected error occured',
            errorTitle: 'Error'
        };

        var self = this;
        self.options = $.extend(defaults, options);

        // Append overlay and ajaxloader to form
        var overlay = $('<div class="' + self.options.ajaxFormOverlayClass + '"></div>').hide().appendTo(self);
        var ajaxLoader = $('<img class="' + self.options.ajaxLoaderClass + '" src="' + self.options.ajaxLoaderUrl + '" alt="" />').hide().appendTo(self);

        // Set position relative to element containing folder, this will be the overlay boundary
        self.parent().css('position', 'relative');

        // Call jquery.form plugin on form
        $(self).ajaxForm({
            beforeSerialize: function (form, ajaxFormOptions) {

                if (!$(self).validate()) {
                    return false;
                }

                // Set overlay and ajaxloader
                $(document.activeElement).blur();
                overlay.fadeIn();
                ajaxLoader.fadeIn();
                
                // Execute beforeSerialize function if specified after fade out
                if (typeof self.options.beforeSerialize == 'function') {
                    self.options.beforeSerialize(form, ajaxFormOptions);
                }
            },
            success: function (data, textStatus, jqXhr) {

                // When a force redirect is set (see main.js) stop further execution
                if (jqXhr.getResponseHeader('ForceRedirect') === '1') {
                    return;
                }

                // Remove overlay and ajaxloader, 
                overlay.fadeOut();
                ajaxLoader.fadeOut(function () {

                    // Execute success function if specified after fade out
                    if (typeof self.options.success == 'function') {
                        self.options.success(data, textStatus, jqXhr);
                    }
                });
            },
            error: function (jqXhr, textStatus, errorThrown) {

                // Remove overlay and ajaxloader, 
                overlay.fadeOut();
                ajaxLoader.fadeOut(function() {

                    // Execute error function if specified after fade out
                    if (typeof self.options.error == 'function') {
                        self.options.error(jqXhr, textStatus, errorThrown);
                    } else {
                        // Show general error message when no error function is specified
                        AddFlashMessage(self.options.errorTitle, self.options.errorMessage, 'error');
                    }
                });
            }
        });
    };
    
})(jQuery);