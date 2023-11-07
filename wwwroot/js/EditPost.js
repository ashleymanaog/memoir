//When btnEditPic clicked
function editPic() {
    //Upload Photo Script
    document.querySelector("#uploadPhoto").addEventListener("change", function (e) {
        if (e.target.files.length == 0) { return; }
        let file = e.target.files[0];
        let url = URL.createObjectURL(file);

        $("#postActualImg").fadeIn("fast").attr('src', url);//URL.createObjectURL(event.target.files[0])
        $("#uploadPhoto").val("");
    });
    //Upload Photo Script (2nd & 3rd Image)
    document.querySelector("#uploadPhoto2").addEventListener("change", function (e) {
        if (e.target.files.length == 0) { return; }
        let file = e.target.files[0];
        let url = URL.createObjectURL(file);

        $("#postActualImg2").fadeIn("fast").attr('src', url);//URL.createObjectURL(event.target.files[0])
        $("#uploadPhoto2").val("");
    });
}
//When btnSavePost is clicked
$('#btnSavePost').click(function () {
    
});

//When btnRemovePic clicked
$(".btnRemovePic").hover(function (event) {
    $(this).addClass("active");
    event.stopPropagation();
}, function (event) {
    $(this).removeClass("active");
    event.stopPropagation();
});

$('.postImgDiv').on("click", '.btnRemovePic', function (event) {
    //alert('has active: '+ $(this).hasClass('active'))
    //alert('has noDisplay: ' + $(this).hasClass('noDisplay'))
    $(this).closest('.postImgDiv').addClass('noDisplay');
});