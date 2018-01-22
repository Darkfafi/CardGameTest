using System;
using System.Collections.Generic;
using System.Linq;

public class NodesList : ISaveContainer
{
    public List<Node> Nodes = new List<Node>();

    public void SaveablesToLoad(object[] saveables)
    {
        Nodes = new List<Node>(saveables.Cast<Node>());
    }

    public ISaveable[] SaveablesToSave()
    {
        return Nodes.ToArray();
    }
}
