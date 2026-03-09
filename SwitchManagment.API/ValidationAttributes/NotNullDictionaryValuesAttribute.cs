using System.ComponentModel.DataAnnotations;

namespace SwitchManagment.API.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class NotNullDictionaryValuesAttribute : ValidationAttribute
    {
        public NotNullDictionaryValuesAttribute()
        {
            ErrorMessage = $"Value in dictionary cant be null.";
        }

        public override bool IsValid(object value) =>
            value is Dictionary<string, string> dictionary ? dictionary.Values.All(value => value != null) : throw new ArgumentException("Argument 'value' is not Dictionary.", nameof(value));
    }
}
