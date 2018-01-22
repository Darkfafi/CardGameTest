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
        XmlObjectReferences references = new XmlObjectReferences();
        XmlDocument doc = new XmlDocument();
        XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
        XmlElement root = doc.DocumentElement;
        doc.InsertBefore(dec, root);

        StreamWriter writer = new StreamWriter("Assets/nodesList.xml");

        nodesList.SaveNodes(doc, references);

        references.Saving_EndReferenceCounter();

        doc.Save(writer.BaseStream);
        writer.Close();
    }

    public void Load()
    {
        XmlObjectReferences references = new XmlObjectReferences();
        StreamReader reader = new StreamReader("Assets/nodesList.xml");
        XmlDocument doc = new XmlDocument();
        doc.Load(reader.BaseStream);
        nodesList.LoadNodes(doc, references);
        references.Loading_EndReferenceCounter();
        reader.Close();
    }

    public void Action()
    {
        for(int i = 0; i < nodesList.Nodes.Count; i++)
        {
            nodesList.Nodes[i].Activate();
        }
    }
}
