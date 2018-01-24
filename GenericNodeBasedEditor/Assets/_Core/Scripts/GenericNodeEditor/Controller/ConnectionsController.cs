using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Xml.Serialization;
using System.Xml.Linq;
using System;
using System.Xml;

// (input) <[NODE]> (output)

public class ConnectionsController : ISaveable
{
    public const int INFINITE_CONNECTIONS_AMOUNT = -101;

    // Constants
    public const int NO_VALID_CONNECTION = -1337;
    public const int FLOW_CONNECTION = 1;
    public const int DATA_CONNECTION = 2;

    // Public Data
    public ConnectionModel[] AllConnections { get { return allConnections.ToArray(); } }

    private List<ConnectionModel> allConnections = new List<ConnectionModel>();

    // My output gives data to these inputs <output, input[]>
    private Dictionary<BaseNodeSocketModel, List<BaseNodeSocketModel>> outputToInputsMap = new Dictionary<BaseNodeSocketModel, List<BaseNodeSocketModel>>();

    // My input has input from output <input, output[]>
    private Dictionary<BaseNodeSocketModel, List<BaseNodeSocketModel>> inputFromOutputsMap = new Dictionary<BaseNodeSocketModel, List<BaseNodeSocketModel>>(); 


    // ------------- Handling With Connections

    public BaseNodeSocketModel[] GetOutputsConnectedToInput(BaseNodeSocketModel input)
    {
        if (!inputFromOutputsMap.ContainsKey(input)) { return new BaseNodeSocketModel[] { }; }
        return inputFromOutputsMap[input].ToArray();
    }

    public BaseNodeSocketModel[] GetInputsConnectedToOutput(BaseNodeSocketModel output)
    {
        if (!outputToInputsMap.ContainsKey(output)) { return new BaseNodeSocketModel[] { }; }
        return outputToInputsMap[output].ToArray();
    }

    // ------------- Adding Connections

    public ConnectionModel Connect(BaseNodeSocketModel output, BaseNodeSocketModel input, out ConnectionInfo connectionInfo)
    {
        connectionInfo = GetConnectionType(output, input);

        if (connectionInfo.ConnectionType != NO_VALID_CONNECTION)
        {
            if (!outputToInputsMap.ContainsKey(output))
            {
                outputToInputsMap.Add(output, new List<BaseNodeSocketModel>());
            }

            if (!inputFromOutputsMap.ContainsKey(input))
            {
                inputFromOutputsMap.Add(input, new List<BaseNodeSocketModel>()); // Can only have output connect to input if no other is connected
            }

            outputToInputsMap[output].Add(input);
            inputFromOutputsMap[input].Add(output);

            input.ParentNode.GetAllInput();
            output.ParentNode.SendAllOutput();

            ConnectionModel cm = new ConnectionModel(input, output);
            cm.ConnectionDestroyedEvent += OnConnectionDestroyedEvent;
            allConnections.Add(cm);

            return cm;
        }
        Debug.LogWarning("Could not connect: " + connectionInfo.ConnectionMessage);
        return null;
    }

    // ------------- Removing Connections

    public void RemoveAllConnectionsForNode(BaseNodeModel node)
    {
        List<BaseNodeSocketModel> inputsToRemove = new List<BaseNodeSocketModel>();
        List<BaseNodeSocketModel> outputsToRemove = new List<BaseNodeSocketModel>();

        foreach(KeyValuePair<BaseNodeSocketModel, List<BaseNodeSocketModel>> pair in outputToInputsMap)
        {
            if(pair.Key.ParentNode == node)
            {
                outputsToRemove.Add(pair.Key);
            }
        }

        foreach (KeyValuePair<BaseNodeSocketModel, List<BaseNodeSocketModel>> pair in inputFromOutputsMap)
        {
            if (pair.Key.ParentNode == node)
            {
                inputsToRemove.Add(pair.Key);
            }
        }

        for(int i = 0; i < inputsToRemove.Count; i++)
        {
            RemoveAllInputConnections(inputsToRemove[i]);
        }

        for(int i = 0; i < outputsToRemove.Count; i++)
        {
            RemoveAllOutputConnections(outputsToRemove[i]);
        }
    }

    public void RemoveConnecton(BaseNodeSocketModel output, BaseNodeSocketModel input)
    {
        if(outputToInputsMap.ContainsKey(output))
        {
            outputToInputsMap[output].Remove(input);

            if(outputToInputsMap[output].Count == 0)
            {
                outputToInputsMap.Remove(output);
            }
        }

        if(inputFromOutputsMap.ContainsKey(input))
        {
            inputFromOutputsMap.Remove(input);
        }

        ConnectionModel cm = allConnections.Where(c => (c.InputSocket == input && c.OutputSocket == output)).FirstOrDefault();
        if(cm != null)
        {
            cm.ConnectionDestroyedEvent -= OnConnectionDestroyedEvent;
            allConnections.Remove(cm);
            cm.DestroyConnection();
        }
    }

