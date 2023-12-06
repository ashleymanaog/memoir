function handleMediaChange(postId, input, postIndex) {
    var editedMediaIdsInput = document.querySelector("#editedMediaIds_" + postId);
    var deletedMediaIdsInput = document.querySelector("#deletedMediaIds_" + postId);

    var postMediaDiv = document.body.querySelector("#postMediaDiv_" + postId);
    postMediaDiv.innerHTML = ""; // Clear existing previews

    var editedMediaIdsArray = editedMediaIdsInput.value.split(',');
    var deletedMediaIdsArray = deletedMediaIdsInput.value.split(',');

    console.log("postId: ", postId);
    console.log("userMedia: ", userMedia);
    var myPostIndex = postIndex.find(item => item.postId === parseInt(postId))?.counter;
    console.log("myPostIndex", myPostIndex)
    var userMediaForPost = userMedia[myPostIndex];
    var mediaArrayForPost = userMediaForPost ? userMediaForPost.$values : [];
    console.log("userMediaForPost: ", userMediaForPost);
    console.log("mediaArrayForPost: ", mediaArrayForPost);

    console.log("filesInput: ", Array.from(input.files).map(file => file.name));
    console.log("deletedMediaIdsArray: ", deletedMediaIdsArray);
    console.log("editedMediaIdsArray: ", editedMediaIdsArray);

    // Display existing media based on editedMediaIds and deletedMediaIds
    mediaArrayForPost.forEach((media, index) => {
        console.log("media: ", media)
        if (!deletedMediaIdsArray.includes(media.mediaId.toString().trim())) {
            console.log("Media Id: ", media.mediaId)
            console.log("Index: ", index)
            console.log(!deletedMediaIdsArray.includes(media.mediaId.toString().trim()));
            console.log(editedMediaIdsArray.includes(media.mediaId.toString().trim()));
            if (editedMediaIdsArray.includes(media.mediaId.toString().trim())) {
                console.log("inside if")
                // Display the edited file or contents of filesInput
                var indexOfMediaId = editedMediaIdsArray.indexOf(media.mediaId.toString().trim());
                console.log("indexOfMediaId: ", indexOfMediaId)
                var fileFromInput = input.files[indexOfMediaId];
                console.log("fileFromInput: ", fileFromInput)
                var previewDiv = createPreviewDivEdited(postId, fileFromInput, index, media);
                postMediaDiv.appendChild(previewDiv);
            } else {
                console.log("else")
                // Display existing media not marked for deletion or edit
                var previewDiv = createPreviewDivExisting(postId, media, index);
                console.log("back to else previewDiv: ", previewDiv);
                postMediaDiv.appendChild(previewDiv);
            }
        }
    });

    // Show New Selected Files that are not edited
    // Store indexes of editedMediaIdsArray with mediaId of "0"
    var mediaIdZeroIndexes = [];
    editedMediaIdsArray.forEach((mediaId, index) => {
        if (mediaId === "0") {
            mediaIdZeroIndexes.push(index);
        }
    });
    console.log("mediaIdZeroIndexes; ", mediaIdZeroIndexes);
    // Iterate over the stored indexes
    mediaIdZeroIndexes.forEach(index => {
        var file = input.files[index];
        var previewDiv = createPreviewDivNew(postId, file, index);
        console.log("back to newFiles if previewDiv: ", previewDiv);
        postMediaDiv.appendChild(previewDiv);
    });

    // Make add icon visible
    var filesInput = document.querySelector("#filesInput_" + postId);
    var addMediaLabel = document.body.querySelector("#addMedia_" + postId);
    console.log("fileInput length; ", filesInput.files.length > 0);
    console.log("mediArray length: ", mediaArrayForPost.length > 0);
    if (filesInput.files.length > 0 || mediaArrayForPost.length > 0) {
        addMediaLabel.style.display = "inline-block";
    } else {
        addMediaLabel.style.display = "none";
    }
}

