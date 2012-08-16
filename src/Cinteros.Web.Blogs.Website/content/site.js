(function (window, document) {
    'use strict';

    $(function () {
        cinterosBlog.initExpandPost();
        cinterosBlog.initFancyBox();
    });

    var cinterosBlog = {
        maxPostHeight: 300,
        initExpandPost: function () {
            $('.post-content').each(function (index, item) {
                var $item = $(item);
                var itemHeight = $item.height();
                if (itemHeight > cinterosBlog.maxPostHeight) {
                    $('.post-comment', $item.parent()).hide();

                    $item.addClass('expandible').height(cinterosBlog.maxPostHeight);
                    $item.attr('data-org-height', itemHeight);

                    $item.after('<div class="post-expand"><span class="post-expand-button"><a href="#">Visa hela</a></span></div>');
                }
            });

            $('.post-expand-button').click(function (event) {
                event.preventDefault();

                var $button = $(this);
                var $comments = $button.parent().next('.post-comment');
                var $postContent = $button.parent().prev('.post-content');

                var orgHeight = $postContent.attr('data-org-height');

                $postContent.removeClass('expandible');
                $postContent.animate({
                    height: orgHeight
                }, 300);

                $button.parent().remove();
                $comments.show();

                setTimeout(function () {
                    $postContent.height('');
                }, 1000);
            });
        },

        initFancyBox: function () {
            $('.post-content a:has(img)').fancybox({
                openEffect: 'elastic',
                closeEffect: 'elastic',

                helpers: {
                    title: {
                        type: 'inside'
                    }
                }
            });
        }
    };
})(window, document, undefined);