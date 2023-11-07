/*//Show Change Banner Modal
function changeBanner() {
    $('#changeBannerModal').modal('show');
}
//Upload Banner Photo Script
$("#uploadBannerPhoto").click(function () {
    $(this).val("");
    $e.preventDefault();
});*/
document.querySelector("#uploadBannerPhoto").addEventListener("change", function (e) {
    if (e.target.files.length == 0) { return; }
    let file = e.target.files[0];
    let url = URL.createObjectURL(file);
    document.querySelector(".fileSelectedBanner").textContent = file.name;
    $("#btnSaveBanner").click(function () {
        $("#uploadBannerPhoto-previewImg").fadeIn("fast").attr('src', url);//URL.createObjectURL(event.target.files[0])
        document.querySelector(".fileSelectedBanner").textContent = "";
        $("#uploadBannerPhoto").val("");
    });
});
//Choose Banner Photo Script
var url = "#";
$(document).ready(function () {
    $(this).click(function () {
        var actEl = document.activeElement;
        if (actEl.getAttribute("id") == "yellowBanner") {
            url = document.getElementById("yellowBannerImg").getAttribute("src");
        } else if (actEl.getAttribute("id") == "whiteBanner") {
            url = document.getElementById("whiteBannerImg").getAttribute("src");
        } else if (actEl.getAttribute("id") == "brownBanner") {
            url = document.getElementById("brownBannerImg").getAttribute("src");
        } else if (actEl.getAttribute("id") == "blueBanner") {
            url = document.getElementById("blueBannerImg").getAttribute("src");
        }
        $("#btnSaveBanner").click(function () {
            $("#uploadBannerPhoto-previewImg").fadeIn("fast").attr('src', url);
        });
    });
});

/*//Show Change Profile Modal
function changeProfile() {
    $('#changeProfileModal').modal('show');
}
//Upload Profile Picture Script
$("#uploadProfilePicture").click(function () {
    $(this).val("");
    $e.preventDefault();
});*/
document.querySelector("#uploadProfilePicture").addEventListener("change", function (e) {
    if (e.target.files.length == 0) { return; }
    let file = e.target.files[0];
    let url = URL.createObjectURL(file);
    document.querySelector(".fileSelectedProfile").textContent = file.name;
    $("#btnSaveProfile").click(function () {
        $("#uploadProfilePicture-previewImg").fadeIn("fast").attr('src', url);
        document.querySelector(".fileSelectedProfile").textContent = "";
        $("#uploadProfilePicture").val("");
    });
});
//Choose Avatar Script
var url = "#";
$(document).ready(function () {
    $(this).click(function () {
        var actEl = document.activeElement;
        for (let i = 1; i < 33; i++) {
            if (actEl.getAttribute("id") == "avatar" + i + "") {
                url = document.getElementById("avatar" + i + "Img").getAttribute("src");
            }
        }
        $("#btnSaveProfile").click(function () {
            $("#uploadProfilePicture-previewImg").fadeIn("fast").attr('src', url);
        });
    });
});