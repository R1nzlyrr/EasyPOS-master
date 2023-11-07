namespace Customers.Common;

public record CustomerResponse(
Guid Id, //un identificador unico para el cliente
string FullName, //nombre completo del cliente
string Email, //correo electronico
string PhoneNumber,//numero de telefono
AddressResponse Address, //instancia que representa la direccion del cliente 
bool Active);

//registro que representa la direccion del cliente
public record AddressResponse(
    string Country, //pais de residencia
    string Line1, //linea de direccion 1
    string Line2, //linea de direccion 2 (opcional) 
    string City, //ciudad
    string State, //provincia
    string ZipCode); //codigo postal