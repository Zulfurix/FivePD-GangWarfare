using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using CalloutAPI;

namespace FivePD_GangWarfare
{
    [CalloutProperties("Gang Warfare", "Zulfurix", "1.0", Probability.Low)]
    public class GangWarfare : CalloutAPI.Callout
    {
        static int maxAmountOfMembers = 10;
        static int minAmountOfMembers = 2;
        Random rnd = new Random();

        Ped[] SuspectsA, SuspectsB;
        int numOfSuspects;

        Vector3[] Locations =
        {
            new Vector3(98, -1935, 21)
        };

        PedHash[] PedModelsA =
        {
            PedHash.BallaEast01GMY,
            PedHash.BallaOrig01GMY,
            PedHash.Ballas01GFY
        };

        PedHash[] PedModelsB =
        {
            PedHash.Vagos01GFY,
            PedHash.MexGoon03GMY,
            PedHash.MexGoon01GMY
        };

        WeaponHash[] Weapons =
        {
            WeaponHash.Pistol,
            WeaponHash.PumpShotgun,
            WeaponHash.MiniSMG
        };

        public GangWarfare()
        {
            InitBase(Locations[rnd.Next(Locations.Length)]);

            // Callout Details
            this.ShortName = "Gang Warfare";
            this.CalloutDescription = "Gang warfare has broken out in the local area between two parties of individuals.";
            this.ResponseCode = 3;
            this.StartDistance = 200f;
        }

        public async override Task Init()
        {
            numOfSuspects = rnd.Next(minAmountOfMembers, maxAmountOfMembers);
            SuspectsA = new Ped[numOfSuspects];
            SuspectsB = new Ped[numOfSuspects];
            OnAccept();

            // Set up entities for Teams A and B
            for (int i = 0; i < numOfSuspects; i++)
            {
                ////////// TEAM A //////////
                SuspectsA[i] = await SpawnPed(PedModelsA[rnd.Next(0, PedModelsA.Length)], Location + new Vector3(i * 2.5f, 12, 0));
                SuspectsA[i].RelationshipGroup = GetHashKey("AMBIENT_GANG_MEXICAN");
                SuspectsA[i].Armor = 100;

                // Weapon Attributes
                SuspectsA[i].Accuracy = 0;
                SuspectsA[i].Weapons.Give(Weapons[rnd.Next(Weapons.Length)], 1000, true, true);
                SetPedCombatAttributes(SuspectsA[i].Handle, 45, true);

                ////////// TEAM B //////////
                SuspectsB[i] = await SpawnPed(PedModelsB[rnd.Next(0, PedModelsB.Length)], Location + new Vector3(i * 2.5f, -12, 0));
                SuspectsB[i].RelationshipGroup = GetHashKey("AMBIENT_GANG_BALLAS");
                SuspectsB[i].Armor = 100;

                // Weapon Attributes
                SuspectsB[i].Accuracy = 0;
                SuspectsB[i].Weapons.Give(Weapons[rnd.Next(Weapons.Length)], 1000, true, true);
                SetPedCombatAttributes(SuspectsB[i].Handle, 45, true);
            }

        }

        public override void OnStart(Ped player)
        {
            base.OnStart(player);
            // Relationship between gang 1 and gang 2
            SetRelationshipBetweenGroups((int)Relationship.Hate, (uint)GetHashKey("AMBIENT_GANG_MEXICAN"), (uint)GetHashKey("AMBIENT_GANG_BALLAS"));    
            SetRelationshipBetweenGroups((int)Relationship.Hate, (uint)GetHashKey("AMBIENT_GANG_BALLAS"), (uint)GetHashKey("AMBIENT_GANG_MEXICAN"));

            // Relationship between player and gang 1 / gang 2
            SetRelationshipBetweenGroups((int)Relationship.Hate, (uint)GetHashKey("AMBIENT_GANG_MEXICAN"), (uint)GetHashKey("PLAYER"));
            SetRelationshipBetweenGroups((int)Relationship.Hate, (uint)GetHashKey("PLAYER"), (uint)GetHashKey("AMBIENT_GANG_MEXICAN"));
            SetRelationshipBetweenGroups((int)Relationship.Hate, (uint)GetHashKey("AMBIENT_GANG_BALLAS"), (uint)GetHashKey("PLAYER"));
            SetRelationshipBetweenGroups((int)Relationship.Hate, (uint)GetHashKey("PLAYER"), (uint)GetHashKey("AMBIENT_GANG_BALLAS"));

            // Relationship between gang 1 / gang 1 & gang 2 / gang 2
            SetRelationshipBetweenGroups((int)Relationship.Companion, (uint)GetHashKey("AMBIENT_GANG_MEXICAN"), (uint)GetHashKey("AMBIENT_GANG_MEXICAN"));
            SetRelationshipBetweenGroups((int)Relationship.Companion, (uint)GetHashKey("AMBIENT_GANG_BALLAS"), (uint)GetHashKey("AMBIENT_GANG_BALLAS"));
            SuspectsA[0].Task.ShootAt(SuspectsB[0]);

            Tick += Update;
        }

        private async Task Update()
        {
            await BaseScript.Delay(100);
        }
    }
}
