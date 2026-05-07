using AutoMapper;
using MAUIERP.ApplicationLayer.Common.Interfaces;
using MAUIERP.ApplicationLayer.DTOs.MasterData;
using MAUIERP.ApplicationLayer.Features.Companies.Commands;
using MAUIERP.ApplicationLayer.Mappings;
using MAUIERP.Domain.Entities.MasterData;
using MAUIERP.UnitTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace MAUIERP.UnitTests.Commands;

public class CreateCompanyCommandTests
{
    private readonly IMapper _mapper;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly List<Company> _companies;

    public CreateCompanyCommandTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _currentUserServiceMock.Setup(x => x.UserName).Returns("testuser");

        _companies = new List<Company>();

        // Setup mock DbSet
        var companiesQueryable = _companies.AsQueryable();
        var mockDbSet = new Mock<DbSet<Company>>();

        mockDbSet.As<IQueryable<Company>>().Setup(m => m.Provider).Returns(companiesQueryable.Provider);
        mockDbSet.As<IQueryable<Company>>().Setup(m => m.Expression).Returns(companiesQueryable.Expression);
        mockDbSet.As<IQueryable<Company>>().Setup(m => m.ElementType).Returns(companiesQueryable.ElementType);
        mockDbSet.As<IQueryable<Company>>().Setup(m => m.GetEnumerator()).Returns(companiesQueryable.GetEnumerator());

        // Setup async add
        mockDbSet.Setup(x => x.AddAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()))
            .Callback<Company, CancellationToken>((entity, token) => _companies.Add(entity))
            .ReturnsAsync((Company entity, CancellationToken token) => null!);

        _contextMock = new Mock<IApplicationDbContext>();
        _contextMock.Setup(x => x.Companies).Returns(mockDbSet.Object);
        _contextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldCreateCompany()
    {
        // Arrange
        var createDto = TestDataHelper.GetValidCreateCompanyDto();
        var command = new CreateCompanyCommand(createDto);
        var handler = new CreateCompanyCommandHandler(_contextMock.Object, _mapper, _currentUserServiceMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
        Assert.NotNull(result.Data);
        Assert.Equal(createDto.Name, result.Data.Name);
        Assert.Equal(createDto.Code, result.Data.Code);
        Assert.Equal(createDto.Email, result.Data.Email);

        // Verify company was added
        Assert.Single(_companies);
        var savedCompany = _companies.First();
        Assert.Equal(createDto.Name, savedCompany.Name);
        Assert.Equal(createDto.Code, savedCompany.Code);
        Assert.Equal("testuser", savedCompany.CreatedBy);
    }

    [Fact(Skip = "Validation is handled by separate validator tests - CompanyValidatorTests")]
    public async Task Handle_DuplicateCode_ShouldReturnError()
    {
        // This test is skipped because validation is tested in CompanyValidatorTests
        await Task.CompletedTask;
    }

    [Fact(Skip = "Validation is handled by separate validator tests - CompanyValidatorTests")]
    public async Task Handle_EmptyName_ShouldReturnError()
    {
        // This test is skipped because validation is tested in CompanyValidatorTests
        await Task.CompletedTask;
    }

    [Fact(Skip = "Validation is handled by separate validator tests - CompanyValidatorTests")]
    public async Task Handle_EmptyCode_ShouldReturnError()
    {
        // This test is skipped because validation is tested in CompanyValidatorTests
        await Task.CompletedTask;
    }

    [Fact(Skip = "Validation is handled by separate validator tests - CompanyValidatorTests")]
    public async Task Handle_InvalidEmail_ShouldReturnError()
    {
        // This test is skipped because validation is tested in CompanyValidatorTests
        await Task.CompletedTask;
    }
}