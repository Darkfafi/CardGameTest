using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;

[Serializable]
public abstract class Node<T> : Node where T : NodeData
{
    public T NodeData { get; private set; }

    public Node(T nodeData)
    {
        NodeQuote = "Basic Quote";
        NodeData = nodeData;
    }
}

public class NodeData { }

public abstract class Node : ISaveable
{
    public string NodeQuote;

    public void Save(XmlDocument doc, XmlObjectReferences references, XmlElement saveableElement)
    {
        XmlElement quoteElement = doc.CreateElement("NodeQuote");
        quoteElement.AppendChild(doc.CreateTextNode(NodeQuote));
        saveableElement.AppendChild(quoteElement);
        SuperSave(saveableElement, doc, references);
    }

    public void Load(XmlElement savedData, XmlObjectReferences references)
    {
        NodeQuote = savedData.GetElementsByTagName("NodeQuote").Item(0).InnerText;
        SuperLoad(savedData, references);
    }

    public virtual void Activate()
    {
        Debug.Log(NodeQuote);
    }

    protected abstract void SuperSave(XmlElement nodeElement, XmlDocument doc, XmlObjectReferences references);
    protected abstract void SuperLoad(XmlElement saveData, XmlObjectReferences references);
}