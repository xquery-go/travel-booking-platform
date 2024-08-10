﻿using HotelBookingPlatform.Domain.Entities;
namespace HotelBookingPlatform.Domain.Abstracts;
public interface IImageRepository
{
    Task<IEnumerable<Image>> GetImagesAsync(string entityType, int entityId);
    Task SaveImagesAsync(string entityType, int entityId, IEnumerable<string> imageUrls);
    Task DeleteImageAsync(int imageId);
    Task DeleteImagesAsync(string entityType, int entityId);
}
