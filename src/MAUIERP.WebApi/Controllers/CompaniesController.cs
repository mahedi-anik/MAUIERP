using MAUIERP.ApplicationLayer.Common.Models;
using MAUIERP.ApplicationLayer.DTOs.MasterData;
using MAUIERP.ApplicationLayer.Features.Companies.Commands;
using MAUIERP.ApplicationLayer.Features.Companies.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MAUIERP.WebApi.Controllers
{
    [ApiController]
    [Route("api/companies")]
    [Authorize]
    public class CompaniesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CompaniesController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<ActionResult<Result<PaginatedList<CompanyDto>>>> GetAll([FromQuery] GetCompaniesQuery query)
            => Ok(await _mediator.Send(query));

        [HttpGet("{id}")]
        public async Task<ActionResult<Result<CompanyDto>>> GetById(Guid id)
            => Ok(await _mediator.Send(new GetCompanyByIdQuery(id)));

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Result<CompanyDto>>> Create(CreateCompanyCommand command)
        {
            var result = await _mediator.Send(command);
            return result.Succeeded ? CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Result<CompanyDto>>> Update(Guid id, UpdateCompanyCommand command)
            => Ok(await _mediator.Send(command));

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Result>> Delete(Guid id)
            => Ok(await _mediator.Send(new DeleteCompanyCommand(id)));
    }
}
