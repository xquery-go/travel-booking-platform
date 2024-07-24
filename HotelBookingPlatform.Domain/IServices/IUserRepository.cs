﻿using HotelBookingPlatform.Domain.DTOs.LocalUser;
using HotelBookingPlatform.Domain.DTOs.Login;
using HotelBookingPlatform.Domain.DTOs.Register;
namespace HotelBookingPlatform.Domain.IServices;
public interface IUserRepository
{
    Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
    Task<LocalUserDto> Register(RegisterDto registerRequestDto);
    Task<bool> IsUniqueUser(string email);
}
