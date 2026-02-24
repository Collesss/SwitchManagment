using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchManagment.API.SwitchService.Exceptions
{
    public enum SwitchServiceErrorType
    {
        Unknown,
        HostNotExistOrUnreac,
        WrongLoginOrPass,
        WrongSuperPass,
        WrongInterface
    }
}
