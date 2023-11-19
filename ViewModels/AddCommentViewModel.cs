namespace ThomasianMemoir.ViewModels
{
    public class AddCommentViewModel
    {
        public int PostId { get; set; }
        public string NewComment { get; set; }
        public int? ParentCommentId { get; set; }
    }
}