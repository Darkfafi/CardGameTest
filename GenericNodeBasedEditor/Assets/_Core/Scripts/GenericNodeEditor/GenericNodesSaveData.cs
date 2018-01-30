using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Xml;
using System;

public class GenericNodesSaveData : ISaveContainer
{
    public const string DATA_ROOT_TAG = "GNE_SaveFile";

    // Saved Connections
    // -- All Nodes connection data should be stored here
    public ConnectionModel[] ConnectionModels;

    // Saved Views
    // -- All Node visual data should be stored here
    public ViewData[] NodeViewData;

    // Saved Models
    // -- All Node Logic holders should be stored here
    public BaseNodeModel[] NodeModels;

    // Saved Socket Models
    public BaseNodeSocketModel[] NodeSockets;

    // Saved ConnectionController as Reference for the models
    public ConnectionsController ConnectionController;

    public TypeSaveable InputType;

    public static bool IsValidFile(TextAsset ta)
    {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(ta.text);
            return doc.GetElementsByTagName(DATA_ROOT_TAG).Count > 0;
    }

    public void SaveablesToLoad(object[] saveables)
    {
        InputType = saveables.Where((s => typeof(TypeSaveable).IsAssignableFrom(s.GetType()))).Cast<TypeSaveable>().ToArray()[0];
        NodeViewData = saveables.Where((s => typeof(ViewData).IsAssignableFrom(s.GetType()))).Cast<ViewData>().ToArray();
        NodeModels = saveables.Where((s => typeof(BaseNodeModel).IsAssignableFrom(s.GetType()))).Cast<BaseNodeModel>().ToArray();
        ConnectionModels = saveables.Where((s => typeof(ConnectionModel).IsAssignableFrom(s.GetType()))).Cast<ConnectionModel>().ToArray();
        NodeSockets = saveables.Where((s => typeof(BaseNodeSocketModel).IsAssignableFrom(s.GetType()))).Cast<BaseNodeSocketModel>().ToArray();
        ConnectionController = saveables.Where((s => typeof(ConnectionsController).IsAssignableFrom(s.GetType()))).Cast<ConnectionsController>().ToArray()[0];
    }

    public ISaveable[] SaveablesToSave()
    {
        List<ISaveable> saveables = new List<ISaveable>(NodeViewData);
        saveables.Add(InputType);
        saveables.AddRange(NodeModels);
        saveables.AddRange(NodeSockets);
        saveables.AddRange(ConnectionModels);
        saveables.Add(ConnectionController);
        return saveables.ToArray();
    }
}

public class TypeSaveable : ISaveable
{
    public Type SaveableType { get; private set; }

    public TypeSaveable() { }

    public static Type GetTypeFromString(string ts)
    {
        if(ts == "")
            return null;
        
        return Type.GetType(ts);
    }

    public TypeSaveable(Type typeToSave)
    {
        SaveableType = typeToSave;
    }

    public void Load(XmlElement savedData, XmlObjectReferences references)
    {
        SaveableType = GetTypeFromString(savedData.GetSingleDataFrom("TypeName"));
    }

    public void Save(XmlDocument doc, XmlObjectReferences references, XmlElement saveableElement)
    {
        string fn = (SaveableType == null) ? "" : SaveableType.FullName;
        saveableElement.AppendChild(doc.CreateElementWithData("TypeName", fn));
    }

    public void AllDataLoaded()
    {

    }
}

