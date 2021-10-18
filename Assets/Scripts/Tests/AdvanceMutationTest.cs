using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using NEAT;
using FlappyBird;

public class AdvanceMutationTest
{
    [Test]
    public void CalculateRedundant()
    {
        var data = SavingSystem.GetGenome<GenomeEvolutionGameControl.GenomeStructEvolveData>("compound-data-stage-3.json", true);
        Genometype genometype = data.aliveGenomes[0];
        Debug.Log(genometype.nodeGenes.Length);
        Debug.Log(genometype.connectionGenes.Length);
        Debug.Log(GenomtypeEvaluator.CountDisabledConnections(genometype));
        Debug.Log(GenomtypeEvaluator.CountRepeatedConnections(genometype));
    }

    public static class GenomtypeEvaluator
    {
        public static int CountDisabledConnections(Genometype genometype)
        {
            int count = 0;

            for (int i = 0; i < genometype.connectionGenes.Length; i++)
            {
                if (!genometype.connectionGenes[i].enabled)
                    count++;
            }

            return count;
        }

        public static int CountRepeatedConnections(Genometype genometype)
        {
            int count = 0;
            Dictionary<int, List<int>> connectionMap = new Dictionary<int, List<int>>();

            for (int i = 0; i < genometype.connectionGenes.Length; i++)
            {
                int fromIndex = genometype.connectionGenes[i].inputNodeIndex;
                if (connectionMap.ContainsKey(fromIndex))
                {
                    if (connectionMap[fromIndex].Contains(genometype.connectionGenes[i].outputNodeIndex))
                    {
                        count++;
                    }
                    else
                    {
                        connectionMap[fromIndex].Add(genometype.connectionGenes[i].outputNodeIndex);
                    }
                }
                else
                {
                    connectionMap.Add(fromIndex, new List<int>(new int[] {
                        genometype.connectionGenes[i].outputNodeIndex
                    }));
                }
            }

            return count;
        }
    }
}
