using System;
using System.ComponentModel.DataAnnotations;

namespace SwitchManagment.API.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class OnlyValuesAttribute: ValidationAttribute
    {
        private readonly object[] _values;

        public OnlyValuesAttribute(object[] values) 
        {
            _values = values ?? throw new ArgumentNullException(nameof(values));
            ErrorMessage = $"This field can only be equal to the following values: {string.Join(", ", _values)}";
        }

        public override bool IsValid(object value) =>
            _values.Any(val => val.Equals(value));
    }
}
