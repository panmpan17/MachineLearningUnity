using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NEAT;

public class GenomeTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void NodeCalculation()
    {
        Genometype.NodeGenes[] nodes = new Genometype.NodeGenes[] {
            new Genometype.NodeGenes(Genometype.NodeGenes.Types.Input, 1),
            new Genometype.NodeGenes(Genometype.NodeGenes.Types.Input, 0, Genometype.NodeGenes.OperatorType.Plus),
            new Genometype.NodeGenes(Genometype.NodeGenes.Types.Input, -1, _outputMode: Genometype.NodeGenes.OutputMode.Reverse),
        };

        Genometype genome = new Genometype(nodes, null);

        // Test multiply node
        genome.InputValueIntoNode(0, 4);
        genome.InputValueIntoNode(0, -2.4f);
        Assert.AreEqual(genome.GetNodeValue(0), 4 * -2.4f);

        // Test addition node
        genome.InputValueIntoNode(1, 4);
        genome.InputValueIntoNode(1, -2.4f);
        Assert.AreEqual(genome.GetNodeValue(1), 4 - 2.4f);

        // Test reverse output node
        Assert.AreEqual(genome.GetNodeValue(2), 1);
    }
}