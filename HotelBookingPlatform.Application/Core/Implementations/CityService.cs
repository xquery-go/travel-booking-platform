﻿using AutoMapper;
using HotelBookingPlatform.Application.Core.Abstracts;
using HotelBookingPlatform.Application.Services;
using HotelBookingPlatform.Domain;
using HotelBookingPlatform.Domain.Abstracts;
using HotelBookingPlatform.Domain.DTOs.City;
using HotelBookingPlatform.Domain.DTOs.Hotel;
using HotelBookingPlatform.Domain.Entities;
using HotelBookingPlatform.Domain.Exceptions;
using HotelBookingPlatform.Domain.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;
using InvalidOperationException = HotelBookingPlatform.Domain.Exceptions.InvalidOperationException;
using KeyNotFoundException = HotelBookingPlatform.Domain.Exceptions.KeyNotFoundException;

namespace HotelBookingPlatform.Application.Core.Implementations;
public class CityService : BaseService<City>, ICityService
{
    private readonly IFileRepository _fileRepository;
    public CityService(IUnitOfWork<City> unitOfWork, IMapper mapper, IWebHostEnvironment environment,IFileRepository fileRepository)
        : base(unitOfWork, mapper)
    {
       _fileRepository = fileRepository;
    }
    public async Task<IEnumerable<CityResponseDto>> GetCities(string cityName, string description, int pageSize, int pageNumber)
    {
        if (pageSize <= 0 || pageNumber <= 0)
            throw new ArgumentException("Page size and page number must be greater than zero.");

        Expression<Func<City, bool>> filter = null;

        if (!string.IsNullOrEmpty(cityName) || !string.IsNullOrEmpty(description))
        {
            filter = c => (!string.IsNullOrEmpty(cityName) && c.Name.Contains(cityName)) ||
                          (!string.IsNullOrEmpty(description) && c.Description.Contains(description));
        }
        var cities = await _unitOfWork.CityRepository.GetAllAsyncPagenation(filter, pageSize, pageNumber);
        foreach (var city in cities)
        {
            city.VisitCount += 1;
            await _unitOfWork.CityRepository.UpdateAsync(city.CityID, city);
        }

        await _unitOfWork.SaveChangesAsync();


        return _mapper.Map<IEnumerable<CityResponseDto>>(cities);
    }
    public async Task<CityWithHotelsResponseDto> GetCity(int id, bool includeHotels)
    {
        var city = await _unitOfWork.CityRepository.GetByIdAsync(id);
        if (city is null)
            throw new KeyNotFoundException("City not found.");

        city.VisitCount += 1;

        // تحديث المدينة في قاعدة البيانات
        await _unitOfWork.CityRepository.UpdateAsync(id, city);
        await _unitOfWork.SaveChangesAsync();

        if (includeHotels)
        {
            var cityWithHotelsDto = new CityWithHotelsResponseDto
            {
                CityID = city.CityID,
                Name = city.Name,
                Country = city.Country,
                PostOffice = city.PostOffice,
                CreatedAtUtc = city.CreatedAtUtc,
                Description = city.Description,
                Hotels = _mapper.Map<IEnumerable<HotelResponseDto>>(city.Hotels)
            };

            return cityWithHotelsDto;
        }

        return _mapper.Map<CityWithHotelsResponseDto>(city);

    }
    public async Task<CityResponseDto> UpdateCity(int id, CityCreateRequest request)
    {
        var existingCity = await _unitOfWork.CityRepository.GetByIdAsync(id);
        if (existingCity is null)
            throw new KeyNotFoundException("City not found.");

        var city = _mapper.Map(request, existingCity);
        await _unitOfWork.CityRepository.UpdateAsync(id, city);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<CityResponseDto>(city);
    }
    public async Task DeleteAsync(int id)
    {
        var city = await _unitOfWork.CityRepository.GetByIdAsync(id);
        if (city is null)
        {
            throw new KeyNotFoundException($"City with ID {id} not found.");
        }

        await _unitOfWork.CityRepository.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }
    public async Task<IEnumerable<HotelBasicResponseDto>> GetHotelsForCityAsync(int cityId)
    {
        try
        {
            var hotels = await _unitOfWork.HotelRepository.GetHotelsForCityAsync(cityId);

            if (hotels is null)
            {
                throw new InvalidOperationException("Hotels not found for the specified city.");
            }

            return _mapper.Map<IEnumerable<HotelBasicResponseDto>>(hotels);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("An error occurred while retrieving hotels.", ex);
        }
    }
    public async Task AddHotelToCityAsync(int cityId, HotelCreateRequest hotelRequest)
    {
        var city = await _unitOfWork.CityRepository.GetByIdAsync(cityId);
        if (city == null)
            throw new KeyNotFoundException("City not found.");

        var hotel = _mapper.Map<Hotel>(hotelRequest);
        hotel.CityID = cityId;

        await _unitOfWork.HotelRepository.CreateAsync(hotel);
        city.Hotels.Add(hotel);

        await _unitOfWork.SaveChangesAsync();
    }
    public async Task DeleteHotelFromCityAsync(int cityId, int hotelId)
    {
        var city = await _unitOfWork.CityRepository.GetByIdAsync(cityId);
        if (city == null)
            throw new KeyNotFoundException("City not found.");

        var hotel = await _unitOfWork.HotelRepository.GetByIdAsync(hotelId);
        if (hotel is null || hotel.CityID != cityId)
            throw new KeyNotFoundException("Hotel not found in the specified city.");

        await _unitOfWork.HotelRepository.DeleteAsync(hotelId);
        city.Hotels.Remove(hotel);

        await _unitOfWork.SaveChangesAsync();
    }
    public async Task<IEnumerable<CityResponseDto>> GetTopVisitedCitiesAsync(int topCount)
    {
        // Assume we have a method in repository to get cities sorted by visit count
        var cities = await _unitOfWork.CityRepository.GetTopVisitedCitiesAsync(topCount);

        if (cities == null || !cities.Any())
        {
            throw new NotFoundException("No cities found.");
        }

        return _mapper.Map<IEnumerable<CityResponseDto>>(cities);
    }


    //////////////////////////



    ////
    ///


    public async Task AddCityImagesAsync(int cityId, IList<IFormFile> imageFiles)
    {
        foreach (var file in imageFiles)
        {
            await _fileRepository.AddFileAsync("City", cityId, file);
        }
    }

    public async Task<IEnumerable<FileDetails>> GetCityImagesAsync(int cityId)
    {
        return await _fileRepository.GetFilesAsync("City", cityId);
    }

    public async Task DeleteCityImageAsync(int cityId, string fileName)
    {
        await _fileRepository.DeleteFileAsync("City", cityId, fileName);
    }


    public async Task DeleteCityImageAsync(int cityId, int imageId)
    {
        // تحقق من وجود المدينة
        var city = await _unitOfWork.CityRepository.GetByIdAsync(cityId);
        if (city == null)
            throw new Exception("City not found");

        // العثور على الصورة وحذفها
        var image = await _fileRepository.GetImageByIdAsync(imageId);
        if (image == null || image.EntityID != cityId || image.EntityType != "City")
            throw new Exception("Image not found or does not belong to this city");

        // حذف الملف من النظام
        if (File.Exists(image.FilePath))
        {
            File.Delete(image.FilePath);
        }

        // حذف بيانات الصورة من قاعدة البيانات
        await _fileRepository.DeleteAsync(imageId);
    }
}









