using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class ConnectionModel : IModel, ISaveable
{
    public event Action<ConnectionModel> ConnectionDestroyedEvent;
    public BaseNodeSocketModel OutputSocket { get { return outputSocket; } private set { outputSocket = value; } }
    public BaseNodeSocketModel InputSocket { get { return inputSocket; } private set { inputSocket = value; } }

    private BaseNodeSocketModel outputSocket;
    private BaseNodeSocketModel inputSocket;

    public ConnectionModel()
    {

    }

    public ConnectionModel(BaseNodeSocketModel input, BaseNodeSocketModel output)
    {
        InputSocket = input;
        OutputSocket = output;
    }

    public void DestroyConnection()
    {
        InputSocket.ParentNode.GetAllInput();

        if (ConnectionDestroyedEvent != null)
            ConnectionDestroyedEvent(this);
    }

    public void Clean()
    {
        ConnectionDestroyedEvent = null;
        OutputSocket = null;
        InputSocket = null;
    }

    public void Save(XmlDocument doc, XmlObjectReferences references, XmlElement saveableElement)
    {
        XmlElement inputSocketElement = doc.CreateElement("InputSocketReference");
        XmlElement outputSocketElement = doc.CreateElement("OutputSocketReference");

        inputSocketElement.AppendChild(doc.CreateTextNode(references.Saving_GetRefCounterFor(inputSocket).ToString()));
        outputSocketElement.AppendChild(doc.CreateTextNode(references.Saving_GetRefCounterFor(outputSocket).ToString()));

        saveableElement.AppendChild(inputSocketElement);
        saveableElement.AppendChild(outputSocketElement);
    }

    public void Load(XmlElement savedData, XmlObjectReferences references)
    {
        uint inputReferenceId = uint.Parse(savedData.GetSingleDataFrom("InputSocketReference"));
        uint outputReferenceId = uint.Parse(savedData.GetSingleDataFrom("OutputSocketReference"));

        references.Loading_GetReferenceFrom(inputReferenceId, OnInputReferenceLoaded);
        references.Loading_GetReferenceFrom(outputReferenceId, OnOutputReferenceLoaded);
    }

    private void OnOutputReferenceLoaded(uint referenceId, object outputInstance)
    {
        outputSocket = (BaseNodeSocketModel)outputInstance;
    }

    private void OnInputReferenceLoaded(uint referenceId, object inputInstance)
    {
        inputSocket = (BaseNodeSocketModel)inputInstance;
    }

    public void AllDataLoaded()
    {

    }
}
