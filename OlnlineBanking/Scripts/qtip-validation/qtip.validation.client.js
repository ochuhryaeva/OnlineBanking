$(document).ready(function () {
    //validation - make sure this is included after jquery.validate.unobtrusive.js
    //unobtrusive validate plugin overrides all defaults, so override them again
    $('.form-ui').each(function () {
        OverrideUnobtrusiveSettings(this);
    });
    //in case someone calls $.validator.unobtrusive.parse, override it also
    var oldUnobtrusiveParse = $.validator.unobtrusive.parse;
    $.validator.unobtrusive.parse = function (selector) {
        oldUnobtrusiveParse(selector);
        $('.form-ui').each(function () {
            OverrideUnobtrusiveSettings(this);
        });
    };
    //replace validation settings function
    function OverrideUnobtrusiveSettings(formElement) {
        var settings = $.data(formElement, 'validator').settings;
        //standard qTip2 stuff copied from sample
        settings.errorPlacement = function (error, element) {
            // Set positioning based on the elements position in the form
            var elem = $(element);


            // Check we have a valid error message
            if (!error.is(':empty')) {
                // Apply the tooltip only if it isn't valid
                elem.filter(':not(.valid)').qtip({
                    overwrite: false,
                    content: error,
                    position: {
                        my: 'left center',  // Position my top left...
                        at: 'right center', // at the bottom right of...
                        viewport: $(window)
                    },
                    show: {
                        event: false,
                        ready: true
                    },
                    hide: false,
                    style: {
                        classes: 'qtip-red' // Make it red... the classic error colour!
                    }
                })
                // If we have a tooltip on this element already, just update its content
                .qtip('option', 'content.text', error);
            }

                // If the error is empty, remove the qTip
            else { elem.qtip('destroy'); }
        };

        settings.success = $.noop;
    }
});