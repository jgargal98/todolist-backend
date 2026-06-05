using TodoList.API.Validation.Category;

namespace TodoList.UnitTests.Validators.Category;

public class UpdateCategoryRequestValidatorTests
{
    private readonly UpdateCategoryRequestValidator _sut = new();

    [Fact]
    public void ValidName_PassesValidation()
    {
        var request = new UpdateCategoryRequest("Work");
        var result = _sut.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void EmptyName_FailsValidation()
    {
        var request = new UpdateCategoryRequest("");
        var result = _sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void NameExceedsMaxLength_FailsValidation()
    {
        var request = new UpdateCategoryRequest(new string('a', 51));
        var result = _sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void NameAtMaxLength_PassesValidation()
    {
        var request = new UpdateCategoryRequest(new string('a', 50));
        var result = _sut.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void SingleCharacterName_PassesValidation()
    {
        var request = new UpdateCategoryRequest("A");
        var result = _sut.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("  Name  ")]
    [InlineData("Name with spaces")]
    public void NameWithSpaces_PassesValidation(string name)
    {
        var request = new UpdateCategoryRequest(name);
        var result = _sut.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
