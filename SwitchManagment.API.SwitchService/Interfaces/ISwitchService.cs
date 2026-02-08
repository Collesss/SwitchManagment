using SwitchManagment.API.SwitchService.Data;

namespace SwitchManagment.API.SwitchService.Interfaces
{
    public interface ISwitchService
    {
        public Task<SwitchSummary> GetSwitchSummary(string ipOrName, string login, string password, string superPassword, CancellationToken cancellationToken = default);

        public Task<SwitchInfo> GetSwitchInfo(string ipOrName, string login, string password, string superPassword, CancellationToken cancellationToken = default);

        public Task SettingPort(string ipOrName, string login, string password, string superPassword, string interfaceName, bool isTrunk, CancellationToken cancellationToken = default, params int[] vlans);
    }
}
