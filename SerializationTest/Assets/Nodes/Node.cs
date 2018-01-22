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

public abstract class Node : Saveable
{
    public string NodeQuote;
    public string TypeString
    {
        get
        {
            return GetType().FullName;
        }
    }

    public XmlElement Save(XmlDocument doc, XmlObjectReferences references)
    {
        XmlElement node = doc.CreateElement("Node");

        XmlElement quoteElement = doc.CreateElement("NodeQuote");

        XmlText quoteText = doc.CreateTextNode(NodeQuote);

        quoteElement.AppendChild(quoteText);
        node.AppendChild(quoteElement);
        node.SetAttribute("NodeType", TypeString);
        node.SetAttribute("SerializedReference", references.Saving_UseRefCounter(this).ToString());

        SuperSave(node, doc, references);

        return node;

    }

    public virtual void Activate()
    {
        Debug.Log(NodeQuote);
    }

    protected abstract void SuperSave(XmlElement nodeElement, XmlDocument doc, XmlObjectReferences references);
    protected abstract void SuperLoad(XmlElement saveData, XmlObjectReferences references);

    public void Load(XmlElement saveData, XmlObjectReferences references)
    {
        NodeQuote = saveData.GetElementsByTagName("NodeQuote").Item(0).InnerText;
        uint counterId = uint.Parse(saveData.GetAttribute("SerializedReference"));
        references.Loading_SetRefCounterFor(this, counterId);
        SuperLoad(saveData, references);
    }
}

public interface Saveable
{
    string TypeString { get; }
    XmlElement Save(XmlDocument doc, XmlObjectReferences references);
    void Load(XmlElement saveData, XmlObjectReferences references);
}