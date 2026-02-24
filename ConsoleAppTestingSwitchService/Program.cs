using SwitchManagment.API.SwitchService.Implementations;
using SwitchManagment.API.SwitchService.Interfaces;
using System.Threading.Tasks;

namespace ConsoleAppTestingSwitchService
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            ISwitchService switchService5 = new SwitchServiceHPComware5();
            //ISwitchService switchService7 = new SwitchServiceHPComware7();


            //await switchService5.SettingPort("10.23.34.23", "admin2", "1928", "512900", "GigabitEthernet1/0/3", true, CancellationToken.None, 264);

            var res = await switchService5.GetSwitchInfo("10.23.34.23", "admin2", "1928", "5129001", CancellationToken.None);

            Console.WriteLine(res.IpOrName);
            Console.WriteLine();

            foreach(var vlan in res.Vlans)
                Console.WriteLine($"{vlan.Vlan} {vlan.Name} {vlan.Description}");

            Console.WriteLine();

            foreach (var port in res.Ports)
                Console.WriteLine($"{port.Interface} {port.Status} {port.Type} {string.Join(", ", port.Vlans)}");
        }
    }
}
