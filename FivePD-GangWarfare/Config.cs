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

        public static string[] ambientVehicleModels;
        public static string[] weaponModels;
        public static string[] pedModelsA;
        public static string[] pedModelsB;


        public static void LoadConfig()
        {
            string jsonText = LoadResourceFile("fivepd", "callouts/FivePD-GangWarfare/FivePD-GangWarfareConfig.json");
            dynamic configFile = JsonConvert.DeserializeObject(jsonText);
            try
            {
                minAmountOfMembers = (int)configFile["config"]["minAmountOfMembers"];

                maxAmountOfMembers = (int)configFile["config"]["maxAmountOfMembers"];

                ambientVehicleChance = (int)configFile["config"]["ambientVehicle"]["ambientVehicleChance"];

                // Parse stored vehicle model strings
                JArray jarr = (JArray)configFile["config"]["ambientVehicle"]["vehicleModels"];
                ambientVehicleModels = jarr.ToObject<string[]>();

                // Parse stored weapon model strings
                jarr = (JArray)configFile["config"]["weapons"];
                weaponModels = jarr.ToObject<string[]>();

                // Parse stored gang ped model strings
                jarr = (JArray)configFile["config"]["gangPedModels"]["gangA"];
                pedModelsA = jarr.ToObject<string[]>();

                jarr = (JArray)configFile["config"]["gangPedModels"]["gangB"];
                pedModelsB = jarr.ToObject<string[]>();

                if ((bool)configFile["config"]["debug"]["debugPrint"])
                {
                    Debug.WriteLine("[FivePD-GangWarfare]: Setting => minimum of " + minAmountOfMembers + " gang members");
                    Debug.WriteLine("[FivePD-GangWarfare]: Setting => maximum of " + maxAmountOfMembers + " gang members");
                    Debug.WriteLine("[FivePD-GangWarfare]: Setting => " + ambientVehicleChance + " chance of ambient vehicle spawning");
                    Debug.WriteLine("[FivePD-GangWarfare]: Parsed " + ambientVehicleModels.Length + " vehicles");
                    Debug.WriteLine("[FivePD-GangWarfare]: Parsed " + weaponModels.Length + " weapons");
                    Debug.WriteLine("[FivePD-GangWarfare]: Parsed " + (pedModelsA.Length + pedModelsB.Length) + " ped models");
                }
            }
            catch (Exception ex)
            {
                // Fallback to these values in the case of an IO exception

                minAmountOfMembers = 1;
                maxAmountOfMembers = 1;
                ambientVehicleChance = 50;

                ambientVehicleModels = new string[] 
                { 
                    "Emperor"
                };

                weaponModels = new string[]
                {
                    "Pistol"
                };

                pedModelsA = new string[]
                {
                    "BallaEast01GMY"
                };

                pedModelsB = new string[]
                {
                    "Vagos01GFY"
                };

                Debug.WriteLine("[FivePD-GangWarfare]: " + ex.Message);
                Debug.WriteLine("[FivePD-GangWarfare]: Fallback values will be used for configuration!");
            }

        }

    }
}
