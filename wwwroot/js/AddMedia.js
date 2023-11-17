function addMediaInput(postIndex) {
    var fileInput = document.createElement('input');
    fileInput.type = 'file';
    fileInput.name = `PostsWithDetails[${postIndex}].PostMedia`;
    fileInput.multiple = true;

    // Append the new file input after the existing ones
    var container = document.querySelector(`[name="PostsWithDetails[${postIndex}].PostMedia"]`);
    container.parentNode.insertBefore(fileInput, container.nextSibling);
}
/*
function addMediaInput(postIndex) {
    // Check if an existing file input is present
    var existingFileInput = document.querySelector(`[name="PostsWithDetails[${postIndex}].PostMedia"]`);

    if (existingFileInput) {
        // Remove the existing file input
        existingFileInput.parentNode.removeChild(existingFileInput);
    }

    // Create a new file input
    var fileInput = document.createElement('input');
    fileInput.type = 'file';
    fileInput.name = `PostsWithDetails[${postIndex}].PostMedia`;
    fileInput.multiple = true;

    // Append the new file input after the existing ones
    var container = document.querySelector(`[name="PostsWithDetails[${postIndex}].PostMedia"]`);
    container.parentNode.insertBefore(fileInput, container.nextSibling);
}


*/