function addIconVisibility(postId, postIndex) {
    // Make add icon visible
    var filesInput = document.querySelector("#filesInput_" + postId);
    var addMediaLabel = document.body.querySelector("#addMedia_" + postId);

    var myPostIndex = postIndex.find(item => item.postId === parseInt(postId))?.counter;
    var userMediaForPost = userMedia[myPostIndex];
    var mediaArrayForPost = userMediaForPost ? userMediaForPost.$values : [];

    console.log("fileInput length; ", filesInput.files.length > 0);
    console.log("mediArray length: ", mediaArrayForPost.length > 0);
    if (filesInput.files.length > 0 || mediaArrayForPost.length > 0) {
        addMediaLabel.style.display = "inline-block";
    } else {
        addMediaLabel.style.display = "none";
    }
}

function createPreviewDivEdited(postId, media, index, oldMedia) {
    console.log("reached createPreviewDivNew")
    console.log("postId passed: ", postId)
    console.log("file passed: ", media)
    console.log("index passed: ", index)
    console.log("media type: ", media.type)
    console.log("postindex: ",postIndex)
    var previewDiv = document.createElement("div");
    console.log("previewDivEmpty: ", previewDiv)
    previewDiv.className = media.type.startsWith("image") ? "postImgDiv" : "postVidDiv";
    console.log("previewDivEmpty2: ", previewDiv)
    previewDiv.innerHTML = `
        ${media.type.startsWith("image")
        ?
        `<div class="postImgDiv">
            <div class="hoverImg">
                <div class="btnEditPicDiv">
                    <label class="fa-solid fa-pen-to-square btn btnEditPic" for="uploadMedia_${index}" onclick="editMedia(${postId}, ${index}, '${oldMedia.mediaId}',  postIndex)"  value="${oldMedia.mediaId}"></label>
                    <button class="fa-solid fa-xmark btn btnRemovePic" onclick="removeMedia(${postId}, ${index}, ${oldMedia.mediaId}, postIndex)"></button>
                </div>
                <img src="${URL.createObjectURL(media)}" class="postActualImg" id="post-previewImg_${index}">
            </div>
        </div>`
        :
        `<div class="postVidDiv">
            <div class="hoverVid">
                <div class="btnEditVidDiv">
                    <label class="fa-solid fa-pen-to-square btn btnEditVid" for="uploadMedia_${index}" onclick="editMedia(${postId}, ${index}, '${oldMedia.mediaId}', postIndex)"  value="${oldMedia.mediaId}"></label>
                    <button class="fa-solid fa-xmark btn btnRemoveVid" onclick="removeMedia(${postId}, ${index}, ${oldMedia.mediaId}, postIndex)"></button>
                </div>
                <video controls data-touch="false" class="embed-responsive" autoplay muted id="post-previewVid_${index}">
                    <source src="${URL.createObjectURL(media)}" alt="Post Media">
                    Your browser does not support the video tag.
                </video>
            </div>
        </div>`}
    `;
    console.log("previewDiv: ", previewDiv)
    return previewDiv;
}

function createPreviewDivNew(postId, media, index) {
    console.log("reached createPreviewDivNew")
    console.log("postId passed: ", postId)
    console.log("file passed: ", media)
    console.log("index passed: ", index)
    console.log("media type: ", media.type)
    console.log("postindex: ", postIndex)
    var previewDiv = document.createElement("div");
    console.log("previewDivEmpty: ", previewDiv)
    previewDiv.className = media.type.startsWith("image") ? "postImgDiv" : "postVidDiv";
    console.log("previewDivEmpty2: ", previewDiv)
    previewDiv.innerHTML = `
        ${media.type.startsWith("image")
        ?
        `<div class="postImgDiv">
        <div class="hoverImg">
            <div class="btnEditPicDiv">
                <label class="fa-solid fa-pen-to-square btn btnEditPic" for="uploadMedia_${index}" onclick="editMedia(${postId}, ${index}, '0',  postIndex)"  value="0"></label>
                <button class="fa-solid fa-xmark btn btnRemovePic" onclick="removeMedia(${postId}, ${index}, '0', postIndex)"></button>
            </div>
            <img src="${URL.createObjectURL(media)}" class="postActualImg" id="post-previewImg_${index}">
        </div>
        </div>`
        :
        `<div class="postVidDiv">
        <div class="hoverVid">
            <div class="btnEditVidDiv">
                <label class="fa-solid fa-pen-to-square btn btnEditVid" for="uploadMedia_${index}" onclick="editMedia(${postId}, ${index}, '0', postIndex)"  value="0"></label>
                <button class="fa-solid fa-xmark btn btnRemoveVid" onclick="removeMedia(${postId}, ${index}, '0', postIndex)"></button>
            </div>
            <video controls data-touch="false" class="embed-responsive" autoplay muted id="post-previewVid_${index}">
                <source src="${URL.createObjectURL(media)}" alt="Post Media">
                Your browser does not support the video tag.
            </video>
        </div>
        </div>`}
    `;
    console.log("previewDiv: ", previewDiv)
    return previewDiv;
}

