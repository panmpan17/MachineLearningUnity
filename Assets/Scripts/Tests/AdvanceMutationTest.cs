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
        GenomeEvolutionGameControl.GenomeStructEvolveData data = SavingSystem.ReadData<GenomeEvolutionGameControl.GenomeStructEvolveData>("compound-data-stage-3.json", true);
        Genometype genometype = data.aliveGenomes[0];
        // Debug.Log(genometype.nodeGenes.Length);
        // Debug.Log(genometype.connectionGenes.Length);
        // Debug.Log(GenomtypeEvaluator.CountDisabledConnections(genometype));
        // Debug.Log(GenomtypeEvaluator.CountRepeatedConnections(genometype));

        int needRemove = GenomtypeEvaluator.CountWasteConnection(genometype);
        Genometype.ConnectionGenens[] newConnections = new Genometype.ConnectionGenens[genometype.connectionGenes.Length - needRemove];
        for (int i = 0; i < newConnections.Length; i++)
        {
            newConnections[i] = genometype.connectionGenes[i];
            Debug.LogFormat("{0} -> {1}", newConnections[i].inputNodeIndex, newConnections[i].outputNodeIndex);
        }
        genometype.connectionGenes = newConnections;

        GenometypeGraphVisualizer visualizer = new GenometypeGraphVisualizer(genometype);
        visualizer.Export("trim-connection", false);
        // genometype.connectionGenes = 
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
    
        public static int CountWasteConnection(Genometype genometype)
        {
            int count = 0;

            for (int i = genometype.connectionGenes.Length - 1; i >= 0; i--)
            {
                if (genometype.nodeGenes[genometype.connectionGenes[i].outputNodeIndex].type == Genometype.NodeGenes.Types.Output)
                {
                    break;
                }
                count++;
            }

            return count;
        }
    }
}
