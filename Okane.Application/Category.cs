namespace Okane.Application;

public class Category : IEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }//evita errores de null inesperados y garantiza que los objetos tengan datos obligatorios desde el inicio.
}