using NUnit.Framework;
using UnityEngine;
using NEAT;

public class GenomeTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void NodeGenes()
    {
        // Test multiply node
        Genometype.NodeGenes node = new Genometype.NodeGenes(Genometype.NodeGenes.Types.Input);
        node.value = 1;
        Assert.AreEqual(node.value, 1);

        // Test node cloning
        Genometype.NodeGenes cloneNode = node;
        cloneNode.value = 2;
        Assert.AreEqual(node.value, 1);
        Assert.AreEqual(cloneNode.value, 2);
    }

    [Test]
    public void DataProcess()
    {
        Genometype.NodeGenes[] nodes = new Genometype.NodeGenes[] {
            new Genometype.NodeGenes(Genometype.NodeGenes.Types.Input, _IOIndex: 0),
            new Genometype.NodeGenes(Genometype.NodeGenes.Types.Hidden),
            new Genometype.NodeGenes(Genometype.NodeGenes.Types.Output),
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

    [Test]
    public void DataProcessMultiple()
    {
        Genometype.NodeGenes[] nodes = new Genometype.NodeGenes[] {
            new Genometype.NodeGenes(Genometype.NodeGenes.Types.Input, _IOIndex: 0),
            new Genometype.NodeGenes(Genometype.NodeGenes.Types.Input, _IOIndex: 1),
            new Genometype.NodeGenes(Genometype.NodeGenes.Types.Output, _addOnType: Genometype.NodeGenes.AddOnType.Multiply, _IOIndex: 0),
        };

        Genometype.ConnectionGenens[] connections = new Genometype.ConnectionGenens[] {
            new Genometype.ConnectionGenens(0, 2, 1f, _operatorType: Genometype.ConnectionGenens.OperatorType.Multiply),
            new Genometype.ConnectionGenens(1, 2, 1f, _operatorType: Genometype.ConnectionGenens.OperatorType.Multiply),
        };

        GenomeController controller = new GenomeController(new Genometype(nodes, connections));
        controller.Reset();

        // Test insert input and process data
        controller.Input(new float[] {3f, 2f});
        controller.StartProcess();
        Assert.AreEqual(controller.GetOutput(0), 6f);
    }

    [Test]
    public void GenomeConnectionMutation()
    {
        Genometype.NodeGenes[] nodes = new Genometype.NodeGenes[] {
            new Genometype.NodeGenes(Genometype.NodeGenes.Types.Input, _IOIndex: 0),
            new Genometype.NodeGenes(Genometype.NodeGenes.Types.Input, _IOIndex: 0),
            new Genometype.NodeGenes(Genometype.NodeGenes.Types.Hidden),
            new Genometype.NodeGenes(Genometype.NodeGenes.Types.Output),
        };

        GenomeMutationController controller = new GenomeMutationController(new Genometype(nodes, new Genometype.ConnectionGenens[0] { }));

        controller.MutateByAddingConnection();

        for (int i = 0; i < controller.mutations.Count; i++)
        {
            Debug.Log(controller.mutations[i]);
        }
    }

    [Test]
    public void GenomeNodeMutation()
    {
        Genometype.NodeGenes[] nodes = new Genometype.NodeGenes[] {
            new Genometype.NodeGenes(Genometype.NodeGenes.Types.Input, _IOIndex: 0),
            new Genometype.NodeGenes(Genometype.NodeGenes.Types.Input, _IOIndex: 0),
            new Genometype.NodeGenes(Genometype.NodeGenes.Types.Output),
        };

        Genometype.ConnectionGenens[] connections = new Genometype.ConnectionGenens[] {
            new Genometype.ConnectionGenens(0, 2, 0.123f),
            new Genometype.ConnectionGenens(1, 2, 3f),
        };

        GenomeMutationController controller = new GenomeMutationController(new Genometype(nodes, connections));

        controller.MutateByInsertNodeBetweenConnection();

        for (int i = 0; i < controller.mutations.Count; i++)
        {
            Debug.Log(controller.mutations[i]);
        }
    }
}