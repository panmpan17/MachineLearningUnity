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
    public void NodeGenes()
    {
        // Test multiply node
        Genometype.NodeGenes node = new Genometype.NodeGenes(Genometype.NodeGenes.Types.Input, 1);
        Assert.AreEqual(node.Value, 1);

        // Test reverse output node
        Genometype.NodeGenes reverseNode = new Genometype.NodeGenes(Genometype.NodeGenes.Types.Input, -1,
            _outputMode: Genometype.NodeGenes.OutputMode.Reverse);
        Assert.AreEqual(reverseNode.Value, 1);

        // Test node cloning
        Genometype.NodeGenes cloneNode = node;
        cloneNode.value = 2;
        Assert.AreEqual(node.Value, 1);
        Assert.AreEqual(cloneNode.Value, 2);
    }

    [Test]
    public void DataProcess()
    {
        Genometype.NodeGenes[] nodes = new Genometype.NodeGenes[] {
            new Genometype.NodeGenes(Genometype.NodeGenes.Types.Input, 0),
            new Genometype.NodeGenes(Genometype.NodeGenes.Types.Hidden, 0),
            new Genometype.NodeGenes(Genometype.NodeGenes.Types.Output, 0),
        };

        Genometype.ConnectionGenens[] connections = new Genometype.ConnectionGenens[] {
            new Genometype.ConnectionGenens(0, 1, 0.1f),
            new Genometype.ConnectionGenens(1, 2, 1f, Genometype.ConnectionGenens.OperatorType.Plus),
        };

        GenomeController controller = new GenomeController(new Genometype(nodes, connections));

        // Test get output
        controller.Reset();
        Assert.AreEqual(controller.GetOutput(0), 0);

        // Test insert input and process data
        controller.Input(new float[] {0.9f});
        controller.StartProcess();
        Assert.AreEqual(controller.GetOutput(0), 1.09f);
    }
}