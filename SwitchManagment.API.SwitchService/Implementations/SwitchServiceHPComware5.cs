using Renci.SshNet;
using Renci.SshNet.Common;
using SwitchManagment.API.SwitchService.Data;
using SwitchManagment.API.SwitchService.Exceptions;
using SwitchManagment.API.SwitchService.Extensions;
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


        private async Task<(SshClient sshClient, ShellStream shellStream)> GetConnectionAndShell(string ipOrName, string login, string password, string superPassword, CancellationToken cancellationToken = default)
        {
            SshClient sshClient = await OpenConnect(ipOrName, login, password, cancellationToken);

            ShellStream shellStream = sshClient.CreateShellStreamNoTerminal();

            EnterSuperPassSystemViewAndDisScreenLen(shellStream, superPassword);

            return (sshClient, shellStream);
        }

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

            shellStream.WriteLineAndExpect("system-view");
            shellStream.Expect(SystemViewPromtShellRegex);
        }

        private IEnumerable<int> GetOnlyVlanNums(ShellStream shellStream)
        {
            shellStream.WriteLine("display vlan");
            shellStream.Expect("The following VLANs exist:\r\n");

            string vlns = shellStream.Expect("\n");

            shellStream.Expect(SystemViewPromtShellRegex);

            return Regex.Matches(vlns, @"\d+").Select(match => Int32.Parse(match.Value)).ToArray();
        }

        private IEnumerable<SwitchVlan> GetVlans(ShellStream shellStream)
        {
            Regex vlanRegex = new Regex(@"VLAN ID: (?<vlan_id>\d+)[?<=\d\D]+?Description: (?<description>[^\r\n]+)[?<=\d\D]+?Name: (?<name>[^\r\n]+)");

            shellStream.WriteLine("display vlan all");
            shellStream.Expect("display vlan all");

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

            shellStream.WriteLineAndExpect("display interface");

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

        public async Task<SwitchInfo> GetSwitchInfo(ConnectConfig connectConfig, CancellationToken cancellationToken = default)
        {
            SshClient sshClient = null; 
            ShellStream shellStream = null;

            try
            {
                (sshClient, shellStream) = await GetConnectionAndShell(connectConfig.IpOrName, connectConfig.Login, connectConfig.Password, connectConfig.SuperPassword, cancellationToken);

                return new SwitchInfo
                {
                    IpOrName = connectConfig.IpOrName,
                    Vlans = GetVlans(shellStream),
                    Ports = GetPorts(shellStream)
                };
            }
            catch (SwitchServiceException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SwitchServiceException(SwitchServiceErrorType.Unknown, ex);
            }
            finally
            {
                shellStream?.Dispose();
                sshClient?.Dispose();
            }
        }

        private void EnterInterface(ShellStream shellStream, string interfaceName)
        {
            shellStream.WriteLineAndExpect($"interface {interfaceName}");

            shellStream.Expect(new ExpectAction("% Wrong parameter found at '^' position.", _ =>
                throw new SwitchServiceException(SwitchServiceErrorType.WrongInterface)),
                        new ExpectAction(SystemViewPromtShellRegex, _ => { }));
        }

        public async Task ConfigurePort(PortConfig portConfig, CancellationToken cancellationToken = default)
        {
            #region validation
            ArgumentNullException.ThrowIfNull(portConfig);
            ArgumentException.ThrowIfNullOrWhiteSpace(portConfig.IpOrName);
            ArgumentException.ThrowIfNullOrWhiteSpace(portConfig.Login);
            ArgumentNullException.ThrowIfNull(portConfig.Password);
            ArgumentNullException.ThrowIfNull(portConfig.SuperPassword);
            ArgumentException.ThrowIfNullOrWhiteSpace(portConfig.InterfaceName);
            ArgumentNullException.ThrowIfNull(portConfig.Vlans);

            if (portConfig.Vlans.Count() == 0)
                throw new ArgumentException("VLAN list cant be empty.");
            else if (!portConfig.IsTrunk && portConfig.Vlans.Count() != 1)
                throw new ArgumentException("If the port is not a trunk, the VLAN list must contain only one value.");

            if (!portConfig.Vlans.All(vlan => vlan > 0))
                throw new ArgumentException("VLAN cant be less 1.");

            if (portConfig.Vlans.Any(vlan => portConfig.Vlans.Count(vl => vlan == vl) > 1))
                throw new ArgumentException("VLAN list can be unique.");
            #endregion


            SshClient sshClient = null;
            ShellStream shellStream = null;

            void CheckInterfaceOrSupportLinkType()
            {
                shellStream.Expect(new ExpectAction(new Regex("% Unrecognized command found at '\\^' position\\.|% Wrong parameter found at '\\^' position\\."), _ => 
                throw new SwitchServiceException(SwitchServiceErrorType.WrongInterface)),
                        new ExpectAction(SystemViewPromtShellRegex, _ => { }));
            }

            try
            {
                (sshClient, shellStream) = await GetConnectionAndShell(portConfig.IpOrName, portConfig.Login, portConfig.Password, portConfig.SuperPassword, cancellationToken);

                EnterInterface(shellStream, portConfig.InterfaceName);

                #region check_vlan_exist
                IEnumerable<int> vlansOnSwitch = GetOnlyVlanNums(shellStream);

                if (!portConfig.Vlans.All(vl => vlansOnSwitch.Any(vlOnSw => vl == vlOnSw)))
                    throw new SwitchServiceException(SwitchServiceErrorType.VLANNotExist);
                #endregion

                shellStream.Expect(SystemViewPromtShellRegex);

                if (portConfig.IsTrunk)
                {
                    #region setting_for_trunk
                    shellStream.WriteLine("port link-type trunk");

                    CheckInterfaceOrSupportLinkType();

                    shellStream.WriteLine("undo port trunk permit vlan all");
                    shellStream.WriteLineAndExpect($"port trunk permit vlan {string.Join(' ', portConfig.Vlans)}");
                    #endregion
                }
                else
                {
                    #region setting_for_access
                    shellStream.WriteLine("port link-type access");

                    CheckInterfaceOrSupportLinkType();

                    shellStream.WriteLineAndExpect($"port access vlan {portConfig.Vlans.Single()}");
                    #endregion
                }

                //wait execute last command
                shellStream.Expect(SystemViewPromtShellRegex);

            }
            catch (SwitchServiceException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SwitchServiceException(SwitchServiceErrorType.Unknown, ex);
            }
            finally 
            {
                shellStream?.Dispose();
                sshClient?.Dispose();
            }
        }

        public async Task ConfigurePort(PortConfigTrunk portConfig, CancellationToken cancellationToken = default)
        {

            #region validation
            ArgumentNullException.ThrowIfNull(portConfig);
            ArgumentException.ThrowIfNullOrWhiteSpace(portConfig.IpOrName);
            ArgumentException.ThrowIfNullOrWhiteSpace(portConfig.Login);
            ArgumentNullException.ThrowIfNull(portConfig.Password);
            ArgumentNullException.ThrowIfNull(portConfig.SuperPassword);
            ArgumentException.ThrowIfNullOrWhiteSpace(portConfig.InterfaceName);
            ArgumentNullException.ThrowIfNull(portConfig.TrunkVlans);

            if (!portConfig.TrunkVlans.All(vlan => vlan > 0))
                throw new ArgumentException("VLAN cant be less 1.");

            if (portConfig.TrunkVlans.Any(vlan => portConfig.Vlans.Count(vl => vlan == vl) > 1))
                throw new ArgumentException("VLAN list can be unique.");
            #endregion


            SshClient sshClient = null;
            ShellStream shellStream = null;

            void CheckInterfaceOrSupportLinkType()
            {
                shellStream.Expect(new ExpectAction(new Regex("% Unrecognized command found at '\\^' position\\.|% Wrong parameter found at '\\^' position\\."), _ =>
                throw new SwitchServiceException(SwitchServiceErrorType.WrongInterface)),
                        new ExpectAction(SystemViewPromtShellRegex, _ => { }));
            }

            try
            {
                (sshClient, shellStream) = await GetConnectionAndShell(portConfig.IpOrName, portConfig.Login, portConfig.Password, portConfig.SuperPassword, cancellationToken);

                EnterInterface(shellStream, portConfig.InterfaceName);

                #region check_vlan_exist
                IEnumerable<int> vlansOnSwitch = GetOnlyVlanNums(shellStream);

                if (!portConfig.TrunkVlans.All(vl => vlansOnSwitch.Any(vlOnSw => vl == vlOnSw)))
                    throw new SwitchServiceException(SwitchServiceErrorType.VLANNotExist);
                #endregion

                shellStream.Expect(SystemViewPromtShellRegex);

                #region setting_for_trunk
                shellStream.WriteLineAndExpect("port link-type trunk");

                CheckInterfaceOrSupportLinkType();

                shellStream.WriteLineAndExpect("undo port trunk permit vlan all");

                if(portConfig.TrunkVlans.Count() > 0)
                    shellStream.WriteLineAndExpect($"port trunk permit vlan {string.Join(' ', portConfig.Vlans)}");
                #endregion

                //wait execute last command
                shellStream.Expect(SystemViewPromtShellRegex);

            }
            catch (SwitchServiceException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SwitchServiceException(SwitchServiceErrorType.Unknown, ex);
            }
            finally
            {
                shellStream?.Dispose();
                sshClient?.Dispose();
            }
        }

        public async Task ConfigurePort(PortConfigAccess portConfig, CancellationToken cancellationToken = default)
        {
            #region validation
            ArgumentNullException.ThrowIfNull(portConfig);
            ArgumentException.ThrowIfNullOrWhiteSpace(portConfig.IpOrName);
            ArgumentException.ThrowIfNullOrWhiteSpace(portConfig.Login);
            ArgumentNullException.ThrowIfNull(portConfig.Password);
            ArgumentNullException.ThrowIfNull(portConfig.SuperPassword);
            ArgumentException.ThrowIfNullOrWhiteSpace(portConfig.InterfaceName);
            ArgumentNullException.ThrowIfNull(portConfig.Vlans);

            if (portConfig.AccessVlan < 1)
                throw new ArgumentException("VLAN cant be less 1.");
            #endregion

            SshClient sshClient = null;
            ShellStream shellStream = null;

            void CheckInterfaceOrSupportLinkType()
            {
                shellStream.Expect(new ExpectAction(new Regex("% Unrecognized command found at '\\^' position\\.|% Wrong parameter found at '\\^' position\\."), _ =>
                throw new SwitchServiceException(SwitchServiceErrorType.WrongInterface)),
                        new ExpectAction(SystemViewPromtShellRegex, _ => { }));
            }

            try
            {
                (sshClient, shellStream) = await GetConnectionAndShell(portConfig.IpOrName, portConfig.Login, portConfig.Password, portConfig.SuperPassword, cancellationToken);

                EnterInterface(shellStream, portConfig.InterfaceName);

                #region check_vlan_exist
                IEnumerable<int> vlansOnSwitch = GetOnlyVlanNums(shellStream);

                if (!vlansOnSwitch.Any(vlOnSw => vlOnSw == portConfig.AccessVlan))
                    throw new SwitchServiceException(SwitchServiceErrorType.VLANNotExist);
                #endregion

                #region setting_for_access
                shellStream.WriteLineAndExpect("port link-type access");
                CheckInterfaceOrSupportLinkType();
                shellStream.WriteLineAndExpect($"port access vlan {portConfig.AccessVlan}");
                #endregion


                //wait execute last command
                shellStream.Expect(SystemViewPromtShellRegex);

            }
            catch (SwitchServiceException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SwitchServiceException(SwitchServiceErrorType.Unknown, ex);
            }
            finally
            {
                shellStream?.Dispose();
                sshClient?.Dispose();
            }
        }
    }
}