using System.ComponentModel.DataAnnotations;

namespace Services.Helpers;

public class ValidationHelper
{
    internal static void ModelValidation(Object obj)
    {
        ValidationContext validationContext = new(obj);

        List<ValidationResult> validationResults = [];

        bool isValid = Validator.TryValidateObject(obj, validationContext, validationResults, true);

        if (!isValid)
            throw new ArgumentException(validationResults.FirstOrDefault()?.ErrorMessage);

    }
}
