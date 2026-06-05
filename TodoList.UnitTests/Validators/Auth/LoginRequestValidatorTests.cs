using TodoList.API.Validation.Auth;

namespace TodoList.UnitTests.Validators.Auth;

public class LoginRequestValidatorTests
{
    private readonly LoginRequestValidator _sut = new();

    [Fact]
    public void ValidLogin_PassesValidation()
    {
        var request = new LoginRequest("user@example.com", "Password123!");
        var result = _sut.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void EmptyEmail_FailsValidation()
    {
        var request = new LoginRequest("", "Password123!");
        var result = _sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void InvalidEmailFormat_FailsValidation()
    {
        var request = new LoginRequest("not-an-email", "Password123!");
        var result = _sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void EmptyPassword_FailsValidation()
    {
        var request = new LoginRequest("user@example.com", "");
        var result = _sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void WhitespaceEmail_FailsValidation()
    {
        var request = new LoginRequest("   ", "Password123!");
        var result = _sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void WhitespacePassword_FailsValidation()
    {
        var request = new LoginRequest("user@example.com", "   ");
        var result = _sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
}
