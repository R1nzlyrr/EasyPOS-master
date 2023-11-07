namespace Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    //Esta clase es un comportamiento genérico que se utiliza para validar las solicitudes antes de pasarlas al siguiente manejador

where TRequest : IRequest<TResponse>
    //representan el tipo de solicitud y el tipo de respuesta que se esperan en la aplicación
where TResponse : IErrorOr
    //se aseguran que las solicitudes y respuestas cumplan con ciertos requisitos de interfaz
{
    private readonly IValidator<TRequest>? _validator;
    //Esta instancia se utiliza para validar las solicitudes
    public ValidationBehavior(IValidator<TRequest>? validator = null)
    {
        _validator = validator;
    }

    public async Task<TResponse> Handle(
        // se utiliza en el patrón Mediator para interceptar y procesar solicitudes
        TRequest request,
        //El método Handle toma la solicitud request, un delegado next que representa el siguiente manejador en la cadena
        //y un token de cancelación cancellationToken
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        
        if (_validator is null) //este metodo comprueba si "validator" es nulo, 
            //si la validacion tiene exito se pasa la solicitud al proximo manejador
        {
            return await next();
        }

        var validatorResult = await _validator.ValidateAsync(request, cancellationToken);

        if (validatorResult.IsValid)
        {
            return await next();
        }

        var errors = validatorResult.Errors
                    .ConvertAll(validationFailure => Error.Validation(
                        validationFailure.PropertyName,
                        validationFailure.ErrorMessage
                    ));

        return (dynamic)errors;
    }
}
