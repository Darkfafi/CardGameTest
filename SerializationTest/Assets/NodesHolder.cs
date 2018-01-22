using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;

public class NodesHolder : MonoBehaviour
{
    private NodesList nodesList = new NodesList();
	
    protected void Awake()
    {
        return;
        for(int i = 0; i < 100; i++)
        {
            SuperNode1 sn1 = new SuperNode1();
            sn1.NodeQuote = "<" + i + ">";
            sn1.NodeData.EnumItem = SuperNode1.SomeCoolEnum.SecondElement;
            sn1.NodeData.Number1 = Random.Range(0, 100);
            sn1.NodeData.Number2 = Random.Range(0, 100);
            nodesList.Nodes.Add(sn1);
        }

        for (int i = 0; i < 100; i++)
        {
            SuperNode2 sn2 = new SuperNode2();
            sn2.NodeData.Node1Data = nodesList.Nodes[Random.Range(0, 100)];
            nodesList.Nodes.Add(sn2);
        }
    }

    public void Save()
    {
        string path = "Assets/nodesList";
        XmlObjectReferences references = new XmlObjectReferences();
        references.Saving_SaveContainer(nodesList, path);
    }

    public void Load()
    {
        string path = "Assets/nodesList";
        XmlObjectReferences references = new XmlObjectReferences();
        references.Loading_LoadContainer(nodesList, path);
    }

    public void Action()
    {
        for(int i = 0; i < nodesList.Nodes.Count; i++)
        {
            nodesList.Nodes[i].Activate();
        }
    }
}
