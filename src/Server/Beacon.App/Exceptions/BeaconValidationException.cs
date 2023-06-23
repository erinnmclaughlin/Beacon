using FluentValidation;
using FluentValidation.Results;

namespace Beacon.App.Exceptions;

public class BeaconValidationException : ValidationException
{
    public BeaconValidationException(string propertyName, string errorMessage) :
        base(new[] { new ValidationFailure(propertyName, errorMessage) })
    {
    }
}
