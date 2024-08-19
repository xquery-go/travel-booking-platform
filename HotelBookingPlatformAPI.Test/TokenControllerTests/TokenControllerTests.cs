﻿using HotelBookingPlatform.Domain.Exceptions;
namespace HotelBookingPlatformAPI.Test.TokenControllerTests;
public class TokenControllerTests
{
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly Mock<IResponseHandler> _responseHandlerMock;
    private readonly Mock<ILog> _loggerMock;
    private readonly TokenController _controller;
    private readonly DefaultHttpContext _httpContext;

    public TokenControllerTests()
    {
        _tokenServiceMock = new Mock<ITokenService>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _responseHandlerMock = new Mock<IResponseHandler>();
        _loggerMock = new Mock<ILog>();
        _controller = new TokenController(
            _tokenServiceMock.Object,
            _httpContextAccessorMock.Object,
            _responseHandlerMock.Object,
            _loggerMock.Object);

        _httpContext = new DefaultHttpContext();
    }

    [Fact]
    public async Task RevokeToken_ShouldReturnBadRequest_WhenTokenCannotBeRevoked()
    {
        // Arrange
        var model = new RevokeToken { Token = "xxxxxxxxx" };
        _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(_httpContext);
        _tokenServiceMock.Setup(s => s.RevokeTokenAsync(model.Token)).ReturnsAsync(false);
        _responseHandlerMock.Setup(r => r.BadRequest("Token is invalid or could not be revoked!")).Returns(new BadRequestObjectResult("Token is invalid or could not be revoked!"));

        // Act
        Func<Task> act = async () => await _controller.RevokeToken(model);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Token is invalid or could not be revoked!");
    }
}