using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using MeuSistema.Application.Customer.Commands;
using MeuSistema.Application.Customer.Responses;
using MeuSistema.Domain.Entities.CustumerAggregate;
using MeuSistema.Domain.ValueObjects;
using MeuSistema.SharedKernel.Primitives;

public class CreateCustomerCommandHandler(
    IValidator<CreateCustomerCommand> validator,
    ICustomerRepository repository,
    IUnitOfWork unitOfWork
) : IRequestHandler<CreateCustomerCommand, Result<CreatedCustomerResponse>>
{
    public async Task<Result<CreatedCustomerResponse>> Handle(
        CreateCustomerCommand request,
        CancellationToken cancellationToken
    )
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return Result<CreatedCustomerResponse>.Invalid(validationResult.AsErrors());

        var email = Email.Create(request.Email);

        if (await repository.ExistsByEmailAsync(email))
            return Result<CreatedCustomerResponse>.Error("O endereço de e-mail informado já está em uso.");

        var customer = Customer.Create(
            request.FirstName,
            request.LastName,
            request.Gender,
            request.Email,
            request.BirthDate
        );


        repository.Add( customer );

        await unitOfWork.SaveChangesAsync();

        return Result<CreatedCustomerResponse>.Created(new CreatedCustomerResponse(customer.Id), location: $"/api/customers/{customer.Id}");


    }
}
