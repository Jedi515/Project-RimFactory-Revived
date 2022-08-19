using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using ProjectRimFactory.Storage;
using RimWorld;
using Verse;
using Verse.AI;
using System.Reflection.Emit;

namespace ProjectRimFactory.Common.HarmonyPatches
{
	[HarmonyPatch(typeof(Verse.AI.HaulAIUtility), "HaulToStorageJob")]
    class Patch_HaulAIUtility_HaulToStorageJob
    {

		static bool foundPos = false;
		static int cnt = 0;
		

		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
		{

			Label end = generator.DefineLabel();
			Label preend = generator.DefineLabel();

			foreach (var instruction in instructions)
			{

				if (instruction.opcode == OpCodes.Brfalse_S) cnt++;

				if (instruction.opcode == OpCodes.Brfalse_S && (cnt == 2 || cnt == 3)) 
				{
					yield return new CodeInstruction(OpCodes.Brfalse_S, preend);
					continue;
				}



				if (!foundPos && instruction.opcode == OpCodes.Ldstr)
                {
					foundPos = true;
					CodeInstruction code = new CodeInstruction(OpCodes.Nop);
					code.labels.Add(preend);

					yield return code;
					
					yield return new CodeInstruction(OpCodes.Ldloc_2);

					yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(
						typeof(Patch_HaulAIUtility_HaulToStorageJob),
						nameof(Patch_HaulAIUtility_HaulToStorageJob.IsDSU), new[] { typeof(IHaulDestination) }));

					yield return new CodeInstruction(OpCodes.Brfalse_S, end);



					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Ldarg_1);

					yield return new CodeInstruction(OpCodes.Ldloc_2);

					yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(
						typeof(Patch_HaulAIUtility_HaulToStorageJob),
						nameof(Patch_HaulAIUtility_HaulToStorageJob.HaulToCellStorageJob), new[] { typeof(Pawn) , typeof(Thing) , typeof(IHaulDestination) }));
					yield return new CodeInstruction(OpCodes.Ret);
					

					instruction.labels.Add(end);
					//yield return new CodeInstruction(end);


				}



				yield return instruction;
			}

		}

		public static Job HaulToCellStorageJob(Pawn p, Thing t, IHaulDestination dest)
		{
			//Probaly not that good
			//When do i register it?
			Building_MassStorageUnit dsu = dest as Building_MassStorageUnit;

			Job job = JobMaker.MakeJob(JobDefOf.HaulToContainer, t, dsu);
			
			
		
			job.count = 99999;
			
			job.haulOpportunisticDuplicates = true;
			job.haulMode = HaulMode.ToCellStorage;

			

			return job;
		}

		public static bool IsDSU(IHaulDestination destination)
        {
            return (destination as Building_MassStorageUnit) != null;
        }


    }
}
