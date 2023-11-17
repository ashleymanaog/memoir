/*//Show Change Banner Modal
function changeBanner() {
    $('#changeBannerModal').modal('show');
}

$("#uploadBannerPhoto").click(function () {
    $(this).val("");
    $e.preventDefault();
});*/
//Upload Banner Photo Script
document.querySelector("#uploadBannerPhoto").addEventListener("click", function (e) {
    document.getElementById("bannerPic").click();
});
document.querySelector("#bannerPic").addEventListener("change", function (e) {
    if (e.target.files.length == 0) { return; }
    let file = e.target.files[0];
    let url = URL.createObjectURL(file);
    document.querySelector(".fileSelectedBanner").textContent = file.name;
    $("#btnSaveBanner").click(function () {
        $("#uploadBannerPhoto-previewImg").fadeIn("fast").attr('src', url);//URL.createObjectURL(event.target.files[0])
        document.querySelector(".fileSelectedBanner").textContent = "";
        if (document.getElementById('btnChooseBanner').checked) {
            $("#bannerPic").val("");
        }
        if (document.getElementById('btnUploadBanner').checked) {
            var btnEditBannerPic = document.getElementById('btnEditBannerPic')
            if (btnEditBannerPic != null) {
                document.getElementById("changeBannerForm").submit();
            }
        }
    });
});
//Choose Banner Photo Script
var url = "";
var filename = "";
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
        filename = url.substring(url.lastIndexOf('/') + 1, url.length);
        $("#btnSaveBanner").click(function () {
            if (url != "#" && url != "") {
                $("#uploadBannerPhoto-previewImg").fadeIn("fast").attr('src', url);
                $("#defaultBanner").val(filename);
            }
            if (document.getElementById('btnChooseBanner').checked) {
                var btnEditBannerPic = document.getElementById('btnEditBannerPic')
                if (btnEditBannerPic != null) {
                    document.getElementById("changeBannerForm").submit();
                }
            }
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
document.querySelector("#uploadProfilePicture").addEventListener("click", function (e) {
    document.getElementById("profilePic").click();
});
document.querySelector("#profilePic").addEventListener("change", function (e) {
    if (e.target.files.length == 0) { return; }
    let file = e.target.files[0];
    let url = URL.createObjectURL(file);
    document.querySelector(".fileSelectedProfile").textContent = file.name;
    $("#btnSaveProfile").click(function () {
        $("#uploadProfilePicture-previewImg").fadeIn("fast").attr('src', url);
        document.querySelector(".fileSelectedProfile").textContent = "";
        if (document.getElementById('btnChooseAvatar').checked) {
            $("#profilePic").val("");
        }
        if (document.getElementById('btnUploadProfile').checked) {
            var btnEditProfilePic = document.getElementById('btnEditProfilePic')
            if (btnEditProfilePic != null) {
                document.getElementById("changeProfileForm").submit();
            }
        }
    });
});
//Choose Avatar Script
var url = "";
var filename = "";
$(document).ready(function () {
    $(this).click(function () {
        var actEl = document.activeElement;
        for (let i = 1; i < 33; i++) {
            if (actEl.getAttribute("id") == "avatar" + i + "") {
                url = document.getElementById("avatar" + i + "Img").getAttribute("src");
            }
        }
        filename = url.substring(url.lastIndexOf('/') + 1, url.length);
        $("#btnSaveProfile").click(function () {
            if (url != "#" && url != "") {
                $("#uploadProfilePicture-previewImg").fadeIn("fast").attr('src', url);
                $("#defaultAvatar").val(filename);
            }
            if (document.getElementById('btnChooseAvatar').checked) {
                var btnEditProfilePic = document.getElementById('btnEditProfilePic')
                if (btnEditProfilePic != null) {
                    document.getElementById("changeProfileForm").submit();
                }
            }
        });
    });
});