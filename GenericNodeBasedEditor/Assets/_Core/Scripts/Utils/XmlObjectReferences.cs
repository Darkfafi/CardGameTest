using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using System.IO;
using System;

public class XmlObjectReferences
{
    public uint RefCounter { get { return refCounter; } private set { refCounter = value; } }
    private Dictionary<object, uint> instanceToRefCounterMap = new Dictionary<object, uint>();
    private Dictionary<object, uint> referenceWithoutCounterUseReservations = new Dictionary<object, uint>();

    private Dictionary<uint, object> setterCounterMap = new Dictionary<uint, object>();
    private Dictionary<uint, Action<uint, object>> waitingForSetterCounter = new Dictionary<uint, Action<uint, object>>();

    private uint refCounter = 1;

    // Loading From XML
    /// <summary>
    /// Registers an instance of an object under the given Id to be inserted as reference
    /// </summary>
    /// <param name="objectToSetRefFor"></param>
    /// <param name="counterId"></param>
    public void Loading_SetRefCounterFor(object objectToSetRefFor, uint counterId)
    {
        if (counterId == 0) { return; }
        setterCounterMap.Add(counterId, objectToSetRefFor);
    }

    /// <summary>
    /// Registers a callback to the id to receive the object linked to it at the end of the loading process
    /// </summary>
    /// <param name="counterId"></param>
    /// <param name="objectCreatedCallback"></param>
    public void Loading_GetReferenceFrom(uint counterId, Action<uint, object> objectCreatedCallback)
    {
        if (counterId == 0) { return; }
        if(!waitingForSetterCounter.ContainsKey(counterId))
        {
            waitingForSetterCounter.Add(counterId, null);
        }

        waitingForSetterCounter[counterId] += objectCreatedCallback;
    }

    /// <summary>
    /// Ends the loading process and Inserts all references. Also checks if all references have been loaded / inserted correctly
    /// </summary>
    public void Loading_EndReferenceCounter()
    {
        foreach(KeyValuePair<uint, object> pair in setterCounterMap)
        {
            if(waitingForSetterCounter.ContainsKey(pair.Key))
            {
                if (waitingForSetterCounter[pair.Key] != null)
                    waitingForSetterCounter[pair.Key](pair.Key, pair.Value);

                waitingForSetterCounter.Remove(pair.Key);
            }
        }

        setterCounterMap.Clear();
        if(waitingForSetterCounter.Count > 0)
        {
            Debug.LogError(referenceWithoutCounterUseReservations.Count + " <- References Requested from counterIds which have not been serializeReferenced");
        }
        waitingForSetterCounter.Clear();
    }


    // Saving To XML
    /// <summary>
    /// Returns a Reference id for the given instance
    /// </summary>
    /// <param name="objectToRef"></param>
    /// <returns></returns>
    public uint Saving_GetRefCounterFor(object objectToRef)
    {
        if (objectToRef == null) { return 0; }
        if (instanceToRefCounterMap.ContainsKey(objectToRef))
        {
            return instanceToRefCounterMap[objectToRef];
        }

        if (referenceWithoutCounterUseReservations.ContainsKey(objectToRef))
        {
            return referenceWithoutCounterUseReservations[objectToRef];
        }
        else
        {
            uint key = RefCounter;
            referenceWithoutCounterUseReservations.Add(objectToRef, key);
            RefCounter++;
            return key;
        }
    }

    public bool Saving_HasRefCounterFor(object objectToRef)
    {
        if (objectToRef == null) { return false; }
        return instanceToRefCounterMap.ContainsKey(objectToRef);
    }

    /// <summary>
    /// Creates a Reference id for the given instance
    /// </summary>
    /// <param name="instance"></param>
    /// <returns></returns>
    public uint Saving_UseRefCounter(object instance)
    {
        if (instance == null) { return 0; }
        if (referenceWithoutCounterUseReservations.ContainsKey(instance))
        {
            uint refReservedCounter = referenceWithoutCounterUseReservations[instance];
            instanceToRefCounterMap.Add(instance, refReservedCounter);
            referenceWithoutCounterUseReservations.Remove(instance);
            return refReservedCounter;
        }
        else
        {
            instanceToRefCounterMap.Add(instance, RefCounter);
            return RefCounter++;
        }
    }

    /// <summary>
    /// Ends the Saving process and checks if all references have been set correctly
    /// </summary>
    public void Saving_EndReferenceCounter()
    {
        if (referenceWithoutCounterUseReservations.Count > 0)
        {
            Debug.LogError(referenceWithoutCounterUseReservations.Count + " <- References made to objects which have not been serializeReferenced");
        }

        RefCounter = 1;
        instanceToRefCounterMap.Clear();
        referenceWithoutCounterUseReservations.Clear();
    }
}

public static class XmlObjectReferencesExtensions
{
    public const string SAVEABLE_TAG = "Saveable";

    public const string REFERENCE_ID_ATTR = "ReferenceId";
    public const string OBJECT_TYPE_ATTR = "ObjectType";
    public const string CONTAINER_TYPE_ATTR = "ContainerType";

