using TodoList.API.Validation.Category;

namespace TodoList.UnitTests.Validators.Category;

public class CreateCategoryRequestValidatorTests
{
    private readonly CreateCategoryRequestValidator _sut = new();

    [Fact]
    public void ValidName_PassesValidation()
    {
        var request = new CreateCategoryRequest("Work");
        var result = _sut.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void EmptyName_FailsValidation()
    {
        var request = new CreateCategoryRequest("");
        var result = _sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void NameExceedsMaxLength_FailsValidation()
    {
        var request = new CreateCategoryRequest(new string('a', 101));
        var result = _sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void NameAtMaxLength_PassesValidation()
    {
        var request = new CreateCategoryRequest(new string('a', 100));
        var result = _sut.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
