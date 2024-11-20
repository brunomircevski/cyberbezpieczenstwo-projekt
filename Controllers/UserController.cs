using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BDwAS_projekt.Models;
using BDwAS_projekt.Models.Dto;
using BDwAS_projekt.Data;
using MongoDB.Bson;

namespace BDwAS_projekt.Controllers;

[ApiController]
[Route("User")]
public class UserController : Controller
{
    private readonly IDbContext _db;

    public UserController(IDbContext db)
    {
        _db = db;
    }

    [HttpGet("")]
    public IActionResult GetAllUsers()
    {
        return Ok(_db.GetUsers());
    }

    [HttpGet("{userId}")]
    public IActionResult GetUser(string userId)
    {
        if (userId.Length != 24) return BadRequest("Id must be 24 characters long.");
        User user = _db.GetUser(userId);
        if (user is null) return NotFound("User not found");

        return Ok(user);
    }

    [HttpPost("")]
    public IActionResult AddUser([FromBody] UserDto userDto)
    {
        if (userDto == null) return BadRequest("Subscription data is required.");

        User user = new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            Email = userDto.Email,
            IsVerified = false,
            RegistrationDate = DateTime.Now,
            Subscriptions = [],
            ViewedPosts = []
        };

        if (_db.AddUser(user)) return Ok(user);
        else return BadRequest("Failed to add user.");
    }

    [HttpPut("{userId}")]
    public IActionResult EditUser(string userId, [FromBody] UserDto userDto)
    {
        if (userId.Length != 24) return BadRequest("Id must be 24 characters long.");

        User user = new()
        {
            Id = userId,
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            Email = userDto.Email
        };

        if (_db.UpdateUser(user)) return Ok(user);
        else return BadRequest("Failed to update user.");
    }

    [HttpPost("{userId}/Verify")]
    public IActionResult VerifyUser(string userId)
    {
        if (userId.Length != 24) return BadRequest("Id must be 24 characters long.");

        if (_db.VerifyUser(userId)) return Ok("User verified.");
        else return BadRequest("Failed to verify user.");
    }

    [HttpDelete("{userId}")]
    public IActionResult DeleteUser(string userId)
    {
        if (userId.Length != 24) return BadRequest("Id must be 24 characters long.");

        if (_db.DeleteUser(userId)) return Ok("User deleted.");
        else return BadRequest("Failed to delete user.");
    }

    [HttpPost("{userId}/Subscription")]
    public IActionResult AddSubscription(string userId, [FromBody] SubscriptionDto subscriptionDto)
    {
        if (subscriptionDto == null) return BadRequest("Subscription data is required.");
        if (userId.Length != 24) return BadRequest("Id must be 24 characters long.");

        Subscription subscription = new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            StartDate = subscriptionDto.StartDate,
            EndDate = subscriptionDto.EndDate,
            AutoRenew = subscriptionDto.AutoRenew,
            ChannelId = subscriptionDto.ChannelId,
            Payments = []
        };

        if (_db.AddSubscription(subscription, userId)) return Ok(subscription);
        else return BadRequest("Failed to add subscription.");
    }

    [HttpPost("{userId}/Subscription/{subscriptionId}/Payment")]
    public IActionResult AddPayment(string userId, string subscriptionId, [FromBody] PaymentDto paymentDto)
    {
        if (subscriptionId.Length != 24) return BadRequest("Id must be 24 characters long.");
        if (paymentDto == null) return BadRequest("Payment data is required.");

        if (paymentDto.Discount < 0) paymentDto.Discount = 0;
        if (paymentDto.Discount > 100) paymentDto.Discount = 100;

        Payment payment = new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            AddedDays = paymentDto.AddedDays,
            FullPrice = paymentDto.FullPrice,
            Discount = paymentDto.Discount,
            PaidPrice = paymentDto.FullPrice * (1.0 - paymentDto.Discount / 100.0),
            PaymentDate = DateTime.Now
        };

        if (_db.AddPayment(payment, subscriptionId, userId)) return Ok(payment);
        else return BadRequest("Failed to add payment.");
    }
}
