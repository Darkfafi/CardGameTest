using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using System.Reflection;

public class NodesList 
{
    public List<Node> Nodes = new List<Node>();

    public void SaveNodes(XmlDocument document, XmlObjectReferences references)
    {
        XmlElement nodesList = document.CreateElement("Nodes");

        for(int i = 0; i < Nodes.Count; i++)
        {
            nodesList.AppendChild(Nodes[i].Save(document, references));
        }

        document.AppendChild(nodesList);
    }

    public void LoadNodes(XmlDocument document, XmlObjectReferences references)
    {
        
        XmlNodeList l = document.GetElementsByTagName("Node");

        for (int i = 0; i < l.Count; i++)
        {
            Node n = (Node)Activator.CreateInstance(Type.GetType(l.Item(i).Attributes.GetNamedItem("NodeType").Value));
            XmlElement e = GetElement(l.Item(i).OuterXml);
            n.Load(e, references);
            Nodes.Add(n);
        }
    }

    private XmlElement GetElement(string xml)
    {
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xml);
        return doc.DocumentElement;
    }
}
