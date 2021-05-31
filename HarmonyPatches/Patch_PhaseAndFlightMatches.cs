using HarmonyLib;
using XRL.World;

namespace WingysMod.HarmonyPatches
{
    [HarmonyPatch(typeof(GameObject))]
    public class Patch_PhaseAndFlightMatches
    {
        public static bool TemporarilyDisabled = false;

        [HarmonyPostfix]
        [HarmonyPatch("PhaseAndFlightMatches")]
        static void Postfix(GameObject GO, ref GameObject __instance, ref bool __result)
        {
            if (__result == true && TemporarilyDisabled == false)
            {
                if (__instance.IsCreature && GO.IsCreature)
                {
                    bool thisObjectSubmerged = __instance.HasEffect("Submerged");
                    bool otherObjectSubmerged = GO.HasEffect("Submerged");
                    if (thisObjectSubmerged || otherObjectSubmerged)
                    {
                        bool bothSubmerged = thisObjectSubmerged && otherObjectSubmerged;
                        if (!bothSubmerged)
                        {
                            //change result of "PhaseAndFlightMatches" method to false
                            __result = false;
                        }
                    }
                }
            }
        }
    }
}