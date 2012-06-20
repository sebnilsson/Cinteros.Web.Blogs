(function (window, document) {
    'use strict';

    $(function () {
        cinterosBlog.initSyntaxHighlighter(appSettings.basePath);
    });

    var cinterosBlog = {
        initSyntaxHighlighter: function (basePath) {
            SyntaxHighlighter.autoloader(
                'csharp c-sharp ' + basePath + 'libraries/SyntaxHighlighter/shBrushCSharp.js',
                'css ' + basePath + 'libraries/SyntaxHighlighter/shBrushCss.js',
                'js jscript javascript ' + basePath + 'libraries/SyntaxHighlighter/shBrushJScript.js',
                'sql ' + basePath + 'libraries/SyntaxHighlighter/shBrushSql.js',
                'html xhtml xml ' + basePath + 'libraries/SyntaxHighlighter/shBrushXml.js'
            );

            SyntaxHighlighter.all({
                bloggerMode: true,
                toolbar: false
            });
        }
    };
})(window, document, undefined);