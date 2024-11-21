using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BDwAS_projekt.Models;
using BDwAS_projekt.Models.Dto;
using BDwAS_projekt.Data;
using MongoDB.Bson;

namespace BDwAS_projekt.Controllers;

[ApiController]
[Route("Channel/{channelId}/Post")]
public class PostController : Controller
{
    private readonly IDbContext _db;

    public PostController(IDbContext db)
    {
        _db = db;
    }

    [HttpGet("{postId}")]
    public IActionResult GetPost(string channelId, string postId)
    {
        if (channelId.Length != 24 || postId.Length != 24) return BadRequest("Id must be 24 characters long.");
        Post post = _db.GetPost(postId, channelId);
        if (post is null) return NotFound("Post not found");

        return Ok(post);
    }

    [HttpPost("")]
    public IActionResult AddPost(string channelId, [FromBody] PostDto postDto)
    {
        if (postDto == null) return BadRequest("Post data is required.");
        if (channelId.Length != 24) return BadRequest("Id must be 24 characters long.");

        Post post = new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Title = postDto.Title,
            Content = postDto.Content,
            IsSponsored = postDto.IsSponsored,
            CreationDate = DateTime.Now,
            Comments = [],
            Ratings = [],
            Images = [],
            Attachments = []
        };

        if (_db.AddPost(post, channelId)) return Ok(post);
        else return BadRequest("Failed to add post.");
    }
    
    [HttpPut("{postId}")]
    public IActionResult UpdatePost(string channelId, string postId, [FromBody] PostDto postDto)
    {
        if (channelId.Length != 24 || postId.Length != 24) return BadRequest("Id must be 24 characters long.");

        Post post = new()
        {
            Id = postId,
            Title = postDto.Title,
            Content = postDto.Content,
            IsSponsored = postDto.IsSponsored,
        };

        if (_db.UpdatePost(post, channelId)) return Ok(post);
        else return BadRequest("Failed to update post.");
    }

    [HttpDelete("{postId}")]
    public IActionResult DeletePost(string channelId, string postId)
    {
        if (channelId.Length != 24 || postId.Length != 24) return BadRequest("Id must be 24 characters long.");

        if (_db.DeletePost(postId, channelId)) return Ok("Post deleted.");
        else return NotFound("Failed to delete post.");
    }

    [HttpPost("{postId}/Comment")]
    public IActionResult AddComment(string channelId, string postId, [FromBody] CommentDto commentDto)
    {
        if (commentDto == null) return BadRequest("Comment data is required.");
        if (channelId.Length != 24 || postId.Length != 24) return BadRequest("Id must be 24 characters long.");

        Comment comment = new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Content = commentDto.Content,
            AuthorId = commentDto.AuthorId,
            CreationDate = DateTime.Now
        };

        if (_db.AddComment(comment, channelId, postId)) return Ok(comment);
        else return BadRequest("Failed to add comment.");
    }

    [HttpDelete("{postId}/Comment/{commentId}")]
    public IActionResult DeletePlan(string channelId, string postId, string commentId)
    {
        if (channelId.Length != 24 || postId.Length != 24  || commentId.Length != 24) return BadRequest("Id must be 24 characters long.");

        if (_db.DeleteComment(commentId, channelId, postId)) return Ok("Comment deleted.");
        else return NotFound("Failed to delete comment.");
    }


    [HttpPost("{postId}/Rating")]
    public IActionResult AddRating(string channelId, string postId, [FromBody] RatingDto ratingDto)
    {
        if (ratingDto == null) return BadRequest("Rating data is required.");
        if (channelId.Length != 24 || postId.Length != 24) return BadRequest("Id must be 24 characters long.");

        Rating rating = new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Value = ratingDto.Value,
            AuthorId = ratingDto.AuthorId
        };

        if (_db.AddRating(rating, channelId, postId)) return Ok(rating);
        else return BadRequest("Failed to add rating.");
    }

    [HttpDelete("{postId}/Rating/{ratingId}")]
    public IActionResult DeleteRating(string channelId, string postId, string ratingId)
    {
        if (channelId.Length != 24 || postId.Length != 24  || ratingId.Length != 24) return BadRequest("Id must be 24 characters long.");

        if (_db.DeleteRating(ratingId, channelId, postId)) return Ok("Rating deleted.");
        else return NotFound("Failed to delete rating.");
    }

    [HttpPost("{postId}/Image")]
    public IActionResult AddImage(string channelId, string postId, [FromBody] ImageDto imageDto)
    {
        if (imageDto == null) return BadRequest("Image data is required.");
        if (channelId.Length != 24 || postId.Length != 24) return BadRequest("Id must be 24 characters long.");

        Image image = new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Path = imageDto.Path,
        };

        if (_db.AddImage(image, channelId, postId)) return Ok(image);
        else return BadRequest("Failed to add image.");
    }

    [HttpDelete("{postId}/Image/{imageId}")]
    public IActionResult DeleteImage(string channelId, string postId, string imageId)
    {
        if (channelId.Length != 24 || postId.Length != 24  || imageId.Length != 24) return BadRequest("Id must be 24 characters long.");

        if (_db.DeleteImage(imageId, channelId, postId)) return Ok("Image deleted.");
        else return NotFound("Failed to delete image.");
    }

    [HttpPost("{postId}/Attachment")]
    public IActionResult AddAttachment(string channelId, string postId, [FromBody] AttachmentDto attachmentDto)
    {
        if (attachmentDto == null) return BadRequest("Attachment data is required.");
        if (channelId.Length != 24 || postId.Length != 24) return BadRequest("Id must be 24 characters long.");
        
        Random random = new Random();

        Attachment attachment = new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Path = attachmentDto.Path,
            Name = attachmentDto.Name,
            Size = random.Next(100, 1000)
        };

        if (_db.AddAttachment(attachment, channelId, postId)) return Ok(attachment);
        else return BadRequest("Failed to add attachment.");
    }

    [HttpDelete("{postId}/Attachment/{attachmentId}")]
    public IActionResult DeleteAttachment(string channelId, string postId, string attachmentId)
    {
        if (channelId.Length != 24 || postId.Length != 24  || attachmentId.Length != 24) return BadRequest("Id must be 24 characters long.");

        if (_db.DeleteAttachment(attachmentId, channelId, postId)) return Ok("Attachment deleted.");
        else return NotFound("Failed to delete attachment.");
    }
}
