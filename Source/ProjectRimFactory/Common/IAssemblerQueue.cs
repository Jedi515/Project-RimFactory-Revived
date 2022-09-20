using System.Collections.Generic;
using Verse;

namespace ProjectRimFactory.Common
{
    public interface IAssemblerQueue
    {
        Map Map { get; }
        List<Thing> GetThingQueue();
    }
}
