﻿namespace HotelBookingPlatform.Application.Core.Implementations;
public class DiscountService : BaseService<Discount>, IDiscountService
{
    public DiscountService(IUnitOfWork<Discount> unitOfWork, IMapper mapper)
        : base(unitOfWork, mapper)
    {
    }

    public async Task<DiscountDto> AddDiscountToRoomAsync(int roomId, decimal percentage, DateTime startDateUtc, DateTime endDateUtc)
    {
        var room = await _unitOfWork.RoomRepository.GetByIdAsync(roomId);

        if (room is null)
        {
            throw new Exception("Room not found.");
        }

        var discount = new Discount
        {
            RoomID = roomId,
            Percentage = percentage,
            StartDateUtc = startDateUtc,
            EndDateUtc = endDateUtc,
        };

        await _unitOfWork.DiscountRepository.CreateAsync(discount);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<DiscountDto>(discount);
    }

    public async Task<List<DiscountDto>> GetAllDiscountsAsync()
    {
        var discounts = await _unitOfWork.DiscountRepository.GetAllAsync(query => query.Include(d => d.Room));
        return _mapper.Map<List<DiscountDto>>(discounts);
    }


    public async Task<DiscountDto> GetDiscountByIdAsync(int id)
    {
        var discount = await _unitOfWork.DiscountRepository.GetByIdAsync(id, query => query.Include(d => d.Room));
        if (discount == null)
        {
            throw new Exception("Discount not found.");
        }

        return _mapper.Map<DiscountDto>(discount);
    }

    public async Task DeleteDiscountAsync(int id)
    {
        var discount = await _unitOfWork.DiscountRepository.GetByIdAsync(id);
        if (discount == null)
        {
            throw new Exception("Discount not found.");
        }

        await _unitOfWork.DiscountRepository.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }


    public async Task<DiscountDto> UpdateDiscountAsync(int id, UpdateDiscountRequest request)
    {
        var discount = await _unitOfWork.DiscountRepository.GetByIdAsync(id);
        if (discount is null)
        {
            throw new Exception("Discount not found.");
        }
        if (request.Percentage.HasValue)
        {
            discount.Percentage = request.Percentage.Value;
        }
        if (request.StartDateUtc.HasValue)
        {
            discount.StartDateUtc = request.StartDateUtc.Value;
        }
        if (request.EndDateUtc.HasValue)
        {
            discount.EndDateUtc = request.EndDateUtc.Value;
        }

        _unitOfWork.DiscountRepository.UpdateAsync(id,discount);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<DiscountDto>(discount);
    }

}
