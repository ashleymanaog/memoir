function handleMediaChange(input) {
    var postMediaDiv = document.getElementById("postMediaDiv");
    postMediaDiv.innerHTML = ""; // Clear existing previews

    Array.from(input.files).forEach((file, index) => {
        var previewDiv = document.createElement("div");
        previewDiv.className = file.type.startsWith("image") ? "postImgDiv" : "postVidDiv";
        previewDiv.innerHTML = `
            ${file.type.startsWith("image")
                ?
                `<div class="postImgDiv">
                    <div class="hoverImg">
                        <div class="btnEditPicDiv">
                            <label class="fa-solid fa-pen-to-square btn btnEditPic" for="uploadMedia_${index}" onclick="editMedia(${index})"  value="${file.name}"></label>
                            <button class="fa-solid fa-xmark btn btnRemovePic" onclick="removeMedia(${index})"></button>
                        </div>
                        <img src="${URL.createObjectURL(file)}" class="postActualImg" id="post-previewImg_${index}">
                    </div>
                </div>`
                :
                `<div class="postVidDiv">
                    <div class="hoverVid">
                        <div class="btnEditVidDiv">
                            <label class="fa-solid fa-pen-to-square btn btnEditVid" for="uploadMedia_${index}" onclick="editMedia(${index})"  value="${file.name}"></label>
                            <button class="fa-solid fa-xmark btn btnRemoveVid" onclick="removeMedia(${index})"></button>
                        </div>
                        <video controls data-touch="false" class="embed-responsive" autoplay muted id="post-previewVid_${index}">
                            <source src="${URL.createObjectURL(file)}" alt="Post Media">
                            Your browser does not support the video tag.
                        </video>
                    </div>
                </div>`}
        `;

        postMediaDiv.appendChild(previewDiv);
    });

    // Make add icon visiblle
    var filesInput = document.getElementById("filesInput");
    var addMediaLabel = document.getElementById("addMedia");
    if (filesInput.files.length > 0) {
        addMediaLabel.style.display = "inline-block";
    } else {
        addMediaLabel.style.display = "none";
    }
}

function removeMedia(indexToRemove) {
    var input = document.getElementById("filesInput");
    var filesArray = Array.from(input.files); // convert FileList to array
    filesArray.splice(indexToRemove, 1); // remove
    // Create a new FileList from the updated array
    var newFileList = new DataTransfer();
    filesArray.forEach(file => newFileList.items.add(file));
    input.files = newFileList.files; //update
    handleMediaChange(input)
    console.log(indexToRemove)
    console.log("Remaining Files:", Array.from(input.files).map(file => file.name));
}

function editMedia(indexToEdit) {
    console.log("Editing media at index:", indexToEdit);

    var filesInput = document.getElementById("filesInput");
    var input = document.createElement("input");
    input.type = "file";
    input.accept = "image/*,video/*";
    input.style.display = "none";
    
    input.addEventListener("change", function () {
        var editedFile = input.files[0];
        if (editedFile) {
            console.log("New File: ", editedFile)
            console.log("To Update: ", Array.from(filesInput.files).map(file => file.name))
            // Update the file at the specified index
            var filesArray = Array.from(filesInput.files);
            filesArray[indexToEdit] = editedFile;
            // Create a new FileList from the updated array
            var newFileList = new DataTransfer();
            filesArray.forEach(file => newFileList.items.add(file));
            // Update the original filesInput
            filesInput.files = newFileList.files;
            handleMediaChange(filesInput);
            console.log("Updated files: ", Array.from(filesInput.files).map(file => file.name))
        }
    });

    input.click();
}

function addMedia() {
    var input = document.createElement("input");
    input.type = "file";
    input.accept = "image/*,video/*";
    input.multiple = true;
    input.style.display = "none";

    input.addEventListener("change", function () {
        var filesInput = document.getElementById("filesInput");
        var newFiles = Array.from(input.files);
        var allFiles = Array.from(filesInput.files).concat(newFiles); //combine with exisitng
        // Create a new FileList from the combined array
        var newFileList = new DataTransfer();
        allFiles.forEach(file => newFileList.items.add(file));
        filesInput.files = newFileList.files; // update filesInput
        handleMediaChange(filesInput);
    });

    input.click();
}