function createPreviewDivExisting(postId, media, index) {
    console.log("reached createPreviewDivExisting")
    console.log("postId passed: ", postId)
    console.log("media passed: ", media)
    console.log("index passed: ", index)
    console.log("media type: ", media.mediaType)
    console.log("postindex: ", postIndex)
    var previewDiv = document.createElement("div");
    console.log("previewDivEmpty: ", previewDiv)
    previewDiv.className = media.mediaType === "Image" ? "postImgDiv" : "postVidDiv";
    console.log("previewDivEmpty2: ", previewDiv)
    previewDiv.innerHTML = `
        ${media.mediaType === "Image"
        ?
        `<div class="postImgDiv">
            <div class="hoverImg">
                <div class="btnEditPicDiv">
                    <label class="fa-solid fa-pen-to-square btn btnEditPic" for="uploadMedia_${index}" onclick="editMedia(${postId}, ${index}, '${media.mediaId}',  postIndex)"  value="${media.mediaId}"></label>
                    <button class="fa-solid fa-xmark btn btnRemovePic" onclick="removeMedia(${postId}, ${index}, ${media.mediaId}, postIndex)"></button>
                </div>
                <img src="${baseUrl + media.mediaPath}" class="postActualImg" id="post-previewImg_${index}">
            </div>
        </div>`
        :
        `<div class="postVidDiv">
            <div class="hoverVid">
                <div class="btnEditVidDiv">
                    <label class="fa-solid fa-pen-to-square btn btnEditVid" for="uploadMedia_${index}" onclick="editMedia(${postId}, ${index}, '${media.mediaId}', postIndex)"  value="${media.mediaId}"></label>
                    <button class="fa-solid fa-xmark btn btnRemoveVid" onclick="removeMedia(${postId}, ${index}, ${media.mediaId}, postIndex)"></button>
                </div>
                <video controls data-touch="false" class="embed-responsive" autoplay muted id="post-previewVid_${index}">
                    <source src="${baseUrl + media.mediaPath}" alt="Post Media">
                    Your browser does not support the video tag.
                </video>
            </div>
        </div>`}
    `;
    console.log("previewDiv: ", previewDiv)
    return previewDiv;
}

