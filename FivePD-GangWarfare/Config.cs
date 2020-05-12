using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;

namespace FivePD_GangWarfare
{
    class Config : BaseScript
    {
        public static int minAmountOfMembers;
        public static int maxAmountOfMembers;
        public static int ambientVehicleChance;

        public static void LoadConfig()
        {
            string jsonText = LoadResourceFile("fivepd", "callouts/FivePD-GangWarfareConfig.json");
            dynamic configFile = JsonConvert.DeserializeObject(jsonText);
            try
            {
                minAmountOfMembers = (int)configFile["config"]["minAmountOfMembers"];
                maxAmountOfMembers = (int)configFile["config"]["maxAmountOfMembers"];
                ambientVehicleChance = (int)configFile["config"]["ambientVehicleChance"];
            }
            catch (Exception ex)
            {
                minAmountOfMembers = 1;
                maxAmountOfMembers = 1;
                ambientVehicleChance = 50;
            }

        }

    }
}
