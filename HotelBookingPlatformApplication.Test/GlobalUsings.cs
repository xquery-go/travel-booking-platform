global using Xunit;

global using AutoMapper;
global using HotelBookingPlatform.Application.Core.Implementations;
global using HotelBookingPlatform.Domain.Entities;
global using HotelBookingPlatform.Domain;
global using Moq;
global using HotelBookingPlatform.Domain.DTOs.Owner;
global using HotelBookingPlatform.Domain.DTOs.City;
global using HotelBookingPlatform.Domain.Exceptions;
global using System.Linq.Expressions;
global using KeyNotFoundException = HotelBookingPlatform.Domain.Exceptions.KeyNotFoundException;
global using HotelBookingPlatform.Domain.DTOs.Hotel;
global using AutoFixture;
global using HotelBookingPlatform.Application.Services;
global using HotelBookingPlatform.Domain.Helpers;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.Extensions.Options;
global using System.IdentityModel.Tokens.Jwt;
global using System.Security.Claims;
global using Microsoft.AspNetCore.Http;
global using FluentAssertions;
global using HotelBookingPlatform.Application.Validator;
global using HotelBookingPlatform.Domain.IServices;
global using UnauthorizedAccessException = HotelBookingPlatform.Domain.Exceptions.UnauthorizedAccessException;

