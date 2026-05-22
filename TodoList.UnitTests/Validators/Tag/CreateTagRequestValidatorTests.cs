using TodoList.Application.Validators.Tag;

namespace TodoList.UnitTests.Validators.Tag;

public class CreateTagRequestValidatorTests
{
    private readonly CreateTagRequestValidator _sut = new();

    [Fact]
    public void ValidName_PassesValidation()
    {
        var request = new CreateTagRequest { Name = "Important" };
        var result = _sut.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void EmptyName_FailsValidation()
    {
        var request = new CreateTagRequest { Name = "" };
        var result = _sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void NameExceedsMaxLength_FailsValidation()
    {
        var request = new CreateTagRequest { Name = new string('a', 51) };
        var result = _sut.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void NameAtMaxLength_PassesValidation()
    {
        var request = new CreateTagRequest { Name = new string('a', 50) };
        var result = _sut.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
