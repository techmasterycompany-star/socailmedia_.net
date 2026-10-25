using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social_Media_Platform.API.Helpers;
using Social_Media_Platform.Application.Common.DTOs;
using Social_Media_Platform.Application.DTOS.Post;
using Social_Media_Platform.Application.Interfaces.IServices;
using Social_Media_Platform.Domain.Entities;

namespace Social_Media_Platform.API.Controllers.Posts
{

    [ApiController]
    [Route("api/v1/posts")]
    [Authorize]
    public sealed class PostsController(IPostService postService) : ControllerBase
    {
        [HttpPost]
        [Authorize(Roles = "User")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(210L * 1024 * 1024)]
        public async Task<IActionResult> Create([FromForm] CreatePostRequest request, CancellationToken cancellationToken)
        {
            var userId = User.GetAuthenticatedUserId();
            var post = await postService.CreateAsync(userId, request, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, ApiResponse<PostDto>.SuccessResponse(post));
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
        {
            var post = await postService.GetByIdAsync(id, User.GetAuthenticatedUserId(), cancellationToken);
            return Ok(ApiResponse<PostDto>.SuccessResponse(post));
        }

        [HttpPut("{id:long}")]
        [Authorize(Roles = "User")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(210L * 1024 * 1024)]
        public async Task<IActionResult> Update(long id, [FromForm] UpdatePostRequest request, CancellationToken cancellationToken)
        {
            var post = await postService.UpdateAsync(id, User.GetAuthenticatedUserId(), request, cancellationToken);
            return Ok(ApiResponse<PostDto>.SuccessResponse(post));
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
        {
            await postService.DeleteAsync(id, User.GetAuthenticatedUserId(), cancellationToken);
            return Ok(ApiResponse<object>.SuccessResponse(new { message = "Post deleted successfully." }));
        }

        [HttpGet("{id:long}/share-link")]
        public async Task<IActionResult> GetShareLink(long id, CancellationToken cancellationToken)
        {
            var result = await postService.GetShareLinkAsync(id, User.GetAuthenticatedUserId(), cancellationToken);
            return Ok(ApiResponse<ShareLinkDto>.SuccessResponse(result));
        }

        [HttpGet("saved")]
        public async Task<IActionResult> GetSaved([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var result = await postService.GetSavedAsync(User.GetAuthenticatedUserId(), page, pageSize, cancellationToken);
            return Ok(ApiResponse<PagedResult<PostDto>>.SuccessResponse(result));
        }
    }


    [ApiController]
    [Route("api/v1/feed")]
    [Authorize]
    public sealed class FeedController(IPostService postService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetFeed([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var result = await postService.GetFeedAsync(User.GetAuthenticatedUserId(), page, pageSize, cancellationToken);
            return Ok(ApiResponse<PagedResult<PostDto>>.SuccessResponse(result));
        }

        [HttpGet("discovery")]
        public async Task<IActionResult> GetDiscoveryFeed([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var result = await postService.GetDiscoveryFeedAsync(User.GetAuthenticatedUserId(), page, pageSize, cancellationToken);
            return Ok(ApiResponse<PagedResult<PostDto>>.SuccessResponse(result));
        }

    }



    [ApiController]
    [Route("api/v1/posts/{id:long}")]
    [Authorize]
    public sealed class PostInteractionsController(IPostInteractionService interactionService) : ControllerBase
    {
        [HttpPost("likes")]
        public async Task<IActionResult> LikePost(long id, CancellationToken cancellationToken)
        {
            var result = await interactionService.LikePostAsync(id, User.GetAuthenticatedUserId(), cancellationToken);
            return Ok(ApiResponse<LikeStateDto>.SuccessResponse(result));
        }

        [HttpDelete("likes")]
        public async Task<IActionResult> UnlikePost(long id, CancellationToken cancellationToken)
        {
            var result = await interactionService.UnlikePostAsync(id, User.GetAuthenticatedUserId(), cancellationToken);
            return Ok(ApiResponse<LikeStateDto>.SuccessResponse(result));
        }

        [HttpGet("comments")]
        public async Task<IActionResult> GetComments(long id, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var result = await interactionService.GetCommentsAsync(id, User.GetAuthenticatedUserId(), page, pageSize, cancellationToken);
            return Ok(ApiResponse<PagedResult<CommentDto>>.SuccessResponse(result));
        }

        [HttpPost("comments")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreateComment(long id, [FromBody] CreateCommentRequest request, CancellationToken cancellationToken)
        {
            var result = await interactionService.CreateCommentAsync(id, User.GetAuthenticatedUserId(), request, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, ApiResponse<CommentDto>.SuccessResponse(result));
        }

        [HttpPost("save")]
        public async Task<IActionResult> SavePost(long id, CancellationToken cancellationToken)
        {
            var result = await interactionService.SavePostAsync(id, User.GetAuthenticatedUserId(), cancellationToken);
            return Ok(ApiResponse<SaveStateDto>.SuccessResponse(result));
        }

        [HttpDelete("save")]
        public async Task<IActionResult> UnsavePost(long id, CancellationToken cancellationToken)
        {
            var result = await interactionService.UnsavePostAsync(id, User.GetAuthenticatedUserId(), cancellationToken);
            return Ok(ApiResponse<SaveStateDto>.SuccessResponse(result));
        }
    }


    [ApiController]
    [Route("api/v1/comments")]
    [Authorize]
    public sealed class CommentsController(IPostInteractionService interactionService) : ControllerBase
    {
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
        {
            await interactionService.DeleteCommentAsync(id, User.GetAuthenticatedUserId(), cancellationToken);
            return Ok(ApiResponse<object>.SuccessResponse(new { message = "Comment deleted successfully." }));
        }
    }

    [ApiController]
    [Route("api/v1/search")]
    [Authorize]
    public sealed class SearchController(ISearchService searchService) : ControllerBase
    {
        [HttpGet("posts")]
        public async Task<IActionResult> SearchPosts([FromQuery] SearchPostsRequest request, CancellationToken cancellationToken)
        {
            var result = await searchService.SearchPostsAsync(request.Q, request.Hashtag, User.GetAuthenticatedUserId(), request.Page, request.PageSize, cancellationToken);
            return Ok(ApiResponse<PagedResult<PostDto>>.SuccessResponse(result));
        }

        [HttpGet("hashtags")]
        public async Task<IActionResult> SearchHashtags([FromQuery] SearchHashtagsRequest request, CancellationToken cancellationToken)
        {
            var result = await searchService.SearchHashtagsAsync(request.Q, request.Limit, cancellationToken);
            return Ok(ApiResponse<List<HashtagDto>>.SuccessResponse(result));
        }

        [HttpGet("users")]
        public async Task<IActionResult> SearchUsers([FromQuery] SearchUsersRequest request, CancellationToken cancellationToken)
        {
            var result = await searchService.SearchUsersAsync(request.Q, User.GetAuthenticatedUserId(), request.Page, request.PageSize, cancellationToken);
            return Ok(ApiResponse<PagedResult<UserSummaryDto>>.SuccessResponse(result));
        }
    }


    [ApiController]
    [Route("api/v1/hashtags")]
    [Authorize]
    public sealed class HashtagsController(ISearchService searchService) : ControllerBase
    {
        [HttpGet("trending")]
        public async Task<IActionResult> GetTrending([FromQuery] int limit = 10, CancellationToken cancellationToken = default)
        {
            var result = await searchService.GetTrendingHashtagsAsync(limit, cancellationToken);
            return Ok(ApiResponse<List<TrendingHashtagDto>>.SuccessResponse(result));
        }
    }

    [ApiController]
    [Route("api/v1/categories")]
    [Authorize]
    public sealed class CategoriesController(ISearchService searchService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await searchService.GetCategoriesAsync(cancellationToken);
            return Ok(ApiResponse<List<CategoryDto>>.SuccessResponse(result));
        }
    }
}
