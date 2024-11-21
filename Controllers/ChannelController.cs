using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BDwAS_projekt.Models;
using BDwAS_projekt.Models.Dto;
using BDwAS_projekt.Data;
using MongoDB.Bson;

namespace BDwAS_projekt.Controllers;

[ApiController]
[Route("Channel")]
public class ChannelController : Controller
{
    private readonly IDbContext _db;

    public ChannelController(IDbContext db)
    {
        _db = db;
    }

    [HttpGet("")]
    public IActionResult GetAllChannels()
    {
        return Ok(_db.GetChannels());
    }

    [HttpGet("{channelId}")]
    public IActionResult GetChannel(string channelId)
    {
        if (channelId.Length != 24) return BadRequest("Id must be 24 characters long.");
        Channel channel = _db.GetChannel(channelId);
        if (channel is null) return NotFound("Channel not found");

        return Ok(channel);
    }

    [HttpPost("")]
    public IActionResult AddChannel([FromBody] ChannelDto channelDto)
    {
        if (channelDto == null) return BadRequest("Channel data is required.");
        if (channelDto.OwnerId?.Length != 24) return BadRequest("Id must be 24 characters long.");

        Channel channel = new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Name = channelDto.Name,
            Description = channelDto.Description,
            OwnerId = channelDto.OwnerId,
            CreationDate = DateTime.Now,
            Subscriptions = [],
            Categories = [],
            Posts = [],
            Streams = [],
            Plans = []
        };

        if (_db.AddChannel(channel)) return Ok(channel);
        else return BadRequest("Failed to add channel.");
    }

    [HttpPut("{channelId}")]
    public IActionResult UpdateChannel(string channelId, [FromBody] ChannelDto channelDto)
    {
        if (channelId.Length != 24) return BadRequest("Id must be 24 characters long.");

        Channel channel = new()
        {
            Id = channelId,
            Name = channelDto.Name,
            Description = channelDto.Description,
            OwnerId = channelDto.OwnerId,
        };

        if (_db.UpdateChannel(channel)) return Ok(channel);
        else return BadRequest("Failed to update channel.");
    }

    [HttpDelete("{channelId}")]
    public IActionResult DeleteChannel(string channelId)
    {
        if (channelId.Length != 24) return BadRequest("Id must be 24 characters long.");

        if (_db.DeleteChannel(channelId)) return Ok("Channel deleted.");
        else return NotFound("Failed to delete channel.");
    }

    [HttpPost("{channelId}/Category")]
    public IActionResult AddCategory(string channelId, [FromBody] CategoryDto categoryDto)
    {
        if (categoryDto == null) return BadRequest("Category data is required.");
        if (channelId.Length != 24) return BadRequest("Id must be 24 characters long.");

        Category category = new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Name = categoryDto.Name,
            MinimumAge = categoryDto.MinimumAge
        };

        if (_db.AddCategrory(category, channelId)) return Ok(category);
        else return BadRequest("Failed to add category.");
    }

    [HttpDelete("{channelId}/Category/{categoryName}")]
    public IActionResult DeleteCategory(string channelId, string categoryName)
    {
        if (categoryName == null) return BadRequest("Category name is required.");
        if (channelId.Length != 24) return BadRequest("Id must be 24 characters long.");

        if (_db.DeleteCategrory(categoryName, channelId)) return Ok("Category deleted.");
        else return NotFound("Failed to delete category.");
    }

    [HttpPost("{channelId}/LiveStream")]
    public IActionResult AddLiveStream(string channelId, [FromBody] LiveStreamDto liveStreamDto)
    {
        if (liveStreamDto == null) return BadRequest("LiveStream data is required.");
        if (channelId.Length != 24) return BadRequest("Id must be 24 characters long.");

        LiveStream liveStream = new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Name = liveStreamDto.Name,
            StartDate = DateTime.Now
        };

        if (_db.AddLiveStream(liveStream, channelId)) return Ok(liveStream);
        else return BadRequest("Failed to add liveStream.");
    }

    [HttpPut("{channelId}/LiveStream/{streamId}")]
    public IActionResult UpdateLiveStream(string channelId, string streamId, [FromBody] LiveStreamDto liveStreamDto)
    {
        if (liveStreamDto == null) return BadRequest("LiveStream data is required.");
        if (channelId.Length != 24 || streamId.Length != 24) return BadRequest("Id must be 24 characters long.");

        LiveStream liveStream = new()
        {
            Id = streamId,
            Name = liveStreamDto.Name,
            SavedPath = liveStreamDto.SavedPath,
            EndDate = liveStreamDto.EndDate
        };

        if (_db.UpdateLiveStream(liveStream, channelId)) return Ok(liveStream);
        else return BadRequest("Failed to update liveStream.");
    }

    [HttpPost("{channelId}/Plan")]
    public IActionResult AddPlan(string channelId, [FromBody] PlanDto planDto)
    {
        if (planDto == null) return BadRequest("Plan data is required.");
        if (channelId.Length != 24) return BadRequest("Id must be 24 characters long.");

        Plan plan = new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Name = planDto.Name,
            Days = planDto.Days,
            Price = planDto.Price,
            Discount = planDto.Discount
        };

        if (_db.AddPlan(plan, channelId)) return Ok(plan);
        else return BadRequest("Failed to add plan.");
    }

    [HttpDelete("{channelId}/Plan/{planId}")]
    public IActionResult DeletePlan(string channelId, string planId)
    {
        if (channelId.Length != 24 || planId.Length != 24) return BadRequest("Id must be 24 characters long.");

        if (_db.DeletePlan(planId, channelId)) return Ok("Plan deleted.");
        else return NotFound("Failed to delete plan.");
    }
}
