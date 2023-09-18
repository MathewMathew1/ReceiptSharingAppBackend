
using System.ComponentModel.DataAnnotations;

namespace ReceiptSharing.Api.Repositories.Utils;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class ArraySizeRangeAttribute : ValidationAttribute
{
    private readonly int _minSize;
    private readonly int _maxSize;

    public ArraySizeRangeAttribute(int minSize, int maxSize)
    {
        _minSize = minSize;
        _maxSize = maxSize;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is Array array && array.Length >= _minSize && array.Length <= _maxSize)
        {
            return ValidationResult.Success;
        }

        return new ValidationResult($"The field {validationContext.DisplayName} must have between {_minSize} and {_maxSize} elements.");
    }
}