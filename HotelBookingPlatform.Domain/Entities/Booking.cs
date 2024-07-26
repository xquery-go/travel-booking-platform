﻿using HotelBookingPlatform.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;
namespace HotelBookingPlatform.Domain.Entities;
public class Booking
{
    public int BookingID { get; set; }
    public string LocalUserId { get; set; }
    public LocalUser LocalUser { get; set; }
    public int HotelId { get; set; }
    public Hotel? Hotel { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime CheckInDateUtc { get; set; }
    public DateTime CheckOutDateUtc { get; set; }
    public DateTime BookingDateUtc { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
}