function removeMedia(postId, indexToRemove, mediaId, postIndex) {
    var editedMediaIdsInput = document.querySelector("#editedMediaIds_" + postId);
    var deletedMediaIdsInput = document.body.querySelector("#deletedMediaIds_" + postId);

    var editedMediaIdsArray = editedMediaIdsInput.value.split(',');
    var input = document.body.querySelector("#filesInput_" + postId);

    console.log("remove mediaId check: ", mediaId);
    console.log(mediaId.toString() != '0');
    console.log("is edited: ", editedMediaIdsArray.includes(mediaId.toString().trim()));
    
    // Existing Media
    if (mediaId.toString() != '0' && mediaId.toString != '') {
        // Edited Existing Media
        if (editedMediaIdsArray.includes(mediaId.toString().trim())) {
            var indexOfMediaId = editedMediaIdsArray.indexOf(mediaId.toString().trim());
            console.log("index: ", indexOfMediaId);
            console.log("index !== -1 is: ", indexOfMediaId !== -1);
            if (indexOfMediaId !== -1) {
                console.log("editedMediaIdsArray before delete:", editedMediaIdsArray);
                console.log("filesInput before delete:", Array.from(input.files).map(file => file.name));

                // Remove from editedMediaIds
                editedMediaIdsArray.splice(indexOfMediaId, 1);
                editedMediaIdsInput.value = editedMediaIdsArray.join(',');
                // Remove from filesInput
                var filesArray = Array.from(input.files);
                filesArray.splice(indexOfMediaId, 1);
                var newFileList = new DataTransfer();
                filesArray.forEach(file => newFileList.items.add(file));
                input.files = newFileList.files;

                console.log("editedMediaIdsArray after delete:", editedMediaIdsArray);
            }
        }
        // Existing & Edited Media
        deletedMediaIdsInput.value += mediaId + ",";
    } else { // New Files
        var filesArray = Array.from(input.files); // convert FileList to array

        // Get actualIndexToRemove (in filesInput)
        var existingEditedMediaCount = editedMediaIdsArray.filter(mediaId => mediaId !== '0' && mediaId !== '' && mediaId !== null && mediaId !== undefined).length;
        var myPostIndex = postIndex.find(item => item.postId === parseInt(postId))?.counter;
        console.log("myPostIndex", myPostIndex)
        var userMediaForPost = userMedia[myPostIndex];
        var mediaArrayForPost = userMediaForPost ? userMediaForPost.$values : [];
        var existingMediaCount = mediaArrayForPost.length;
        var actualIndexToRemove = indexToRemove - existingMediaCount + existingEditedMediaCount;

        console.log("filesInput before delete:", Array.from(input.files).map(file => file.name));
        console.log("actualIndexToRemove: ", actualIndexToRemove);

        // Remove from editedMediaIds
        editedMediaIdsArray.splice(actualIndexToRemove, 1);
        editedMediaIdsInput.value = editedMediaIdsArray.join(',');

        // Remove from filesInput
        filesArray.splice(actualIndexToRemove, 1); // remove
        // Create a new FileList from the updated array
        var newFileList = new DataTransfer();
        filesArray.forEach(file => newFileList.items.add(file));
        input.files = newFileList.files; //update
    }
    handleMediaChange(postId, input, postIndex);
    console.log("Remaining Files after delete:", Array.from(input.files).map(file => file.name));
}

