using SwitchManagment.API.SwitchService.Implementations;
using SwitchManagment.API.SwitchService.Interfaces;
using System.Threading.Tasks;

namespace ConsoleAppTestingSwitchService
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //ISwitchService switchService5 = new SwitchServiceHPComware5();
            ISwitchService switchService7 = new SwitchServiceHPComware7();


            //await switchService5.SettingPort("10.23.34.23", "admin2", "1928", "512900", "GigabitEthernet1/0/3", true, CancellationToken.None, 264);

            var res = await switchService7.GetSwitchInfo("192.168.200.10", "admin2", "1928", null, CancellationToken.None);

        }
    }
}
