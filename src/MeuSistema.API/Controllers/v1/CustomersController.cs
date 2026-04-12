using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using MeuSistema.API.Extensions;
using MeuSistema.API.Models;
using MeuSistema.Application.Customer.Commands;
using MeuSistema.Application.Customer.Queries.GetCustomers;
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


    [HttpDelete("{id:guid}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete([Required] Guid id) =>
        (await mediator.Send(new DeleteCustomerCommand(id))).ToActionResult();

    [HttpGet("{id:guid}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ApiResponse<GetByIdCustomerResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById([Required] Guid id) =>
        (await mediator.Send(new GetByIdCustomerQuery(id))).ToActionResult();

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<GetCustomersResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll() =>
       (await mediator.Send(new GetCustomersQuery())).ToActionResult();

}
