function displayOrHidePhone() {

    var type = document.getElementById("RoleType");


    if (type.value == "All Members") {

        document.getElementById("Phone").style.visibility = "hidden";
    }
    else
    {
        document.getElementById("Phone").style.visibility = "visible";
    }
}

function displayOrHideTo() {

    var type = document.getElementById("RoleType");


    if (type.value == "All Members") {

        document.getElementById("To").style.visibility = "hidden";
    }
    else {
        document.getElementById("To").style.visibility = "visible";
    }
}

function SearchTotalAmountByType() {

    var type = document.getElementById("SearchType");


    if ((type.value == "Members") || (type.value == "Non Members")) {

        document.getElementById("Text").style.visibility = "hidden";
    }
    else {
        document.getElementById("Text").style.visibility = "visible";
    }
}
