using System.ComponentModel.DataAnnotations;

namespace SwitchManagment.API.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class OnlyDictionaryKeyValuesAttribute : ValidationAttribute
    {
        private readonly object[] _values;

        public OnlyDictionaryKeyValuesAttribute(object[] values)
        {
            _values = values ?? throw new ArgumentNullException(nameof(values));
            //"This field can only be equal to the following values: {string.Join(", ", _values)}"
            ErrorMessage = $"Only the following values ​​can be used as a key: {string.Join(", ", _values)}";
        }


        public override bool IsValid(object value) =>
            value is Dictionary<string, string> dictionary ? dictionary.Keys.All(key => _values.Any(val => val.Equals(key))) : throw new ArgumentException("Argument 'value' is not Dictionary.", nameof(value));
    }
}
