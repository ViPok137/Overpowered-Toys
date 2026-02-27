using HarmonyLib;
using System;
using Verse;
using RimWorld;

namespace OPFloorMod
{
    [StaticConstructorOnStartup]
    public static class OPFloorHarmony
    {
        static OPFloorHarmony()
        {
            var harmony = new Harmony("com.vipok.OPFloor");
            harmony.PatchAll();
        }
    }

    // ИСПРАВЛЕНИЕ: Патчим StatExtension, так как GetStatValue находится там.
    // ВАЖНО: Патчить GetStatValue опасно для производительности. 
    // Лучше делать проверки максимально быстрыми.
    [HarmonyPatch(typeof(StatExtension), nameof(StatExtension.GetStatValue), new Type[] { typeof(Thing), typeof(StatDef), typeof(bool), typeof(int) })]
    public static class Patch_StatExtension_GetStatValue
    {
        // Кэшируем Def, чтобы не искать его по строке каждый раз (это очень медленно)
        private static TerrainDef cachedTerrainDef;
        private static TerrainDef TargetTerrain
        {
            get
            {
                if (cachedTerrainDef == null)
                    cachedTerrainDef = DefDatabase<TerrainDef>.GetNamedSilentFail("OP_BrokenRealityFloor");
                return cachedTerrainDef;
            }
        }

        // В методах расширения первым аргументом идет сам объект (thing), а не __instance
        public static void Postfix(Thing thing, StatDef stat, ref float __result)
        {
            // 1. Быстрая проверка на стат (самая дешевая операция)
            if (stat != StatDefOf.MoveSpeed) return;

            // 2. Проверка, что это пешка и она на карте
            if (!(thing is Pawn pawn) || !pawn.Spawned) return;

            // 3. Проверка террейна
            // Используем .topGrid вместо .terrainGrid для микро-оптимизации, если возможно, 
            // но стандартный TerrainAt надежнее.
            if (pawn.Map.terrainGrid.TerrainAt(pawn.Position) == TargetTerrain)
            {
                __result *= 10f;
            }
        }
    }
}