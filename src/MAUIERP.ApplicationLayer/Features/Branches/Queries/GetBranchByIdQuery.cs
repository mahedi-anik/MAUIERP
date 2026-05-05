using AutoMapper;
using MAUIERP.ApplicationLayer.Common.Interfaces;
using MAUIERP.ApplicationLayer.Common.Models;
using MAUIERP.ApplicationLayer.DTOs.MasterData;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MAUIERP.ApplicationLayer.Features.Branches.Queries
{
    public record GetBranchByIdQuery(Guid Id) : IRequest<Result<BranchDto>>;

    public class GetBranchByIdQueryHandler : IRequestHandler<GetBranchByIdQuery, Result<BranchDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetBranchByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<BranchDto>> Handle(GetBranchByIdQuery request, CancellationToken cancellationToken)
        {
            var branch = await _context.Branches
                .Include(b => b.Company)
                .FirstOrDefaultAsync(b => b.Id == request.Id && !b.IsDeleted, cancellationToken);

            if (branch == null)
            {
                return Result<BranchDto>.Failure("Branch not found");
            }

            var branchDto = _mapper.Map<BranchDto>(branch);
            return Result<BranchDto>.Success(branchDto);
        }
    }
}
