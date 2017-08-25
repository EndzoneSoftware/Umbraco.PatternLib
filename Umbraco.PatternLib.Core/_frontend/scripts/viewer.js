/* Viewer initialization */

(function ($) {

    $('#viewer-size-controls a[data-size-class]').on('click', function (e) {
        e.preventDefault();

        // find viewer cell
        var viewerCell = $('#viewer-cell');

        if (viewerCell.length === 0) {
            return;
        }

        // toggle viewer size class
        var sizeClass = $(this).data('size-class');

        viewerCell
            .removeClass('viewer-cell--small viewer-cell--medium viewer-cell--large viewer-cell--full')
            .addClass(sizeClass);

        // update viewer size controls state
        $(this).siblings('a[data-size-class]').addClass('secondary');
        $(this).removeClass('secondary');
    });

    $('#viewer-new-window').on('click', function (e) {
        e.preventDefault();

        // find viewer iframe
        var viewerFrame = $('#viewer-frame');

        if (viewerFrame.length === 0) {
            return;
        }

        // get iframe URL
        var src = viewerFrame.attr('src');

        if (!src || src.length === 0) {
            return;
        }

        // open iframe URL in new window
        window.open(src, '_blank');
        window.focus();
    });
    

})(window.jQuery);
