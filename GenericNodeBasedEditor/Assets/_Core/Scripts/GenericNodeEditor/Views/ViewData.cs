using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class ViewData : ISaveable
{
    public string NodeViewType { get; private set; }
    public BaseNodeModel NodeModel { get; private set; }
    public ViewportRect ViewportRect { get; private set; }
    public bool IsRemoveable { get; private set; }
    public Vector2 LoadedPosition { get; private set; }

    public ViewData() { }

    public ViewData(Type parentType, BaseNodeModel model, ViewportRect viewportRect, bool isRemoveable)
    {
        NodeViewType = parentType.FullName;
        NodeModel = model;
        ViewportRect = viewportRect;
        LoadedPosition = viewportRect.Rect.position;
        IsRemoveable = isRemoveable;
    }

    public void SetScene(IOriginScene scene, float width, float height)
    {
        float x = LoadedPosition.x;
        float y = LoadedPosition.y;
        ViewportRect = new ViewportRect(new Rect(x, y, width, height), scene);
    }

    public void Save(XmlDocument doc, XmlObjectReferences references, XmlElement saveableElement)
    {
        saveableElement.AppendElements(
            doc.CreateElementWithData("NodeViewTypeString", NodeViewType),
            doc.CreateElementWithData("RepresentingModel", references.Saving_GetRefCounterFor(NodeModel).ToString()),
            doc.CreateElementWithData("X", ViewportRect.Rect.position.x.ToString()),
            doc.CreateElementWithData("Y", ViewportRect.Rect.position.y.ToString()),
            doc.CreateElementWithData("IsRemoveable", IsRemoveable.ToString())
        );
    }

    public void Load(XmlElement savedData, XmlObjectReferences references)
    {
        NodeViewType = savedData.GetSingleDataFrom("NodeViewTypeString");
        IsRemoveable = bool.Parse(savedData.GetSingleDataFrom("IsRemoveable"));

        LoadedPosition = new Vector2(float.Parse(savedData.GetSingleDataFrom("X")), float.Parse(savedData.GetSingleDataFrom("Y")));

        if (ViewportRect != null)
            ViewportRect.Rect.position = LoadedPosition;

        uint nodeModelReference = uint.Parse(savedData.GetSingleDataFrom("RepresentingModel"));
        references.Loading_GetReferenceFrom(nodeModelReference, OnNodeModelLoaded);
    }

    public void AllDataLoaded()
    {

    }
    private void OnNodeModelLoaded(uint referenceId, object nodeModelInstance)
    {
        NodeModel = (BaseNodeModel)nodeModelInstance;
    }
}
