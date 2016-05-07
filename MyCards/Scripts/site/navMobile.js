var isNavOpen = false;

function openCloseNav(forceClose) {
    if (isNavOpen || forceClose){

        isNavOpen = false;

        document.getElementById("sidenav").style.width = "0";
        document.getElementById("mainMobile").style.marginRight = "0";
        document.getElementById("openNav").style.right = "0";
    }
    else {

        isNavOpen = true;

        var sidebarSize = window.innerWidth * 0.75;

        document.getElementById("sidenav").style.width = sidebarSize + 'px';
        document.getElementById("mainMobile").style.marginLeft = sidebarSize + 'px';
        document.getElementById("openNav").style.right = sidebarSize + 'px';
    }

}

$(document).on('click', '#mobile', function () {

    var clickedItem = $(event.target).first();

    // Check if need to close nav
    if ((!clickedItem.hasClass('c-rating__item')) && // Star click
        (!clickedItem.hasClass('mDontCls'))       &&
        (!clickedItem.closest("div").hasClass('toggle-group')) // Checkbox
        )
    {
        openCloseNav(true);
    }
});