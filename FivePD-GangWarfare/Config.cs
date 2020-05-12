using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using Newtonsoft.Json;

namespace FivePD_GangWarfare
{
    class Config : BaseScript
    {
        public static int maxAmountOfMembers;
        public static int minAmountOfMembers;

        public Config()
        {
            dynamic jsonText = JsonConvert.DeserializeObject("callouts/FivePD-GangWarfareConfig.json");
            if (jsonText != null)
            {
                maxAmountOfMembers = jsonText.config.gangs.maxAmountOfMembers;
                minAmountOfMembers = jsonText.config.gangs.mminmountOfMembers;
            }
        }

    }
}
