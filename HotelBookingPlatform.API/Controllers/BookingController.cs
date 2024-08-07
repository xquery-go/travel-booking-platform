﻿using HotelBookingPlatform.Domain.DTOs.Booking;
using Microsoft.AspNetCore.Mvc;
using HotelBookingPlatform.Application.Core.Abstracts;
using Swashbuckle.AspNetCore.Annotations;
using HotelBookingPlatform.Domain.Exceptions;
using HotelBookingPlatform.Domain.Enums;
using System.Security.Claims;
namespace HotelBookingPlatform.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[ResponseCache(CacheProfileName = "DefaultCache")]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Retrieve a booking by its unique identifier.")]
    public async Task<IActionResult> GetBooking(int id)
    {
        try
        {
            var response = await _bookingService.GetBookingAsync(id);
            return Ok(response);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> CreateBooking([FromBody] BookingCreateRequest request)
    {
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value; 
        if (userEmail is null)
        {
            return Unauthorized("User email not found in token.");
        }

        try
        {
            var booking = await _bookingService.CreateBookingAsync(request, userEmail);
            return Ok(booking);
        }
        catch (BadRequestException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }


    [HttpPut("{id}/status")]
    [SwaggerOperation(Summary = "Update the status of a booking.")]
    public async Task<IActionResult> UpdateBookingStatus(int id, [FromBody] BookingStatus newStatus)
    {
        try
        {
            await _bookingService.UpdateBookingStatusAsync(id, newStatus);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (BadRequestException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}


