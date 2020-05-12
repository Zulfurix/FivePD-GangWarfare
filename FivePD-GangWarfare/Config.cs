using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace FivePD_GangWarfare
{
    class Config : BaseScript
    {
        string configFile = LoadResourceFile("fivepd", "callouts/FivePD-GangWarfare.json");
    }
}
