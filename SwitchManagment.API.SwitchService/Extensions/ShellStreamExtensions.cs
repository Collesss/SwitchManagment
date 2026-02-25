using Renci.SshNet;

namespace SwitchManagment.API.SwitchService.Extensions
{
    public static class ShellStreamExtensions
    {
        public static string WriteLineAndExpect(this ShellStream shellStream, string line)
        {
            shellStream.WriteLine(line);
            return shellStream.Expect(line);
        }
    }
}
