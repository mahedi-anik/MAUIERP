using FluentValidation.TestHelper;
using MAUIERP.ApplicationLayer.Common.Interfaces;
using MAUIERP.ApplicationLayer.DTOs.MasterData;
using MAUIERP.ApplicationLayer.Features.Companies.Commands;
using MAUIERP.Domain.Entities.MasterData;
using MAUIERP.UnitTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace MAUIERP.UnitTests.Validators;

public class CompanyValidatorTests
{
    private readonly CreateCompanyCommandValidator _createValidator;
    private readonly UpdateCompanyCommandValidator _updateValidator;
    private readonly List<Company> _companies;
    private readonly Mock<IApplicationDbContext> _contextMock;

    public CompanyValidatorTests()
    {
        _companies = new List<Company>();

        // Create a queryable mock DbSet
        var companiesQueryable = _companies.AsQueryable();
        var mockDbSet = new Mock<DbSet<Company>>();

        // Setup IQueryable
        mockDbSet.As<IQueryable<Company>>().Setup(m => m.Provider)
            .Returns(new TestAsyncQueryProvider<Company>(companiesQueryable.Provider));
        mockDbSet.As<IQueryable<Company>>().Setup(m => m.Expression).Returns(companiesQueryable.Expression);
        mockDbSet.As<IQueryable<Company>>().Setup(m => m.ElementType).Returns(companiesQueryable.ElementType);
        mockDbSet.As<IQueryable<Company>>().Setup(m => m.GetEnumerator()).Returns(companiesQueryable.GetEnumerator());

        // Setup IAsyncEnumerable
        mockDbSet.As<IAsyncEnumerable<Company>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncEnumerator<Company>(companiesQueryable.GetEnumerator()));

        _contextMock = new Mock<IApplicationDbContext>();
        _contextMock.Setup(c => c.Companies).Returns(mockDbSet.Object);

        _createValidator = new CreateCompanyCommandValidator(_contextMock.Object);
        _updateValidator = new UpdateCompanyCommandValidator(_contextMock.Object);
    }

    [Fact]
    public async Task Validator_EmptyName_ShouldHaveError()
    {
        // Arrange
        var command = new CreateCompanyCommand(new CreateCompanyDto { Name = "", Code = "TEST001" });

        // Act & Assert
        var result = await _createValidator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Company.Name);
    }

    [Fact]
    public async Task Validator_EmptyCode_ShouldHaveError()
    {
        // Arrange
        var command = new CreateCompanyCommand(new CreateCompanyDto { Name = "Test Company", Code = "" });

        // Act & Assert
        var result = await _createValidator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Company.Code);
    }

    [Fact]
    public async Task Validator_ValidModel_ShouldNotHaveErrors()
    {
        // Arrange
        var command = new CreateCompanyCommand(TestDataHelper.GetValidCreateCompanyDto());

        // Act & Assert
        var result = await _createValidator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validator_DuplicateName_ShouldHaveError()
    {
        // Arrange
        var existingCompany = TestDataHelper.GetValidCompany();
        _companies.Add(existingCompany);

        var createDto = TestDataHelper.GetValidCreateCompanyDto();
        createDto.Name = existingCompany.Name; // Duplicate name

        var command = new CreateCompanyCommand(createDto);

        // Act & Assert
        var result = await _createValidator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Company.Name);
    }

    [Fact]
    public async Task Validator_DuplicateCode_ShouldHaveError()
    {
        // Arrange
        var existingCompany = TestDataHelper.GetValidCompany();
        _companies.Add(existingCompany);

        var createDto = TestDataHelper.GetValidCreateCompanyDto();
        createDto.Code = existingCompany.Code; // Duplicate code

        var command = new CreateCompanyCommand(createDto);

        // Act & Assert
        var result = await _createValidator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Company.Code);
    }

    [Fact]
    public async Task Validator_InvalidEmail_ShouldHaveError()
    {
        // Arrange
        var createDto = TestDataHelper.GetValidCreateCompanyDto();
        createDto.Email = "invalid-email";

        var command = new CreateCompanyCommand(createDto);

        // Act & Assert
        var result = await _createValidator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Company.Email);
    }

    [Fact]
    public async Task Validator_NameTooLong_ShouldHaveError()
    {
        // Arrange
        var createDto = TestDataHelper.GetValidCreateCompanyDto();
        createDto.Name = new string('A', 201); // Assuming max length is 200

        var command = new CreateCompanyCommand(createDto);

        // Act & Assert
        var result = await _createValidator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Company.Name);
    }

    [Fact]
    public async Task Validator_CodeTooLong_ShouldHaveError()
    {
        // Arrange
        var createDto = TestDataHelper.GetValidCreateCompanyDto();
        createDto.Code = new string('A', 51); // Assuming max length is 50

        var command = new CreateCompanyCommand(createDto);

        // Act & Assert
        var result = await _createValidator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Company.Code);
    }

    [Fact]
    public async Task UpdateValidator_ValidModel_ShouldNotHaveErrors()
    {
        // Arrange
        var updateDto = new UpdateCompanyDto
        {
            Name = "Updated Company",
            Code = "UPD001",
            Phone = "0987654321",
            Email = "updated@company.com"
        };
        var command = new UpdateCompanyCommand(Guid.NewGuid(), updateDto);

        // Act & Assert
        var result = await _updateValidator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task UpdateValidator_NullName_ShouldHaveError()
    {
        // Arrange
        var updateDto = new UpdateCompanyDto
        {
            Name = null,
            Code = "UPD001"
        };
        var command = new UpdateCompanyCommand(Guid.NewGuid(), updateDto);

        // Act & Assert
        var result = await _updateValidator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Company.Name);
    }

    [Fact]
    public async Task UpdateValidator_EmptyName_ShouldHaveError()
    {
        // Arrange
        var updateDto = new UpdateCompanyDto
        {
            Name = "",
            Code = "UPD001"
        };
        var command = new UpdateCompanyCommand(Guid.NewGuid(), updateDto);

        // Act & Assert
        var result = await _updateValidator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Company.Name);
    }

    [Fact]
    public async Task UpdateValidator_EmptyCode_ShouldHaveError()
    {
        // Arrange
        var updateDto = new UpdateCompanyDto
        {
            Name = "Updated Company",
            Code = ""
        };
        var command = new UpdateCompanyCommand(Guid.NewGuid(), updateDto);

        // Act & Assert
        var result = await _updateValidator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Company.Code);
    }

    [Fact]
    public async Task UpdateValidator_DuplicateName_ShouldHaveError()
    {
        // Arrange
        var existingCompany = TestDataHelper.GetValidCompany();
        _companies.Add(existingCompany);

        var updateDto = new UpdateCompanyDto
        {
            Name = existingCompany.Name, // Duplicate name
            Code = "UPD001"
        };
        var command = new UpdateCompanyCommand(Guid.NewGuid(), updateDto);

        // Act & Assert
        var result = await _updateValidator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Company.Name);
    }
}