    /// <summary>
    /// Saves the returned content of the container to the given path as Xml file & Ends the Saver
    /// </summary>
    /// <param name="references"></param>
    /// <param name="container"></param>
    /// <param name="path">Folders & FileName</param>
    public static void Saving_SaveContainer(this XmlObjectReferences references, ISaveContainer container, string path, string rootTag)
    {
        XmlDocument doc = new XmlDocument();
        XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
        XmlElement root = doc.DocumentElement;
        doc.InsertBefore(dec, root);

        StreamWriter writer = new StreamWriter(path);

        ISaveable[] saveables = container.SaveablesToSave();

        XmlElement containerElement = doc.CreateElement(rootTag);
        containerElement.SetAttribute( CONTAINER_TYPE_ATTR , container.GetType().FullName);

        for (int i = 0; i < saveables.Length; i++)
        {
            XmlElement saveableElement = doc.CreateElement(SAVEABLE_TAG);
            saveableElement.SetAttribute(REFERENCE_ID_ATTR, references.Saving_UseRefCounter(saveables[i]).ToString());
            saveableElement.SetAttribute(OBJECT_TYPE_ATTR, saveables[i].GetType().FullName.ToString());
            saveables[i].Save(doc, references, saveableElement);
            containerElement.AppendChild(saveableElement);
        }

        references.Saving_EndReferenceCounter();
        doc.AppendChild(containerElement);
        doc.Save(writer.BaseStream);
        writer.Close();
        //Debug.Log("Data Saved to " + path);
    }

    /// <summary>
    /// Loads all the ISaveables from the given path xml file and loads it into the container
    /// NOTE: Only works if the xml file was saved with the 'Saving_SaveContainer' method!
    /// </summary>
    /// <param name="references"></param>
    /// <param name="containerToLoadInto"></param>
    /// <param name="path">Folders & FileName</param>
    public static void Loading_LoadContainer(this XmlObjectReferences references, ISaveContainer containerToLoadInto, string path)
    {
        XmlDocument doc = new XmlDocument();
        StreamReader reader = new StreamReader(path);
        doc.Load(reader.BaseStream);
        Loading_LoadContainerWithXml(references, containerToLoadInto, doc.DocumentElement.OuterXml);
        reader.Close();
    }
    
    /// <summary>
     /// Loads all the ISaveables from the given path xml file and loads it into the container
     /// NOTE: Only works if the xml file was saved with the 'Saving_SaveContainer' method!
     /// </summary>
     /// <param name="references"></param>
     /// <param name="containerToLoadInto"></param>
     /// <param name="path">Folders & FileName</param>
    public static void Loading_LoadContainerWithXml(this XmlObjectReferences references, ISaveContainer containerToLoadInto, string xmlData)
    {
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xmlData);
        XmlNodeList list = doc.GetElementsByTagName(SAVEABLE_TAG);
        object[] loadedSaveables = new object[list.Count];
        List<ISaveable> saveablesCreated = new List<ISaveable>();

        for (int i = 0; i < list.Count; i++)
        {
            ISaveable saveable = null;
            Type type = Type.GetType(list.Item(i).Attributes.GetNamedItem(OBJECT_TYPE_ATTR).Value);

            try
            {
                saveable = (ISaveable)Activator.CreateInstance(type);
            }
            catch (MissingMethodException)
            {
                throw new MissingMethodException("Needs Parameterless Constructor for " + type.Name);
            }

            loadedSaveables[i] = saveable;
            saveablesCreated.Add(saveable);
            XmlElement saveableXml = GetElement(list.Item(i).OuterXml);
            saveable.Load(saveableXml, references);
            references.Loading_SetRefCounterFor(saveable, uint.Parse(saveableXml.GetAttribute(REFERENCE_ID_ATTR)));
        }

        containerToLoadInto.SaveablesToLoad(loadedSaveables);
        references.Loading_EndReferenceCounter();

        for (int i = 0; i < saveablesCreated.Count; i++)
        {
            saveablesCreated[i].AllDataLoaded();
        }

        saveablesCreated.Clear();
    }

    // Extra Helpers
    public static XmlElement CreateElementWithData(this XmlDocument doc, string elementName, string value)
    {
        XmlElement e = doc.CreateElement(elementName);
        e.AppendChild(doc.CreateTextNode(value));
        return e;
    }

    public static void AppendElements(this XmlNode node, params XmlNode[] nodesToAppend)
    {
        for(int i = 0; i < nodesToAppend.Length; i++)
        {
            node.AppendChild(nodesToAppend[i]);
        }
    }

    public static string GetSingleDataFrom(this XmlElement node, string dataHoldingElementName)
    {
        return node.GetElementsByTagName(dataHoldingElementName).Item(0).InnerText;
    }

    public static string[] GetAllDataFrom(this XmlElement node, string dataHoldingElementName)
    {
        XmlNodeList list = node.GetElementsByTagName(dataHoldingElementName);
        string[] returnValue = new string[list.Count];
        for(int i = 0; i < returnValue.Length; i++)
        {
            returnValue[i] = list.Item(i).InnerText;
        }
        return returnValue;
    }

    public static XmlElement GetElement(string xml)
    {
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xml);
        return doc.DocumentElement;
    }
}

public interface ISaveContainer
{
    void SaveablesToLoad(object[] saveables);
    ISaveable[] SaveablesToSave();
}

public interface ISaveable
{
    void Save(XmlDocument doc, XmlObjectReferences references, XmlElement saveableElement);
    void Load(XmlElement savedData, XmlObjectReferences references);
    void AllDataLoaded();
}