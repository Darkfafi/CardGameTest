using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class SuperNode2<T> : Node<SuperNode2Data>
{
    public SuperNode2() : base(new SuperNode2Data())
    {
    }

    protected override void SuperLoad(XmlElement saveData, XmlObjectReferences references)
    {
        uint counterId = uint.Parse(saveData.GetElementsByTagName("NodeRef").Item(0).InnerText);
        references.Loading_GetReferenceFrom(counterId, Node1DataReferenceLoaded);
    }

    private void Node1DataReferenceLoaded(uint counterId, object objectReference)
    {
        NodeData.Node1Data = (Node)objectReference;
        //Debug.Log(NodeData.Node1Data + " <= " + counterId);
    }

    protected override void SuperSave(XmlElement nodeElement, XmlDocument doc, XmlObjectReferences references)
    {
        XmlElement data = doc.CreateElement("Data");
        XmlElement superNodeRef = doc.CreateElement("NodeRef");
        data.AppendChild(superNodeRef);
        superNodeRef.AppendChild(doc.CreateTextNode(references.Saving_GetRefCounterFor(NodeData.Node1Data).ToString()));
        nodeElement.AppendChild(data);
    }

    public override void Activate()
    {
        Debug.Log(NodeQuote + " " + NodeData.Node1Data.NodeQuote);
    }
}

public class SuperNode2Data : NodeData
{
    public Node Node1Data;
}
