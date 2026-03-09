using System.ComponentModel.DataAnnotations;

namespace SwitchManagment.API.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class VlanListAttribute : ValidationAttribute
    {
        public VlanListAttribute()
        {
            ErrorMessage = $"A VLAN in the VLAN list can be unique and range from 1 to 4094 inclusive.";
        }

        public override bool IsValid(object value) =>
            value is IEnumerable<int> vlans ? 
            vlans.All(vlan => vlan > 0) && vlans.GroupBy(vlan => vlan).All(vlanGroup => vlanGroup.Count() == 1) : 
            throw new ArgumentException("Argument 'value' is not list int.", nameof(value));
    }
}