    public int ConnectionsAmountNodeSocket(BaseNodeSocketModel socket)
    {
        BaseNodeSocketModel[] models = (socket.SocketModelType == SocketModelType.Input) ? inputFromOutputsMap[socket].ToArray() : outputToInputsMap[socket].ToArray();
        return (models == null) ? 0 : models.Length;
    }

    private void OnConnectionDestroyedEvent(ConnectionModel connectionToDestroy)
    {
        RemoveConnecton(connectionToDestroy.OutputSocket, connectionToDestroy.InputSocket);
    }

    public void RemoveAllOutputConnections(BaseNodeSocketModel output)
    {
        if (outputToInputsMap.ContainsKey(output))
        {
            BaseNodeSocketModel[] inputs = outputToInputsMap[output].ToArray();
            for(int i = 0; i < inputs.Length; i++)
            {
                RemoveConnecton(output, inputs[i]);
            }
        }
    }

    public void RemoveAllInputConnections(BaseNodeSocketModel input)
    {
        if (inputFromOutputsMap.ContainsKey(input))
        {
            BaseNodeSocketModel[] outputs = inputFromOutputsMap[input].ToArray();
            for (int i = 0; i < outputs.Length; i++)
            {
                RemoveConnecton(outputs[i], input);
            }
        }
    }

    private ConnectionInfo GetConnectionType(BaseNodeSocketModel output, BaseNodeSocketModel input)
    {
        int connectionType = 0;
        string connectionMessage = "";

        if(output.SocketDataType != input.SocketDataType)
        {
            connectionMessage = "Cannot connect to different Socket Type";
        }

        if (output.ParentNode == input.ParentNode)
        {
            connectionMessage = "Cannot connect to Socket of same node";
        }

        if (output.SocketModelType != SocketModelType.Output)
        {
            connectionMessage = "Output socket has to be of type Output";
        }

        if (input.SocketModelType != SocketModelType.Input)
        {
            connectionMessage = "Input socket has to be of type Input";
        }

        int outputsConnectedToInput = GetOutputsConnectedToInput(input).Length;
        int inputsConnectedToOutputs = GetInputsConnectedToOutput(output).Length;

        if (outputsConnectedToInput >= input.MaxConnectionsAmount && input.MaxConnectionsAmount != INFINITE_CONNECTIONS_AMOUNT)
        {
            connectionMessage = "Outputs connected to Input at max of " + input.MaxConnectionsAmount;
        }

        if (inputsConnectedToOutputs >= output.MaxConnectionsAmount && output.MaxConnectionsAmount != INFINITE_CONNECTIONS_AMOUNT)
        {
            connectionMessage = "Output connected to max inputs of " + output.MaxConnectionsAmount;
        }

        if (input.SocketStreamingType != null && output.SocketStreamingType != null)
            if (!input.SocketStreamingType.IsAssignableFrom(output.SocketStreamingType))
            {
                connectionMessage = "Input type must be assignable from output type";
            }

        if (connectionMessage != "")
        {
            connectionType = NO_VALID_CONNECTION;
        }
        else
        {
            switch (output.SocketDataType)
            {
                case SocketDataType.DataConnection:
                    connectionType = DATA_CONNECTION;
                    connectionMessage = "Connection valid to Data Socket";
                    break;
                case SocketDataType.FlowConnection:
                    connectionType = FLOW_CONNECTION;
                    connectionMessage = "Connection valid to Flow Socket";
                    break;
            }
        }
        return new ConnectionInfo(connectionType, connectionMessage); // No valid connection could be found
    }

    private ConnectionInfo ConnectConnectionPrototype(ConnectionModel connection)
    {
        ConnectionInfo ci;
        Connect(connection.OutputSocket, connection.InputSocket, out ci);
        return ci;
    }

    public void Save(XmlDocument doc, XmlObjectReferences references, XmlElement saveableElement)
    {
        for(int i = 0; i < allConnections.Count; i++)
        {
            saveableElement.AppendChild(doc.CreateElementWithData("ConnectionModelReference", references.Saving_GetRefCounterFor(allConnections[i]).ToString()));
        }
    }

    public void Load(XmlElement savedData, XmlObjectReferences references)
    {
        string[] referenceIdsStrings = savedData.GetAllDataFrom("ConnectionModelReference");
        for(int i = 0; i < referenceIdsStrings.Length; i++)
        {
            uint refId = uint.Parse(referenceIdsStrings[i]);
            references.Loading_GetReferenceFrom(refId, OnConnectionModelLoaded);
        }
    }

    private void OnConnectionModelLoaded(uint referenceId, object connectionModelInstance)
    {
        ConnectConnectionPrototype(((ConnectionModel)connectionModelInstance));
    }

    public void AllDataLoaded()
    {

    }
}

public struct ConnectionInfo
{
    public int ConnectionType { get; private set; }
    public string ConnectionMessage { get; private set; }

    public ConnectionInfo(int connectionType, string connectionMessage)
    {
        ConnectionType = connectionType;
        ConnectionMessage = connectionMessage;
    }

    public override string ToString()
    {
        return "ConnectionType: " + ConnectionType + " | ConnectionMessage: " + ConnectionMessage;
    }
}

