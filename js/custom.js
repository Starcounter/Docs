 var gitbook = gitbook || [];
gitbook.push(function() {
    //Move the search bar
    var search = document.getElementById("book-search-input");
    document.getElementsByClassName("search-dropdown-content")[0].appendChild(search);

    //Set the current option
    var select = document.getElementsByClassName("version-select");
    var options = select[0].childNodes;
    var currentPageIsSet = false;
    for(var i = 0; i < options.length; i++){
    if(window.location.href.indexOf(options[i].innerHTML) != -1 && !currentPageIsSet && options[i].nodeType == 1){
        resetSelected(options);
        options[i].setAttribute("selected", "true");
        currentPageIsSet = true;
    }
    }
});

function resetSelected(options){
    for(var x = 0; x < options.length; x++){
    if(options[x].nodeType == 1){
        options[x].removeAttribute("selected");
    }
    }
}


toggleSearchDropdown = function() {
    document.getElementsByClassName("search-dropdown-content")[0].classList.toggle("show");
    console.log("HERE");
}

function toggleDropdown() {
    document.getElementById("menu-dropdown").classList.toggle("show");
}

// Close the dropdown menu if the user clicks outside of it
window.onclick = function(event) {
    if (!event.target.matches('.dropbtn') && document.getElementsByClassName("big-header-buttons") == null) {
    var dropdowns = document.getElementsByClassName("dropdown-content");
    for (var i = 0; i < dropdowns.length; i++) {
        var openDropdown = dropdowns[i];
        if (openDropdown.classList.contains('show')) {
        openDropdown.classList.remove('show');
        }
    }
    }

    // Not a very beautiful if clause
    if (!event.target.matches(".search-dropdown") &&
        !event.target.matches("#book-search-input input") &&
        !event.target.matches("#book-search-input") &&
        document.getElementsByClassName("big-header-buttons") != null &&
        document.getElementsByClassName("search-dropdown-content")[0].classList.contains("show")){
        document.getElementsByClassName("search-dropdown-content")[0].classList.remove("show");
    }
}

var book = document.getElementsByClassName("book")[0];
var header = document.getElementsByClassName("book-header")[0];
book.appendChild(header);