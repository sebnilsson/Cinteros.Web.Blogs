$(function () {
    'use strict';
    initFunctions.syntaxHighlighter(appSettings.basePath);
});

var initFunctions = {
    syntaxHighlighter: function (basePath) {
        'use strict';
        SyntaxHighlighter.autoloader(
            'csharp c-sharp ' + basePath + 'libraries/SyntaxHighlighter/shBrushCSharp.js',
            'css ' + basePath + 'libraries/SyntaxHighlighter/shBrushCss.js',
            'js jscript javascript ' + basePath + 'libraries/SyntaxHighlighter/shBrushJScript.js',
            'sql ' + basePath + 'libraries/SyntaxHighlighter/shBrushSql.js',
            'html xhtml xml ' + basePath + 'libraries/SyntaxHighlighter/shBrushXml.js'
        );

        SyntaxHighlighter.all({
            bloggerMode: true,
            //clipboardSwf: 'http://alexgorbatchev.com/pub/sh/current/scripts/clipboard.swf',
            toolbar: false
        });
    }
};