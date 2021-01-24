using HarmonyLib;
using XRL.World;

namespace WingysMod.HarmonyPatches
{
    [HarmonyPatch(typeof(GameObject))]
    class Patch_PhaseAndFlightMatches
    {
        [HarmonyPostfix]
        [HarmonyPatch("PhaseAndFlightMatches")]
        static void Postfix(GameObject GO, ref GameObject __instance, ref bool __result)
        {
            if (__result == true)
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