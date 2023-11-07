using Domain.Customers;
using Domain.Primitives;
using Domain.ValueObjects;
using Domain.DomainErrors;

namespace Application.Customers.Create;
//esta clase se encarga de manejar los comandos para crear un cliente
public sealed class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, ErrorOr<Guid>>
{
    private readonly ICustomerRepository _customerRepository; //repocitorio de clientes
    private readonly IUnitOfWork _unitOfWork;// unidad de trabajo para tranaccion
    
    // Constructor de la clase, que recibe el repositorio de clientes y la unidad de trabajo
    public CreateCustomerCommandHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
    {
        // Se verifica que los parámetros no sean nulos y, de serlo, se lanza una excepción
        _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }
     // Implementación del método Handle de IRequestHandler para procesar el comando de creación del cliente
    public async Task<ErrorOr<Guid>> Handle(CreateCustomerCommand command, CancellationToken cancellationToken)
    {

        // Validación de los datos de entrada.
        // Se crean objetos PhoneNumber y Address a partir de los datos proporcionados en el comando
        if (PhoneNumber.Create(command.PhoneNumber) is not PhoneNumber phoneNumber)
        {
            return Errors.Customer.PhoneNumberWithBadFormat;
        }

        if (Address.Create(command.Country, command.Line1, command.Line2, command.City,
                    command.State, command.ZipCode) is not Address address)
        {
          return Errors.Customer.AddressWithBadFormat;
        }

        // Creación de una instancia de Customer a partir de los datos proporcionados en el comando.
        var customer = new Customer(
            new CustomerId(Guid.NewGuid()),
            command.Name,
            command.LastName,
            command.Email,
            phoneNumber,
            address,
            true
        );

        //se agrega el cliente al repositorio
        _customerRepository.Add(customer);
        
        // Se guarda la transacción en la unidad de trabajo
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        // Se devuelve el identificador del cliente creado
        return customer.Id.Value;
    }
}
