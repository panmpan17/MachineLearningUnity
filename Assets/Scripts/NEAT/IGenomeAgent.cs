using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace NEAT
{
    public interface IGenomeAgent
    {
        GameObject gameObject { get; }
        Genometype GenomeData { get; }

        void Prepare(ITraningGameControl gameControl, Genometype genome);
    }
}