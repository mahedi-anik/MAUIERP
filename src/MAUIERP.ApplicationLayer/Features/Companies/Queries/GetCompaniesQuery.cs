using AutoMapper;
using MAUIERP.ApplicationLayer.Common.Interfaces;
using MAUIERP.ApplicationLayer.Common.Models;
using MAUIERP.ApplicationLayer.DTOs.MasterData;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MAUIERP.ApplicationLayer.Features.Companies.Queries
{
    public record GetCompaniesQuery(int Page = 1, int PageSize = 10, string? SearchTerm = null)
    : IRequest<Result<PaginatedList<CompanyDto>>>;

    public class GetCompaniesQueryHandler : IRequestHandler<GetCompaniesQuery, Result<PaginatedList<CompanyDto>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetCompaniesQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<PaginatedList<CompanyDto>>> Handle(GetCompaniesQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Companies
                .Include(c => c.Branches)
                .Where(c => !c.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(c =>
                    c.Name.Contains(request.SearchTerm) ||
                    c.Code.Contains(request.SearchTerm) ||
                    c.TaxNumber.Contains(request.SearchTerm));
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var companyDtos = _mapper.Map<List<CompanyDto>>(items);

            return Result<PaginatedList<CompanyDto>>.Success(
                new PaginatedList<CompanyDto>(companyDtos, totalCount, request.Page, request.PageSize));
        }
    }
}
