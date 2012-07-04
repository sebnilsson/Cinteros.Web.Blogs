(function (window, document) {
    'use strict';

    $(function () {
        $('pre:not([class^="brush"])').addClass('brush: plain');

        SyntaxHighlighter.all({
            bloggerMode: true,
            toolbar: false
        });
    });
})(window, document, undefined);