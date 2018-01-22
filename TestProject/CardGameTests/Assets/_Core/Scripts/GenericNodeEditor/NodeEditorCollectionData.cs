using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
public class NodeEditorCollectionData
{
    public ConnectionsController Connections; // The connections of the nodes
    public ViewportRect[] DrawablesViewportData; // The positions of the nodes
    public BaseNodeModel[] NodeModels; // The data of the nodes

    public NodeEditorCollectionData() { }

    public NodeEditorCollectionData(ConnectionsController connections, BaseNodeView[] nodeViews)
    {
        ViewportRect[] rects = new ViewportRect[nodeViews.Length];
        BaseNodeModel[] models = new BaseNodeModel[nodeViews.Length]; 

        for(int i = 0; i < nodeViews.Length; i++)
        {
            rects[i] = nodeViews[i].ViewportRect;
            models[i] = nodeViews[i].NodeModel;
        }

        SetData(connections, rects, models);
    }

    private void SetData(ConnectionsController connections, ViewportRect[] viewportData, BaseNodeModel[] models)
    {
        Connections = connections;
        DrawablesViewportData = viewportData;
        NodeModels = models;
    }
}
