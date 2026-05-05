using AutoMapper;
using MAUIERP.ApplicationLayer.Common.Interfaces;
using MAUIERP.ApplicationLayer.Common.Models;
using MAUIERP.ApplicationLayer.DTOs.MasterData;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MAUIERP.ApplicationLayer.Features.Branches.Queries
{
    public record GetBranchesQuery(
        int Page = 1, 
        int PageSize = 10, 
        string? SearchTerm = null,
        Guid? CompanyId = null) 
        : IRequest<Result<PaginatedList<BranchDto>>>;

    public class GetBranchesQueryHandler : IRequestHandler<GetBranchesQuery, Result<PaginatedList<BranchDto>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetBranchesQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<PaginatedList<BranchDto>>> Handle(GetBranchesQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Branches
                .Include(b => b.Company)
                .Where(b => !b.IsDeleted)
                .AsQueryable();

            // Filter by company if specified
            if (request.CompanyId.HasValue && request.CompanyId != Guid.Empty)
            {
                query = query.Where(b => b.CompanyId == request.CompanyId);
            }

            // Search filter
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(b =>
                    b.Name.Contains(request.SearchTerm) ||
                    b.Code.Contains(request.SearchTerm) ||
                    b.Email.Contains(request.SearchTerm) ||
                    b.City.Contains(request.SearchTerm));
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var branchDtos = _mapper.Map<List<BranchDto>>(items);

            return Result<PaginatedList<BranchDto>>.Success(
                new PaginatedList<BranchDto>(branchDtos, totalCount, request.Page, request.PageSize));
        }
    }
}
