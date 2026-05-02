using AutoMapper;
using MAUIERP.ApplicationLayer.Common.Interfaces;
using MAUIERP.ApplicationLayer.Common.Models;
using MAUIERP.ApplicationLayer.DTOs.MasterData;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MAUIERP.ApplicationLayer.Features.Companies.Commands
{
    public record UpdateCompanyCommand(Guid Id, UpdateCompanyDto Company) : IRequest<Result<CompanyDto>>;

    public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, Result<CompanyDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;

        public UpdateCompanyCommandHandler(IApplicationDbContext context, IMapper mapper, ICurrentUserService currentUser)
        {
            _context = context;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<Result<CompanyDto>> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
        {
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.Id == request.Id && !c.IsDeleted, cancellationToken);

            if (company == null)
                return Result<CompanyDto>.Failure("Company not found");

            _mapper.Map(request.Company, company);
            company.UpdatedAt = DateTime.UtcNow;
            company.UpdatedBy = _currentUser.UserName;

            await _context.SaveChangesAsync(cancellationToken);

            return Result<CompanyDto>.Success(_mapper.Map<CompanyDto>(company));
        }
    }

}
