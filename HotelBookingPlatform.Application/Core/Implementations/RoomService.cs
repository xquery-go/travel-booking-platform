﻿namespace HotelBookingPlatform.Application.Core.Implementations;
public class RoomService : BaseService<Room>, IRoomService
{
    private readonly Domain.ILogger.ILogger _logger;
    public RoomService(IUnitOfWork<Room> unitOfWork, IMapper mapper,ILogger logger)
        : base(unitOfWork, mapper)
    {
        _logger = logger;
    }

    public async Task<RoomResponseDto> GetRoomAsync(int id)
    {
        var room = await _unitOfWork.RoomRepository.GetByIdAsync(id);

        if (room is null)
            throw new KeyNotFoundException("Room not found");

        var roomDto = _mapper.Map<RoomResponseDto>(room);
        return roomDto;
    }
    public async Task<RoomResponseDto> UpdateRoomAsync(int id, RoomCreateRequest request)
    {
       ValidationHelper.ValidateRequest(request);
        var existingRoom = await _unitOfWork.RoomRepository.GetByIdAsync(id);
        if (existingRoom is null)
            throw new KeyNotFoundException("Room not found");

        var room = _mapper.Map<Room>(request);
        await _unitOfWork.RoomRepository.UpdateAsync(id, room);

        return _mapper.Map<RoomResponseDto>(room);
    }
    public async Task DeleteRoomAsync(int id)
    {
        var room = await _unitOfWork.RoomRepository.GetByIdAsync(id);
        if (room is null)
            throw new KeyNotFoundException("Room not found");

        await _unitOfWork.RoomRepository.DeleteAsync(id);
    }
}

