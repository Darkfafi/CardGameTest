using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;



public class IntegerNodeModel : BaseNodeModel
{
    public int Value { get; private set; }
    private OutputSocketModel<int> output;

    public IntegerNodeModel() : base() { }

    public IntegerNodeModel(ConnectionsController connectionsController) : base(connectionsController)
    {

    }

    public void SetValue(int value)
    {
        if (Value == value) { return; }
        Value = value;
        output.SendOutputData(connectionsController);
    }

    protected override void ConstructSockets()
    {
        output = AddOutputSocket<int>(OnRequestData, ConnectionsController.INFINITE_CONNECTIONS_AMOUNT);
    }

    private int OnRequestData()
    {
        return Value;
    }

    protected override void DataReceivedFromInputSocket(object data)
    {
        output.SendOutputData(connectionsController);
    }

    protected override void SpecificSave(XmlDocument doc, XmlObjectReferences references, XmlElement saveableElement)
    {
        XmlElement valueElement = doc.CreateElement("Value");
        valueElement.AppendChild(doc.CreateTextNode(Value.ToString()));
        saveableElement.AppendChild(valueElement);
    }

    protected override void SpecificLoad(XmlElement savedData, XmlObjectReferences references)
    {
        int value = int.Parse(savedData.GetSingleDataFrom("Value"));
        Value = value;
    }

    protected override void OnAllDataLoaded()
    {
        output = GetLoadedOutputSocket<int>(0, OnRequestData);
        SetValue(Value);
    }
}
