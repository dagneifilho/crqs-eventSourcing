using Amazon.Runtime.Internal;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Common.DTOs;
using Post.Query.Api.DTOs;
using Post.Query.Api.Queries;
using Post.Query.Domain.Entities;

namespace Post.Query.Api.Controllers;

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
    public async Task<ActionResult> GetAllPostsAsync()
    {
        try
        {
            var posts = await _queryDispatcher.SendAsync(new FindAllPostsQuery());
            if(posts is null || !posts.Any())
                return NoContent();
            return NormalResponse(posts);
        }
        catch(Exception ex)
        {
            return ErrorResponse(ex);
        }
    }
    [HttpGet("byId/{postId}")]
    public async Task<ActionResult> GetByIdAsync([FromRoute]Guid postId)
    {
        try
        {
            var posts = await _queryDispatcher.SendAsync(new FindPostByIdQuery{Id = postId});
            if(posts is null || !posts.Any())
                return NoContent();
            return NormalResponse(posts);
        }   
        catch(Exception ex)
        {
            return ErrorResponse(ex);
        }
    }
    [HttpGet("byAuthor/{author}")]
    public async Task<ActionResult> GetPostsByAuthorAsync(string author)
    {
        try
        {
            var posts = await _queryDispatcher.SendAsync(new FindPostsByAuthorQuery() {Author = author});
            if(posts is null || !posts.Any())
                return NoContent();
            return NormalResponse(posts);
        }
        catch(Exception ex)
        {
            return ErrorResponse(ex);
        }
    }
    [HttpGet("withComments")]
    public async Task<ActionResult> GetPostsWithCommentAsync()
    {
        try
        {
            var posts = await _queryDispatcher.SendAsync(new FindPostsWithCommentsQuery());
            if(posts is null || !posts.Any())
                return NoContent();
            return NormalResponse(posts);
        }
        catch(Exception ex)
        {
            return ErrorResponse(ex);
        }
    }
    [HttpGet("withLikes/{numberOfLikes}")]
    public async Task<ActionResult> GetPostsWithLikesAsync(int numberOfLikes)
    {
        try
        {
            var posts = await _queryDispatcher.SendAsync(new FindPostsWithLikesQuery(){NumberOfLikes = numberOfLikes});
            if(posts is null || !posts.Any())
                return NoContent();
            return NormalResponse(posts);
        }
        catch(Exception ex)
        {
            return ErrorResponse(ex);
        }
    }

    private ActionResult NormalResponse(List<PostEntity> posts)
    {
        var count = posts.Count;
        return Ok(new PostLookupResponse 
            {
                Message =$"Successfully returned {count} post{(count>1?"s":string.Empty)}",
                Posts = posts
            });
    }
    private ActionResult ErrorResponse(Exception ex) 
    {
        const string SAFE_ERROR_MESSAGE = "Error while processing request to retrieve posts!";
        _logger.LogError(ex, SAFE_ERROR_MESSAGE);
        
        return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
        {
            Message = SAFE_ERROR_MESSAGE
        });
    }

}
