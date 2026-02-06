using SwitchManagment.API.SwitchService.Implementations;
using SwitchManagment.API.SwitchService.Interfaces;
using System.Threading.Tasks;

namespace ConsoleAppTestingSwitchService
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            ISwitchService switchService = new SwitchServiceHPComware5();

            await switchService.SettingPort("10.23.34.23", "admin2", "1928", "512900", "GigabitEthernet1/0/3", true, CancellationToken.None, 264);

        }
    }
}
