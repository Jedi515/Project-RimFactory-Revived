using System;
using System.Collections.Generic;
using Verse;

namespace ProjectRimFactory.Common.HarmonyPatches
{
    static class PatchStorageUtil
    {
        private static Dictionary<Tuple<Map, IntVec3, Type>, object> cache = new Dictionary<Tuple<Map, IntVec3, Type>, object>();
        private static int lastTick = 0;
        private static Dictionary<Map, PRFMapComponent> mapComps = new Dictionary<Map, PRFMapComponent>();

        public static PRFMapComponent GetPRFMapComponent (Map map)
        {
            PRFMapComponent outval = null;
            if (map is not null && !mapComps.TryGetValue(map,out outval))
            {
                outval = map.GetComponent<PRFMapComponent>();
                mapComps.Add(map, outval);
            }
            return outval;
        }

        public static T Get<T>(Map map, IntVec3 pos) where T : class
        {
            return pos.IsValid ? pos.GetFirst<T>(map) : null;
        }

        public static T GetWithTickCache<T>(Map map, IntVec3 pos) where T : class
        {
            if (Find.TickManager.TicksGame != lastTick)
            {
                cache.Clear();
                lastTick = Find.TickManager.TicksGame;
            }
            var key = new Tuple<Map, IntVec3, Type>(map, pos, typeof(T));
            if (!cache.TryGetValue(key, out object val))
            {
                val = Get<T>(map, pos);
                cache.Add(key, val);
            }

            return (T)val;
        }
    }
}
