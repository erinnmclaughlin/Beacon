using Beacon.Common.Requests.Auth;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beacon.API.IntegrationTests.Validation;

[Trait("Category", "[Feature] User Registration & Login")]
public sealed class RegisterRequestValidatorTests
{
    private readonly RegisterRequest.Validator _validator = new();

    [Fact]
    public void Validate_ShouldNotContainAnyErrors_WhenRequestIsValid()
    {
        var result = _validator.TestValidate(new RegisterRequest
        {
            EmailAddress = "person@test.com",
            DisplayName = "Person",
            Password = "Password123",
            ConfirmPassword = "Password123"
        });

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("nope")]
    public void Validate_ShouldContainError_WhenEmailIsInvalid(string email)
    {
        var result = _validator.TestValidate(new RegisterRequest { EmailAddress = email });
        result.ShouldHaveValidationErrorFor(r => r.EmailAddress);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldContainError_WhenDisplayNameIsInvalid(string displayName)
    {
        var result = _validator.TestValidate(new RegisterRequest { DisplayName = displayName });
        result.ShouldHaveValidationErrorFor(r => r.DisplayName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldContainError_WhenPasswordIsInvalid(string password)
    {
        var result = _validator.TestValidate(new RegisterRequest { Password = password });
        result.ShouldHaveValidationErrorFor(r => r.Password);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("Password1234")] // doesn't match
    public void Validate_ShouldContainError_WhenConfirmPasswordIsInvalid(string confirmPassword)
    {
        var result = _validator.TestValidate(new RegisterRequest
        {
            Password = "Password123",
            ConfirmPassword = confirmPassword
        });
        result.ShouldHaveValidationErrorFor(r => r.ConfirmPassword);
    }
}
