

using System.ComponentModel.DataAnnotations;
using MediatR;
using Ardalis.Result;
using MeuSistema.Application.Customer.Responses;
using MeuSistema.Domain.Entities.CustumerAggregate;

namespace MeuSistema.Application.Customer.Commands
{
    public class CreateCustomerCommand : IRequest<Result<CreatedCustomerResponse>>
    {
        [Required]
        [MaxLength(100)]
        [DataType(DataType.Text)]
        public string FirstName { get; set; } = string.Empty;

        [Required] 
        [MaxLength(100)]
        [DataType(DataType.Text)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public EGender Gender { get; set; }

        [Required]
        [MaxLength(200)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;
         
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime BirthDate { get; set; }
    }
}
