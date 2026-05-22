using TodoList.API.Validation.Auth;

namespace TodoList.UnitTests.Validators.Auth;

public class RegisterRequestValidatorTests
{
    private readonly RegisterRequestValidator _sut = new();

    private static RegisterRequest ValidRequest => new(
        "user@example.com",
        "SecurePass1!",
        "SecurePass1!");

    [Fact]
    public void ValidRegister_PassesValidation()
    {
        var result = _sut.TestValidate(ValidRequest);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void EmptyEmail_FailsValidation()
    {
        var request = ValidRequest with { Email = "" };
        var result = _sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void InvalidEmailFormat_FailsValidation()
    {
        var request = ValidRequest with { Email = "invalid" };
        var result = _sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void EmailExceedsMaxLength_FailsValidation()
    {
        var longEmail = new string('a', 142) + "@test.com";
        var request = ValidRequest with { Email = longEmail };
        var result = _sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void EmptyPassword_FailsValidation()
    {
        var request = ValidRequest with { Password = "" };
        var result = _sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void PasswordTooShort_FailsValidation()
    {
        var request = ValidRequest with { Password = "Ab1!" };
        var result = _sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void PasswordTooLong_FailsValidation()
    {
        var request = ValidRequest with { Password = new string('A', 60) + "1!" };
        var result = _sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void PasswordMissingUppercase_FailsValidation()
    {
        var request = ValidRequest with { Password = "lowercase1!" };
        var result = _sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void PasswordMissingLowercase_FailsValidation()
    {
        var request = ValidRequest with { Password = "UPPERCASE1!" };
        var result = _sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void PasswordMissingDigit_FailsValidation()
    {
        var request = ValidRequest with { Password = "SecurePass!" };
        var result = _sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void PasswordMissingSpecialChar_FailsValidation()
    {
        var request = ValidRequest with { Password = "SecurePass1" };
        var result = _sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void EmptyConfirmPassword_FailsValidation()
    {
        var request = ValidRequest with { ConfirmPassword = "" };
        var result = _sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.ConfirmPassword);
    }

    [Fact]
    public void PasswordMismatch_FailsValidation()
    {
        var request = ValidRequest with { ConfirmPassword = "DifferentPass1!" };
        var result = _sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.ConfirmPassword);
    }

    [Fact]
    public void BoundaryLengthPassword_PassesValidation()
    {
        var request = ValidRequest with { Password = "Ab1!defgh", ConfirmPassword = "Ab1!defgh" };
        var result = _sut.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void MaxLengthEmail_PassesValidation()
    {
        var localPart = new string('a', 138);
        var request = ValidRequest with { Email = $"{localPart}@te" };
        var result = _sut.TestValidate(request);
        Assert.True(request.Email.Length <= 150);
    }
}
