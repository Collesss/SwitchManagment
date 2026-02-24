using Renci.SshNet;
using Renci.SshNet.Common;
using SwitchManagment.API.SwitchService.Data;
using SwitchManagment.API.SwitchService.Exceptions;
using SwitchManagment.API.SwitchService.Interfaces;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace SwitchManagment.API.SwitchService.Implementations
{
    public class SwitchServiceHPComware5 : ISwitchService
    {
        private static readonly Regex SystemViewPromtShellRegex = new Regex(@"^\[[^\[\]]+\]", RegexOptions.Multiline);

        private async Task<SshClient> OpenConnect(string ipOrName, string login, string password, CancellationToken cancellationToken = default)
        {
            SshClient sshClient = new SshClient(ipOrName, 22, login, password);

            try
            {
                await sshClient.ConnectAsync(cancellationToken);
            }
            catch (SshAuthenticationException e) when (e.Message == "Permission denied (password).")
            {
                throw new SwitchServiceException(SwitchServiceErrorType.WrongLoginOrPass, e);
            }
            catch (SocketException e)
            {
                throw new SwitchServiceException(SwitchServiceErrorType.HostNotExistOrUnreac, e);
            }

            return sshClient;
        }
        
        private void EnterSuperPassSystemViewAndDisScreenLen(ShellStream shellStream, string superPassword)
        {
            shellStream.WriteLine("_cmdline-mode on");
            shellStream.WriteLine("Y");
            shellStream.WriteLine(superPassword);

            shellStream.Expect(new ExpectAction("Error: Invalid password.", _ => throw new SwitchServiceException(SwitchServiceErrorType.WrongSuperPass)),
                new ExpectAction("Warning: Now you enter an all-command mode for developer's testing, some commands may affect operation by wrong use, please carefully use it with our engineer's direction.", _ => { }));

            shellStream.WriteLine("screen-length disable");

            shellStream.WriteLine("system-view");
            shellStream.Expect("system-view");
            shellStream.Expect(SystemViewPromtShellRegex);
        }

        private IEnumerable<SwitchVlan> GetVlans(ShellStream shellStream)
        {
            Regex vlanRegex = new Regex(@"VLAN ID: (?<vlan_id>\d+)[?<=\d\D]+?Description: (?<description>[^\r\n]+)[?<=\d\D]+?Name: (?<name>[^\r\n]+)");

            shellStream.WriteLine("display vlan all");
            shellStream.Expect("display vlan all");

            /*
            StringBuilder stringBuilder = new StringBuilder();

            bool @continue = true;

            do
            {
                await Task.Delay(100);

                shellStream.Expect(new ExpectAction("---- More ----", str =>
                {
                    Console.WriteLine(str);
                    stringBuilder.Append(str);
                    shellStream.WriteLine(string.Empty);
                }), new ExpectAction(new Regex(@"^\[[^\[\]]+\]", RegexOptions.Multiline), str =>
                {
                    Console.WriteLine(str);
                    stringBuilder.Append(str);
                    @continue = false;
                }));

            } while (@continue);
            */

            string rawOutputVlanInfo = shellStream.Expect(SystemViewPromtShellRegex);

            return vlanRegex.Matches(rawOutputVlanInfo).Select(match => new SwitchVlan
            {
                Vlan = int.Parse(match.Groups["vlan_id"].Value),
                Name = match.Groups["name"].Value,
                Description = match.Groups["description"].Value
            }).ToArray();
        }

        private IEnumerable<SwitchPort> GetPorts(ShellStream shellStream)
        {
            Regex interfaceRegex = new Regex(@"(?<interface>[^ ]+) current state: (?<state>[^\r\n]+)[\d\D]+?Description: (?<description>[^\r\n]+)[\d\D]+?Port link-type: (?:(?<link_type>access)[\d\D]+?Untagged VLAN ID : (?<vlan>\d+)|(?<link_type>trunk)[\d\D]+?VLAN permitted:(?: (?:(?<vlan_range>(?<from>\d+)-(?<to>\d+))|(?<vlan>\d+))[^,\n]*[,\n]?)+|(?<link_type>.+))");

            shellStream.WriteLine("display interface");
            shellStream.Expect("display interface");

            string rawOutputInterfaceInfo = shellStream.Expect(SystemViewPromtShellRegex);

            return interfaceRegex.Matches(rawOutputInterfaceInfo)
                .Select(match => new SwitchPort
                {
                    Interface = match.Groups["interface"].Value,
                    Description = match.Groups["description"].Value,
                    Status = match.Groups["state"].Value switch
                    {
                        "DOWN" => SwitchPortStatus.Down,
                        "UP" => SwitchPortStatus.Up,
                        _ => SwitchPortStatus.Disable
                    },
                    Type = match.Groups["link_type"].Value switch
                    {
                        "access" => SwitchPortType.Access,
                        "trunk" => SwitchPortType.Trunk,
                        _ => SwitchPortType.Unknown
                    },
                    Vlans = match.Groups["vlan"].Captures.Select(matchVlan => int.Parse(matchVlan.Value)).ToArray()
                }).ToArray();
        }

        private async Task<(SshClient sshClient, ShellStream shellStream)> GetConnectionAndShell(string ipOrName, string login, string password, string superPassword, CancellationToken cancellationToken = default)
        {
            SshClient sshClient = await OpenConnect(ipOrName, login, password, cancellationToken);

            ShellStream shellStream = sshClient.CreateShellStreamNoTerminal();

            EnterSuperPassSystemViewAndDisScreenLen(shellStream, superPassword);

            return (sshClient, shellStream);
        }


        public async Task<SwitchInfo> GetSwitchInfo(string ipOrName, string login, string password, string superPassword, CancellationToken cancellationToken = default)
        {
            (SshClient sshClient, ShellStream shellStream) = await GetConnectionAndShell(ipOrName, login, password, superPassword, cancellationToken);

            try
            {
                return new SwitchInfo
                {
                    IpOrName = ipOrName,
                    Vlans = GetVlans(shellStream),
                    Ports = GetPorts(shellStream)
                };
            }
            finally
            {
                shellStream?.Dispose();
                sshClient?.Dispose();
            }
            /*
            using var sshClient = await OpenConnect(ipOrName, login, password, cancellationToken);

            using ShellStream shellStream = sshClient.CreateShellStreamNoTerminal();
            
            EnterSuperPassSystemViewAndDisScreenLen(shellStream, superPassword);
            */
        }



        public async Task ConfigurePort(string ipOrName, string login, string password, string superPassword, string interfaceName, bool isTrunk, CancellationToken cancellationToken = default, params int[] vlans)
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

            if (vlans.Any(vlan => vlans.Count(vl => vlan == vl) > 1))
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
                new ExpectAction(SystemViewPromtShellRegex, _ => { }));
            #endregion

            #region check_vlan_exist
            shellStream.WriteLine("display vlan");
            shellStream.Expect("The following VLANs exist:\r\n");

            string vlns = shellStream.Expect("\n");

            int[] vlansOnSwitch = Regex.Matches(vlns, @"\d+").Select(match => Int32.Parse(match.Value)).ToArray();

            if (!vlans.All(vl => vlansOnSwitch.Any(vlOnSw => vl == vlOnSw)))
                throw new SwitchServiceException("VLAN does not exist.");
            #endregion

            shellStream.Expect(SystemViewPromtShellRegex);

            if (isTrunk)
            {
                #region setting_for_trunk
                shellStream.WriteLine("port link-type trunk");

                shellStream.Expect(new ExpectAction("% Unrecognized command found at '^' position.", _ => throw new SwitchServiceException("Invalid interface name.")),
                    new ExpectAction(SystemViewPromtShellRegex, _ => { }));

                shellStream.WriteLine("undo port trunk permit vlan all");
                shellStream.WriteLine($"port trunk permit vlan {string.Join(' ', vlans)}");
                shellStream.Expect($"port trunk permit vlan {string.Join(' ', vlans)}");
                #endregion
            }
            else
            {
                #region setting_for_access
                shellStream.WriteLine("port link-type access");

                shellStream.Expect(new ExpectAction("% Unrecognized command found at '^' position.", _ => throw new SwitchServiceException("Invalid interface name.")),
                    new ExpectAction(SystemViewPromtShellRegex, _ => { }));

                shellStream.WriteLine($"port access vlan {vlans.Single()}");
                shellStream.Expect($"port access vlan {vlans.Single()}");
                #endregion
            }

            //wait execute last command
            shellStream.Expect(SystemViewPromtShellRegex);

            shellStream.Close();
            sshClient.Disconnect();
        }

        /*
        public async Task SettingPortAccess(string ipOrName, string login, string password, string superPassword, string interfaceName, int vlan, CancellationToken cancellationToken = default)
        {

        }
        public async Task SettingPortTrunk(string ipOrName, string login, string password, string superPassword, string interfaceName, CancellationToken cancellationToken = default, params int[] vlans)
        {

        }
        */
    }
}