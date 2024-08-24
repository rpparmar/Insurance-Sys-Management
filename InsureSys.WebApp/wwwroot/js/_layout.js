jQuery(document).ready(function () {

});

jQuery(window).on('beforeunload', function () {
    $('.loading').addClass('show');
});

jQuery(window).on('load', function () {
    var $parent_div = $('li.menu-item-active').closest('div.menu-submenu');
    $parent_div.show();

    var $most_parent_li = $parent_div.closest('li');
    $most_parent_li.addClass('menu-item-here menu-item-open');

    /*
    //// Action to add necessary classes for activated links and its parent nodes.
    var $parent_li;
    var $currentactivatedLink = $('a[href="' + window.location.pathname + '"]');
    $parent_li = $currentactivatedLink.closest('li.menu-item');
    //// if clicked on dashboard or any other link not containig child items then avoid below lines of code.
    if ($parent_li !== undefined && $parent_li.length === 1) {
        $parent_li.addClass('menu-item-active');

        var $parent_div = $currentactivatedLink.closest('div.menu-submenu');
        $parent_div.show();

        var $most_parent_li = $parent_div.closest('li');
        $most_parent_li.addClass('menu-item-here menu-item-open');

    }
    else {
        $('#li_dashboard').addClass('menu-item menu-item-active');
    }
    */
});