function editMedia(actualPostId, indexToEdit, mediaId, postIndex) {
    console.log("index Edit:", indexToEdit);
    console.log("postId Edit:", actualPostId);
    console.log("mediaId Edit:", mediaId); 

    var filesInput = document.body.querySelector("#filesInput_" + actualPostId);
    var input = document.createElement("input");
    input.type = "file";
    input.accept = "image/*,video/*";
    input.style.display = "none";
    
    var editedMediaIdsInput = document.body.querySelector("#editedMediaIds_" + actualPostId);
    // Check if mediaId already exists (for exisitng media)
    if (editedMediaIdsInput.value.includes(mediaId) && mediaId.trim() !== "" && mediaId.trim() !== "0") {
        var existingIndex = editedMediaIdsInput.value.split(',').indexOf(mediaId);
        if (existingIndex !== -1) {
            input.addEventListener("change", function () {
                var editedFile = input.files[0];
                if (editedFile) {
                    console.log("New File: ", editedFile);
                    console.log("To Update: ", Array.from(filesInput.files).map(file => file.name));

                    var editedMediaIdsArray = editedMediaIdsInput.value.split(',');
                    editedMediaIdsArray[existingIndex] = mediaId;
                    editedMediaIdsInput.value = editedMediaIdsArray.join(',');

                    // Update the file at the specified index
                    var filesArray = Array.from(filesInput.files);
                    filesArray[existingIndex] = editedFile;
                    // Create a new FileList from the updated array
                    var newFileList = new DataTransfer();
                    filesArray.forEach(file => newFileList.items.add(file));
                    // Update the original filesInput
                    filesInput.files = newFileList.files;
                    console.log("Updated files: ", Array.from(filesInput.files).map(file => file.name));
                    handleMediaChange(actualPostId, filesInput, postIndex);
                }
            });

            input.click();
        }
    } else if (mediaId.trim() == "" || mediaId.trim() == "0") {
        input.addEventListener("change", function () {
            var editedFile = input.files[0];
            if (editedFile) {
                console.log("New File else: ", editedFile);
                console.log("To Update else: ", Array.from(filesInput.files).map(file => file.name));
                
                // Update the file at the specified index
                var filesArray = Array.from(filesInput.files);
                filesArray[indexToEdit] = editedFile;
                // Create a new FileList from the updated array
                var newFileList = new DataTransfer();
                filesArray.forEach(file => newFileList.items.add(file));
                // Update the original filesInput
                filesInput.files = newFileList.files;
                console.log("Updated files else: ", Array.from(filesInput.files).map(file => file.name));
                handleMediaChange(actualPostId, filesInput, postIndex);
            }
        });

        input.click();
    } else {
        input.addEventListener("change", function () {
            var editedFile = input.files[0];
            if (editedFile) {
                console.log("New File else: ", editedFile);
                console.log("To Update else: ", Array.from(filesInput.files).map(file => file.name));

                editedMediaIdsInput.value += mediaId + ",";
                //filesArray[indexToEdit] = editedFile;
                filesArray = Array.from(filesInput.files).concat(editedFile);
                // Create a new FileList from the updated array
                var newFileList = new DataTransfer();
                filesArray.forEach(file => newFileList.items.add(file));
                // Update the original filesInput
                filesInput.files = newFileList.files;
                console.log("Updated files else: ", Array.from(filesInput.files).map(file => file.name));
                handleMediaChange(actualPostId, filesInput, postIndex);
            }
        });
        
        input.click();
    }
}

function addMedia(postId, postIndex) {
    var input = document.createElement("input");
    input.type = "file";
    input.accept = "image/*,video/*";
    input.multiple = true;
    input.style.display = "none";

    input.addEventListener("change", function () {
        var filesInput = document.body.querySelector("#filesInput_" + postId);
        var editedMediaIdsInput = document.body.querySelector("#editedMediaIds_" + postId);

        // Add "0" for each new file in editedMediaIdsInput
        for (let i = 0; i < input.files.length; i++) {
            editedMediaIdsInput.value += "0,";
        }

        var newFiles = Array.from(input.files);
        var allFiles = Array.from(filesInput.files).concat(newFiles); // combine with existing
        // Create a new FileList from the combined array
        var newFileList = new DataTransfer();
        allFiles.forEach(file => newFileList.items.add(file));
        // Update filesInput
        filesInput.files = newFileList.files;
        handleMediaChange(postId, filesInput, postIndex);
    });

    input.click();
}

function addMediaFromEmpty(postId, postIndex) {
    var input = document.body.querySelector("#filesInput_" + postId);
    var editedMediaIdsInput = document.querySelector("#editedMediaIds_" + postId);
    var deletedMediaIdsInput = document.querySelector("#deletedMediaIds_" + postId);

    // Empty the values
    input.value = "";
    editedMediaIdsInput.value = "";
    deletedMediaIdsInput.value = "";

    input.addEventListener("change", function () {
        var editedMediaIdsInput = document.body.querySelector("#editedMediaIds_" + postId);

        // Add "0" for each new file in editedMediaIdsInput
        for (let i = 0; i < input.files.length; i++) {
            editedMediaIdsInput.value += "0,";
        }

        var allFiles = Array.from(input.files)
        // Create a new FileList from the combined array
        var newFileList = new DataTransfer();
        allFiles.forEach(file => newFileList.items.add(file));
        // Update filesInput
        input.files = newFileList.files;
        handleMediaChange(postId, input, postIndex);
    });
}