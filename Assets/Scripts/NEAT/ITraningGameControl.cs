using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace NEAT
{
    public interface ITraningGameControl
    {
        float GameStartTime { get; }

        IGenomeAgent GetGenomeControlFromPool();
    }
}