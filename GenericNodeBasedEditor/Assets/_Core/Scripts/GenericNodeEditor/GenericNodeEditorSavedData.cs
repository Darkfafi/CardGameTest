using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GenericNodeEditorSavedData : ISaveContainer
{
    // Saved Connections
    // -- All Nodes connection data should be stored here
    public ConnectionModel[] ConnectionModels;

    // Saved Views
    // -- All Node visual data should be stored here
    public BaseNodeView[] NodeViews;

    // Saved Models
    // -- All Node Logic holders should be stored here
    public BaseNodeModel[] NodeModels;

    // Saved Socket Models
    public BaseNodeSocketModel[] NodeSockets;

    // Saved ConnectionController as Reference for the models
    public ConnectionsController ConnectionController;

    public void SaveablesToLoad(object[] saveables)
    {
        NodeViews = saveables.Where((s => typeof(BaseNodeView).IsAssignableFrom(s.GetType()))).Cast<BaseNodeView>().ToArray();
        NodeModels = saveables.Where((s => typeof(BaseNodeModel).IsAssignableFrom(s.GetType()))).Cast<BaseNodeModel>().ToArray();
        ConnectionModels = saveables.Where((s => typeof(ConnectionModel).IsAssignableFrom(s.GetType()))).Cast<ConnectionModel>().ToArray();
        NodeSockets = saveables.Where((s => typeof(BaseNodeSocketModel).IsAssignableFrom(s.GetType()))).Cast<BaseNodeSocketModel>().ToArray();
        ConnectionController = saveables.Where((s => typeof(ConnectionsController).IsAssignableFrom(s.GetType()))).Cast<ConnectionsController>().ToArray()[0];
    }

    public ISaveable[] SaveablesToSave()
    {
        List<ISaveable> saveables = new List<ISaveable>(NodeViews);
        saveables.AddRange(NodeModels);
        saveables.AddRange(NodeSockets);
        saveables.AddRange(ConnectionModels);
        saveables.Add(ConnectionController);
        return saveables.ToArray();
    }
}
