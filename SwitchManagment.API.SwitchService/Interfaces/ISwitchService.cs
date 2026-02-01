using SwitchManagment.API.SwitchService.Data;

namespace SwitchManagment.API.SwitchService.Interfaces
{
    public interface ISwitchService
    {
        public SwitchSummary GetSwitchSummary(string ipOrName, string login, string password);

        public Task SettingPort(string ipOrName, string login, string password, string interfaceName, bool isTrunk, params int[] vlans);
    }
}
