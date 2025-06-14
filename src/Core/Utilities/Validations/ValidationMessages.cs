namespace Core.Utilities.Validations;
public static class ValidationMessages
{
    public const string NOT_EMPTY = "The {PropertyName} field cannot be empty.";
    public const string MAX_LENGTH = "The {PropertyName} field cannot exceed {MaxLength} characters.";
    public const string MIN_LENGTH = "The {PropertyName} field cannot be less than {MinLength} characters.";
    public const string GREATER_THAN_ZERO = "The {PropertyName} field must be greater than 0.";
    public const string INVALID_EMAIL = "The {PropertyName} field must be a valid email address.";
    public const string INVALID_URL_FORMAT = "The {PropertyName} must be a valid URL..";

    public static class Producto
    {
        public const string CANTIDAD_DE_STOCK_NOT_NEGATIVE = "The {PropertyName} can't be negative.";
        public const string CODIGO_BARRA_CANT_BE_DUPLICATED = "The {PropertyName} can't contain duplicated values.";
        public const string PRODUCTOS_NOT_FOUND = "No products found.";
    }
    public static class CodigoBarra
    {
        public const string CODIGO_BARRA_EXISTS = "The {PropertyName} already exists";
    }
}