using Renci.SshNet;
using SwitchManagment.API.SwitchService.Data;
using SwitchManagment.API.SwitchService.Interfaces;

namespace SwitchManagment.API.SwitchService.Implementations
{
    public class SwitchServiceHPComware5 : ISwitchService
    {
        public Task<SwitchSummary> GetSwitchSummary(string ipOrName, string login, string password, string superPassword, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        //test login: admin3 ; pass: 1234
        public async Task SettingPort(string ipOrName, string login, string password, string superPassword, string interfaceName, bool isTrunk, CancellationToken cancellationToken = default, params int[] vlans)
        {
            #region validation
            ArgumentException.ThrowIfNullOrWhiteSpace(ipOrName);
            ArgumentException.ThrowIfNullOrWhiteSpace(login);
            ArgumentNullException.ThrowIfNull(password);
            ArgumentException.ThrowIfNullOrWhiteSpace(interfaceName);
            ArgumentNullException.ThrowIfNull(vlans);

            if (vlans.Length == 0)
                throw new ArgumentException("VLAN list cant be empty.", nameof(vlans));
            else if (!isTrunk && vlans.Length != 1)
                throw new ArgumentException("If the port is not a trunk, the VLAN list must contain only one value.", nameof(vlans));

            if (!vlans.All(vlan => vlan > 0))
                throw new ArgumentException("VLAN cant be less 1.", nameof(vlans));

            if(vlans.Any(vlan => vlans.Count(vl => vlan == vl) > 1))
                throw new ArgumentException("VLAN list can be unique.", nameof(vlans));
            #endregion

            using var sshClient = new SshClient(ipOrName, 22, login, password);
            
            await sshClient.ConnectAsync(cancellationToken);

            using ShellStream shellStream = sshClient.CreateShellStreamNoTerminal();

            shellStream.WriteLine("_cmdline-mode on");
            shellStream.WriteLine("Y");
            shellStream.WriteLine(superPassword);
            shellStream.WriteLine("system-view");
            shellStream.WriteLine($"interface {interfaceName}");

            if (isTrunk)
            {
                shellStream.WriteLine("port link-type access");
                shellStream.WriteLine("port link-type trunk");
                shellStream.WriteLine($"port trunk permit vlan {string.Join(' ', vlans)}");

                Console.WriteLine(shellStream.Expect("Done."));
            }
            else
            {
                shellStream.WriteLine("port link-type access");
                shellStream.WriteLine($"port access permit vlan {vlans.Single()}");
            }

            Console.WriteLine(shellStream.Expect("Done."));

            shellStream.Close();
            sshClient.Disconnect();
        }
    }
}
