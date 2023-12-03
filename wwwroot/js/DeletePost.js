function deletePost(button) {
    var postId = $(button).data('post-id');
    console.log(postId);

    // Add AJAX request to delete the post
    $.ajax({
        url: '/Dashboard/DeletePost',
        type: 'POST',
        data: { postId: postId },
        success: function (result) {
            if (result.success) {
                // Optionally, you can reload the page or update the UI accordingly
                location.reload(); // Reload the page
            } else {
                alert('Failed to delete post. ' + result.error);
            }
        },
        error: function (xhr, status, errorThrown) {
            console.error('Error occurred while deleting the post:', status, errorThrown);
            alert('Error occurred while deleting the post. Check the console for details.');
        }
    });
}
