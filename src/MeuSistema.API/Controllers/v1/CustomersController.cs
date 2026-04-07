using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using MeuSistema.API.Extensions;
using MeuSistema.API.Models;
using MeuSistema.Application.Customer.Commands;
using MeuSistema.Application.Customer.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MeuSistema.API.Controllers.v1;


[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
public class CustomersController(IMediator mediator): ControllerBase
{
    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ApiResponse<CreatedCustomerResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody][Required] CreateCustomerCommand command) =>
        (await mediator.Send(command)).ToActionResult();
}
