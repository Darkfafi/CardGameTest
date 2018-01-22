using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class SuperNode1 : Node<SuperNode1Data>
{
    public enum SomeCoolEnum
    {
        FirstElement,
        SecondElement,
        SuperCoolElement
    }

    public SuperNode1() : base(new SuperNode1Data())
    {

    }

    protected override void SuperLoad(XmlElement saveData, XmlObjectReferences references)
    {
        NodeData.Number1 = int.Parse(saveData.GetElementsByTagName("N1").Item(0).InnerText);
        NodeData.Number2 = int.Parse(saveData.GetElementsByTagName("N2").Item(0).InnerText);
        NodeData.Result = int.Parse(saveData.GetElementsByTagName("R").Item(0).InnerText);
        NodeData.EnumItem = (SomeCoolEnum)int.Parse(saveData.GetElementsByTagName("EnumItem").Item(0).InnerText);
    }

    protected override void SuperSave(XmlElement nodeElement, XmlDocument doc, XmlObjectReferences references)
    {
        XmlElement data = doc.CreateElement("Data");

        XmlElement n1 = doc.CreateElement("N1");
        XmlElement n2 = doc.CreateElement("N2");
        XmlElement r = doc.CreateElement("R");
        XmlElement enumElement = doc.CreateElement("EnumItem");

        n1.AppendChild(doc.CreateTextNode(NodeData.Number1.ToString()));
        n2.AppendChild(doc.CreateTextNode(NodeData.Number2.ToString()));
        r.AppendChild(doc.CreateTextNode(NodeData.Result.ToString()));
        enumElement.AppendChild(doc.CreateTextNode(((int)NodeData.EnumItem).ToString()));

        data.AppendChild(n1);
        data.AppendChild(n2);
        data.AppendChild(r);
        data.AppendChild(enumElement);

        nodeElement.AppendChild(data);
    }

    public override void Activate()
    {
        NodeData.Result = NodeData.Number1 + NodeData.Number2;
        Debug.Log(NodeQuote + " " + NodeData.Number1 + "+" + NodeData.Number2 + "=" + NodeData.Result + " " + NodeData.EnumItem);
    }
}

public class SuperNode1Data : NodeData
{
    public int Number1;
    public int Number2;
    public int Result;
    public SuperNode1.SomeCoolEnum EnumItem;
}