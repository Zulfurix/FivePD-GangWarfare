using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using FivePD.API;

namespace FivePD_GangWarfare
{
    [CalloutProperties("Gang Warfare", "Zulfurix", "1.0.3")]
    public class GangWarfare : FivePD.API.Callout
    {
        Random rnd = new Random();

        Ped[] SuspectsA, SuspectsB;
        Vehicle ambientVeh;
        int numOfSuspects;

        Vector3[] Locations =
        {
            new Vector3(98, -1935, 21),
            new Vector3(319, -2078, 18),
            new Vector3(428, -1867, 27),
            new Vector3(509, -1718, 29),
            new Vector3(557, -1921, 25),
            new Vector3(849, -2340, 30),
            new Vector3(988, -2374, 31),
            new Vector3(498, -2568, 7),
            new Vector3(-106, -2442, 6),
            new Vector3(236, -1723, 29),
            new Vector3(913, -1240, 26),
            new Vector3(-178, -1301, 31),
            new Vector3(-89, -1424, 30)
        };

        public GangWarfare()
        {
            InitInfo(Locations[rnd.Next(Locations.Length)]);
            Config.LoadConfig();

            // Callout Details
            this.ShortName = "Gang Warfare";
            this.CalloutDescription = "Gang warfare has broken out in the local area between two parties of individuals.";
            this.ResponseCode = 3;
            this.StartDistance = 125f;
        }

        public async override Task OnAccept()
        {
            InitBlip();

            numOfSuspects = rnd.Next(Config.minAmountOfMembers, Config.maxAmountOfMembers);
            SuspectsA = new Ped[numOfSuspects];
            SuspectsB = new Ped[numOfSuspects];

            // Create relationship groups
            World.AddRelationshipGroup("GANG_A");
            World.AddRelationshipGroup("GANG_B");

            // Set up entities for Teams A and B
            for (int i = 0; i < numOfSuspects; i++)
            {
                ////////// TEAM A //////////
                PedHash generatedPedHash = (PedHash)GetHashKey(Config.pedModelsA[rnd.Next(0, Config.pedModelsA.Length)]);
                SuspectsA[i] = await SpawnPed(generatedPedHash, Location + new Vector3(i * 2.5f, 12, 0));
                SuspectsA[i].RelationshipGroup = GetHashKey("GANG_A");
                SuspectsA[i].Health = 250;

                // Weapon Attributes
                WeaponHash generatedWeaponHash = (WeaponHash)GetHashKey(Config.weaponModels[rnd.Next(Config.weaponModels.Length)]);
                SuspectsA[i].Accuracy = 0;
                SuspectsA[i].Weapons.Give(generatedWeaponHash, 1000, true, true);
                SetPedCombatAttributes(SuspectsA[i].Handle, 45, true);

                ////////// TEAM B //////////
                generatedPedHash = (PedHash)GetHashKey(Config.pedModelsB[rnd.Next(0, Config.pedModelsB.Length)]);
                SuspectsB[i] = await SpawnPed(generatedPedHash, Location + new Vector3(i * 2.5f, -12, 0));
                SuspectsB[i].RelationshipGroup = GetHashKey("GANG_B");
                SuspectsB[i].Health = 250;

                // Weapon Attributes
                generatedWeaponHash = (WeaponHash)GetHashKey(Config.weaponModels[rnd.Next(Config.weaponModels.Length)]);
                SuspectsB[i].Accuracy = 0;
                SuspectsB[i].Weapons.Give(generatedWeaponHash, 1000, true, true);
                SetPedCombatAttributes(SuspectsB[i].Handle, 45, true);
            }

            // Create ambient vehicle
            if (rnd.Next(0, 100) < Config.ambientVehicleChance)
            {
                VehicleHash generatedVehicleHash = (VehicleHash)GetHashKey(Config.ambientVehicleModels[rnd.Next(Config.ambientVehicleModels.Length)]);
                ambientVeh = await SpawnVehicle(generatedVehicleHash, Location, rnd.Next(360));
                ambientVeh.IsEngineRunning = true;
                ambientVeh.AreLightsOn = true;
            }
        }

        public override void OnStart(Ped player)
        {
            base.OnStart(player);

            // Relationship between gang 1 and gang 2
            SetRelationshipBetweenGroups((int)Relationship.Hate, (uint)GetHashKey("GANG_A"), (uint)GetHashKey("GANG_B"));    
            SetRelationshipBetweenGroups((int)Relationship.Hate, (uint)GetHashKey("GANG_B"), (uint)GetHashKey("GANG_A"));

            // Relationship between player and gang 1 / gang 2
            SetRelationshipBetweenGroups((int)Relationship.Hate, (uint)GetHashKey("PLAYER"), (uint)GetHashKey("GANG_A"));
            SetRelationshipBetweenGroups((int)Relationship.Hate, (uint)GetHashKey("GANG_A"), (uint)GetHashKey("PLAYER"));
            SetRelationshipBetweenGroups((int)Relationship.Hate, (uint)GetHashKey("PLAYER"), (uint)GetHashKey("GANG_B"));
            SetRelationshipBetweenGroups((int)Relationship.Hate, (uint)GetHashKey("GANG_B"), (uint)GetHashKey("PLAYER"));

            // Relationship between gang 1 / gang 1 & gang 2 / gang 2
            SetRelationshipBetweenGroups((int)Relationship.Companion, (uint)GetHashKey("GANG_A"), (uint)GetHashKey("GANG_A"));
            SetRelationshipBetweenGroups((int)Relationship.Companion, (uint)GetHashKey("GANG_B"), (uint)GetHashKey("GANG_B"));

            // Trigger gunfight between peds
            SuspectsA[0].Task.ShootAt(SuspectsB[0]);

            if (ambientVeh != null)
            {
                ambientVeh.MarkAsNoLongerNeeded();
            }
        }

        private void ClearAllRelationships()
        {
            // Relationship between gang 1 and gang 2
            ClearRelationshipBetweenGroups((int)Relationship.Hate, (uint)GetHashKey("GANG_A"), (uint)GetHashKey("GANG_B"));
            ClearRelationshipBetweenGroups((int)Relationship.Hate, (uint)GetHashKey("GANG_B"), (uint)GetHashKey("GANG_A"));

            // Relationship between player and gang 1 / gang 2
            ClearRelationshipBetweenGroups((int)Relationship.Hate, (uint)GetHashKey("PLAYER"), (uint)GetHashKey("GANG_A"));
            ClearRelationshipBetweenGroups((int)Relationship.Hate, (uint)GetHashKey("GANG_A"), (uint)GetHashKey("PLAYER"));
            ClearRelationshipBetweenGroups((int)Relationship.Hate, (uint)GetHashKey("PLAYER"), (uint)GetHashKey("GANG_B"));
            ClearRelationshipBetweenGroups((int)Relationship.Hate, (uint)GetHashKey("GANG_B"), (uint)GetHashKey("PLAYER"));

            // Relationship between gang 1 / gang 1 & gang 2 / gang 2
            ClearRelationshipBetweenGroups((int)Relationship.Companion, (uint)GetHashKey("GANG_A"), (uint)GetHashKey("GANG_A"));
            ClearRelationshipBetweenGroups((int)Relationship.Companion, (uint)GetHashKey("GANG_B"), (uint)GetHashKey("GANG_B"));

            RemoveRelationshipGroup((uint)GetHashKey("GANG_B"));
            RemoveRelationshipGroup((uint)GetHashKey("GANG_A"));
        }

        public override void OnCancelBefore()
        {
            ClearAllRelationships();
            base.OnCancelBefore();
        }
    }
}
