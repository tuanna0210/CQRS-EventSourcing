using Post.Common.DTOs;

namespace Post.Query.API.DTOs
{
    public class NewPostResponse : BaseResponse
    {
        public Guid Id { get; set; }
    }
}
