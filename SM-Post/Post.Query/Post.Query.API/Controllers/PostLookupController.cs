using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Common.DTOs;
using Post.Query.API.DTOs;
using Post.Query.API.Queries;
using Post.Query.Domain.Entities;
using System.Security.Cryptography;

namespace Post.Query.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PostLookupController : ControllerBase
    {
        private readonly ILogger<PostLookupController> _logger;
        private readonly IQueryDispatcher<PostEntity> _queryDispatcher;

        public PostLookupController(ILogger<PostLookupController> logger, IQueryDispatcher<PostEntity> queryDispatcher)
        {
            _logger = logger;
            _queryDispatcher = queryDispatcher;
        }
        [HttpGet]
        public async Task<ActionResult> GetAllPostAsync()
        {
            try
            {
                var posts = await _queryDispatcher.SendAsync(new FindAllPostsQuery());
                if (posts == null || !posts.Any()) return NoContent();
                var count = posts.Count();
                return Ok(new PostLookupResponse()
                {
                    Posts = posts,
                    Message = $"Successfully returned {count} post{(count > 1 ? "s" : string.Empty)}!"
                });
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "Error while processing request to retrieve all post!";
                _logger.LogError(ex, SAFE_ERROR_MESSAGE);
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse()
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }

        [HttpGet("byId/{postId}")]
        public async Task<ActionResult> GetByPostIdAsync(Guid postId)
        {
            try
            {
                var posts = await _queryDispatcher.SendAsync(new FindPostByIdQuery() { Id = postId });
                if (posts == null || !posts.Any()) return NoContent();

                return Ok(new PostLookupResponse()
                {
                    Posts = posts,
                    Message = $"Successfully returned post!"
                });
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "Error while processing request to find post by ID!";
                _logger.LogError(ex, SAFE_ERROR_MESSAGE);
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse()
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }

        [HttpGet("byAuthor/{author}")]
        public async Task<ActionResult> GetPostsByAuthorAsync(string author)
        {
            try
            {
                var posts = await _queryDispatcher.SendAsync(new FindPostsByAuthorQuery() { Author = author });
                if (posts == null || !posts.Any()) return NoContent();

                var count = posts.Count();
                return Ok(new PostLookupResponse()
                {
                    Posts = posts,
                    Message = $"Successfully returned {count} post{(count > 1 ? "s" : string.Empty)}!"
                });
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "Error while processing request to find posts by author!";
                _logger.LogError(ex, SAFE_ERROR_MESSAGE);
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse()
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }

        [HttpGet("withComments")]
        public async Task<ActionResult> GetPostsWithCommentAsync()
        {
            try
            {
                var posts = await _queryDispatcher.SendAsync(new FindPostsWithCommentsQuery() { });
                if (posts == null || !posts.Any()) return NoContent();

                var count = posts.Count();
                return Ok(new PostLookupResponse()
                {
                    Posts = posts,
                    Message = $"Successfully returned {count} post{(count > 1 ? "s" : string.Empty)}!"
                });
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "Error while processing request to find posts with comment!";
                _logger.LogError(ex, SAFE_ERROR_MESSAGE);
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse()
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }

        [HttpGet("withLikes/{numberOfLikes}")]
        public async Task<ActionResult> GetPostsWithLikesAsync(int numberOfLikes)
        {
            try
            {
                var posts = await _queryDispatcher.SendAsync(new FindPostsWithLikesQuery() { NumberOfLikes = numberOfLikes });
                if (posts == null || !posts.Any()) return NoContent();

                var count = posts.Count();
                return Ok(new PostLookupResponse()
                {
                    Posts = posts,
                    Message = $"Successfully returned {count} post{(count > 1 ? "s" : string.Empty)}!"
                });
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "Error while processing request to find posts with likes!";
                _logger.LogError(ex, SAFE_ERROR_MESSAGE);
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse()
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }
    }
}
