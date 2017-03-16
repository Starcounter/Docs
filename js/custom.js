toggleSearchDropdown = function() {
    document.getElementsByClassName("search-dropdown-content")[0].classList.toggle("show");
}

toggleDropdown = function() {
    document.getElementById("menu-dropdown").classList.toggle("show");
}

var gitbook = gitbook || [];
gitbook.push(function() {
    moveSearchbar();
    setCurrentVersion();
});

moveSearchbar = function() {
    var search = document.getElementById("book-search-input");
    document.getElementsByClassName("search-dropdown-content")[0].appendChild(search);
}

setCurrentVersion = function() {
    var select = document.getElementsByClassName("version-select");
    var options = select[0].childNodes;
    var currentPageIsSet = false;
    for(var i = 0; i < options.length; i++){
        if(shouldSetAsSelectedVersion(options[i]) && !currentPageIsSet){
            resetSelectedVersion(options);
            options[i].setAttribute("selected", "true");
            currentPageIsSet = true;
        }
    }
}

shouldSetAsSelectedVersion = function(option) {
    var isLoadedPage = window.location.href.indexOf(option.innerHTML) != -1;
    var isElementType = option.nodeType == 1;
    return isLoadedPage && isElementType;
}

resetSelectedVersion = function(options){
    for(var x = 0; x < options.length; x++){
        if(options[x].nodeType == 1){
            options[x].removeAttribute("selected");
        }
    }
}

// Close the dropdown menu if the user clicks outside of it
window.onclick = function(event) {
    closeMenuDropdownOnOutsideClick(event);
    closeSearchDropdownOnOutsideClick(event);
}

closeMenuDropdownOnOutsideClick = function(event) {
    if (shouldCloseDropdownMenu(event)) {
        closeDropdownMenu();
    }
}

closeSearchDropdownOnOutsideClick = function(event) {
    if (shouldCloseSearchDropdown(event)){
        closeSearchDropdown();
    }
}

shouldCloseDropdownMenu = function(event) {
    var clickedOutsideDropdownButton = !event.target.matches('.dropbtn');
    var smallMenuIsOpen = window.outerWidth < 1060; 
    console.log(clickedOutsideDropdownButton);
    console.log(smallMenuIsOpen);
    return clickedOutsideDropdownButton && smallMenuIsOpen;
}

shouldCloseSearchDropdown = function(event) {
    var clickedOutsideOfSearchDropdown = !event.target.matches(".search-dropdown") && !event.target.matches("#book-search-input input") && !event.target.matches("#book-search-input");
    var bigHeaderIsOpen = document.getElementsByClassName("big-header-buttons") != null;
    var searchDropdownIsOpen = document.getElementsByClassName("search-dropdown-content")[0].classList.contains("show"); 
    return clickedOutsideOfSearchDropdown && bigHeaderIsOpen && searchDropdownIsOpen;
}

closeDropdownMenu = function() {
    var dropdowns = document.getElementsByClassName("dropdown-content");
    for (var i = 0; i < dropdowns.length; i++) {
        var openDropdown = dropdowns[i];
        if (openDropdown.classList.contains('show')) {
            openDropdown.classList.remove('show');
        }
    }
}

closeSearchDropdown = function() {
    document.getElementsByClassName("search-dropdown-content")[0].classList.remove("show");
}

attachHeader = function() {
    var book = document.getElementsByClassName("book")[0];
    var header = document.getElementsByClassName("book-header")[0];
    book.appendChild(header);
}

attachHeader();


