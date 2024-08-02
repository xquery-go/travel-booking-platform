﻿using HotelBookingPlatform.Domain.DTOs.Room;

namespace HotelBookingPlatform.Application.Core.Abstracts;
public interface IRoomService
{
    Task<RoomResponseDto> GetRoomAsync(int id);
  //  Task<RoomResponseDto> CreateRoomAsync(RoomCreateRequest request);
    Task<RoomResponseDto> UpdateRoomAsync(int id, RoomCreateRequest request);
    Task DeleteRoomAsync(int id);
}
