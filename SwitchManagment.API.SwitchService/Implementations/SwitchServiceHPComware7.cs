using Renci.SshNet;
using SwitchManagment.API.SwitchService.Data;
using SwitchManagment.API.SwitchService.Interfaces;

namespace SwitchManagment.API.SwitchService.Implementations
{
    public class SwitchServiceHPComware7 : ISwitchService
    {
        public SwitchSummary GetSwitchSummary(string ipOrName, string login, string password)
        {
            throw new NotImplementedException();
        }

        //test login: admin3 ; pass: 1234
        public async Task SettingPort(string ipOrName, string login, string password, string interfaceName, bool isTrunk, params int[] vlans)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(ipOrName);
            ArgumentException.ThrowIfNullOrWhiteSpace(login);
            ArgumentNullException.ThrowIfNull(password);
            ArgumentException.ThrowIfNullOrWhiteSpace(interfaceName);
            ArgumentNullException.ThrowIfNull(vlans);

            if (vlans.Length == 0)
                throw new ArgumentException("vlans cant be em", nameof(vlans));
            else if (!isTrunk && vlans.Length != 1)
                throw new ArgumentException("if port not trunck, vlan can contains only one value.", nameof(vlans));

            if (!vlans.All(vlan => vlan > 0))
                throw new ArgumentException("vlan cant be less 1.", nameof(vlans));


            using var sshClient = new SshClient(ipOrName, 22, login, password);
            sshClient.Connect();


            var command1 = sshClient.RunCommand("system-view");
            await Task.Delay(1_000);
            Console.WriteLine(command1.Result);

            var command2 = sshClient.RunCommand("interface GigabitEthernet1/0/20");
            await Task.Delay(1_000);
            Console.WriteLine(command2.Result);


            var command3 = sshClient.RunCommand("port link-type trunk");
            await Task.Delay(1_000);
            Console.WriteLine(command3.Result);


            var command4 = sshClient.RunCommand("port link-type trunk");
            await Task.Delay(1_000);
            Console.WriteLine(command4.Result);


            var command5 = sshClient.RunCommand("port trunk permit 10 20");
            await Task.Delay(1_000);
            Console.WriteLine(command4.Result);
        }
    }
}
