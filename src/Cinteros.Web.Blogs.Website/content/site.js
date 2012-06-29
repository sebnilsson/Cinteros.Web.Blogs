(function (window, document) {
    'use strict';

    $(function () {
        cinterosBlog.initFancyBox();
    });

    var cinterosBlog = {
        initFancyBox: function () {
            $('.post-content a:has(img)').fancybox({
                openEffect	: 'elastic',
    	        closeEffect	: 'elastic',

    	        helpers: {
    	            title: {
    	                type: 'inside'
    	            }
    	        }
            });
        }
    };
})(window, document, undefined);