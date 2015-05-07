define(['trio'], function ($) {
    $.fn.extend({
        itspTableBinder: function () {
            var that = $(this);
            var formid = $(this).attr('id');
            $("a[data-event='submit'][data-target='" + formid + "']").on('click', function () {
                that.trigger('submit');
            });
            $("a[data-event='reset'][data-target='" + formid + "']").on('click', function () {
                document.getElementById(formid).reset();
                that.trigger('submit');
            });
        }
    });
    return $;
});