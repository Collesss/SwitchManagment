using Renci.SshNet;
using SwitchManagment.API.SwitchService.Data;
using SwitchManagment.API.SwitchService.Interfaces;

namespace SwitchManagment.API.SwitchService.Implementations
{
    public class SwitchServiceHPComware5 : ISwitchService
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
                throw new ArgumentException("if port not trunck, vlan can be contains only one value.", nameof(vlans));

            if (!vlans.All(vlan => vlan > 0))
                throw new ArgumentException("vlan cant be less 1.", nameof(vlans));

            //if(vlans.All(vlan => vlans.Any(vl => vlan == vl)))


            using (var sshClient = new SshClient(ipOrName, 22, login, password))
            {
                sshClient.Connect();


                await sshClient.ConnectAsync(CancellationToken.None);

                ShellStream shellStream = sshClient.CreateShellStreamNoTerminal();

                shellStream.WriteLine("_cmdline-mode on");
                shellStream.WriteLine("Y");
                shellStream.WriteLine("512900");
                shellStream.WriteLine("system-view");
                shellStream.WriteLine($"interface {interfaceName}");

                if(isTrunk)
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

                /*
                while (shellStream.DataAvailable)
                {
                    if (shellStream.ReadLine() is string line)
                        Console.WriteLine(line);
                    else
                        break;
                }
                */

                shellStream.Close();
                sshClient.Disconnect();
            }
        }
    }
}
