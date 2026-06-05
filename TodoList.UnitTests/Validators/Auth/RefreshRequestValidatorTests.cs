using TodoList.API.Validation.Auth;

namespace TodoList.UnitTests.Validators.Auth;

public class RefreshRequestValidatorTests
{
    private readonly RefreshRequestValidator _sut = new();

    [Fact]
    public void ValidRefresh_PassesValidation()
    {
        var request = new RefreshRequest("access-token", "refresh-token");
        var result = _sut.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void EmptyAccessToken_FailsValidation()
    {
        var request = new RefreshRequest("", "refresh-token");
        var result = _sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.AccessToken);
    }

    [Fact]
    public void EmptyRefreshToken_FailsValidation()
    {
        var request = new RefreshRequest("access-token", "");
        var result = _sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.RefreshToken);
    }

    [Fact]
    public void BothTokensEmpty_FailsValidation()
    {
        var request = new RefreshRequest("", "");
        var result = _sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.AccessToken);
        result.ShouldHaveValidationErrorFor(x => x.RefreshToken);
    }
}
