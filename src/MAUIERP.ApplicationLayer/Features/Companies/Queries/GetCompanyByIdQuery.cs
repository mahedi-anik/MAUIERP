using AutoMapper;
using MAUIERP.ApplicationLayer.Common.Interfaces;
using MAUIERP.ApplicationLayer.Common.Models;
using MAUIERP.ApplicationLayer.DTOs.MasterData;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MAUIERP.ApplicationLayer.Features.Companies.Queries
{
    public record GetCompanyByIdQuery(Guid Id) : IRequest<Result<CompanyDto>>;

    public class GetCompanyByIdQueryHandler : IRequestHandler<GetCompanyByIdQuery, Result<CompanyDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetCompanyByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<CompanyDto>> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
        {
            var company = await _context.Companies
                .Include(c => c.Branches)
                .FirstOrDefaultAsync(c => c.Id == request.Id && !c.IsDeleted, cancellationToken);

            if (company == null)
                return Result<CompanyDto>.Failure($"Company with ID {request.Id} not found");

            var companyDto = _mapper.Map<CompanyDto>(company);
            return Result<CompanyDto>.Success(companyDto);
        }
    }
}