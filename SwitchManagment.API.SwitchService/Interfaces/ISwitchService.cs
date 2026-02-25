using SwitchManagment.API.SwitchService.Data;

namespace SwitchManagment.API.SwitchService.Interfaces
{
    public interface ISwitchService
    {
        public Task<SwitchInfo> GetSwitchInfo(ConnectConfig connectConfig, CancellationToken cancellationToken = default);

        public Task ConfigurePort(PortConfig portConfig, CancellationToken cancellationToken = default);
    }
}
