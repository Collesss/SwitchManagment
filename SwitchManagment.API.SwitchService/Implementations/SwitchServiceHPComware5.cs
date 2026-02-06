using Renci.SshNet;
using SwitchManagment.API.SwitchService.Data;
using SwitchManagment.API.SwitchService.Exceptions;
using SwitchManagment.API.SwitchService.Interfaces;
using System.Text.RegularExpressions;

namespace SwitchManagment.API.SwitchService.Implementations
{
    public class SwitchServiceHPComware5 : ISwitchService
    {
        public Task<SwitchSummary> GetSwitchSummary(string ipOrName, string login, string password, string superPassword, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        
        public async Task SettingPort(string ipOrName, string login, string password, string superPassword, string interfaceName, bool isTrunk, CancellationToken cancellationToken = default, params int[] vlans)
        {
            #region validation
            ArgumentException.ThrowIfNullOrWhiteSpace(ipOrName);
            ArgumentException.ThrowIfNullOrWhiteSpace(login);
            ArgumentNullException.ThrowIfNull(password);
            ArgumentNullException.ThrowIfNull(superPassword);
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

            #region open_connect_and_shell
            using var sshClient = new SshClient(ipOrName, 22, login, password);
            
            await sshClient.ConnectAsync(cancellationToken);

            using ShellStream shellStream = sshClient.CreateShellStreamNoTerminal();
            #endregion

            #region enter_cmdline-mode
            shellStream.WriteLine("_cmdline-mode on");
            shellStream.WriteLine("Y");
            shellStream.WriteLine(superPassword);

            shellStream.Expect(new ExpectAction("Error: Invalid password.", _ => throw new SwitchServiceException("Invalid super password.")),
                new ExpectAction("Warning: Now you enter an all-command mode for developer's testing, some commands may affect operation by wrong use, please carefully use it with our engineer's direction.", _ => { }));

            #endregion

            #region enter_system_view_and_interface
            shellStream.WriteLine("system-view");
            shellStream.WriteLine($"interface {interfaceName}");

            shellStream.Expect($"interface {interfaceName}");
            shellStream.Expect(new ExpectAction("% Wrong parameter found at '^' position.", _ => throw new SwitchServiceException("Invalid interface name.")),
                new ExpectAction(new Regex(@"\[[^\[\]]+\]"), _ => { }));
            #endregion

            #region check_vlan_exist
            shellStream.WriteLine("display vlan");
            shellStream.Expect("The following VLANs exist:\r\n");

            string vlns = shellStream.Expect("\n");

            int[] vlansOnSwitch = Regex.Matches(vlns, @"\d+").Select(match => Int32.Parse(match.Value)).ToArray();

            if (!vlans.All(vl => vlansOnSwitch.Any(vlOnSw => vl == vlOnSw)))
                throw new SwitchServiceException("VLAN does not exist.");
            #endregion

            if (isTrunk)
            {
                #region setting_for_trunk
                shellStream.WriteLine("port link-type trunk");
                shellStream.WriteLine("undo port trunk permit vlan all");
                shellStream.WriteLine($"port trunk permit vlan {string.Join(' ', vlans)}");

                shellStream.Expect($"port trunk permit vlan {string.Join(' ', vlans)}");
                #endregion
            }
            else
            {
                #region setting_for_access
                shellStream.WriteLine("port link-type access");
                shellStream.WriteLine($"port access vlan {vlans.Single()}");

                shellStream.Expect($"port access vlan {vlans.Single()}");
                #endregion
            }

            //wait execute last command
            shellStream.Expect(new Regex(@"\[[^\[\]]+\]"));

            shellStream.Close();
            sshClient.Disconnect();
        }

    }
}
