using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Reflection;
using System;
using System.Linq;
using System.IO;

public class XmlSaver
{
    XmlObjectReferences references = new XmlObjectReferences();

    public void Save(object classToSave, string path)
    {
        XmlDocument doc = new XmlDocument();
        XmlDeclaration declaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
        XmlElement root = doc.DocumentElement;
        doc.InsertBefore(declaration, root);

        // Writer
        StreamWriter writer = new StreamWriter(path);

        // Data to Work with
        object[] itemsToSave;
        Dictionary<XmlElement, List<object>> nextSaveablesMap = new Dictionary<XmlElement, List<object>>(); //  new List<object>(InternalSave(classToSave, doc, root));


        // Save the given class
        XmlElement parent;
        object[] parentObjects = InternalSave(classToSave, doc, doc, out parent);
        nextSaveablesMap.Add(parent, new List<object>(parentObjects));

        while (nextSaveablesMap.Count > 0)
        {
            Dictionary<XmlElement, List<object>> newItemsMap = new Dictionary<XmlElement, List<object>>();

            foreach (KeyValuePair<XmlElement, List<object>> pair in nextSaveablesMap)
            {
                itemsToSave = pair.Value.ToArray();
                for (int i = 0; i < itemsToSave.Length; i++)
                {
                    XmlElement element;
                    object[] objectsToSaveNext = InternalSave(itemsToSave[i], doc, parent, out element);
                    if(element != null)
                        newItemsMap.Add(element, new List<object>(objectsToSaveNext));
                }
            }

            nextSaveablesMap.Clear();
            nextSaveablesMap = newItemsMap;
        }

        references.Saving_EndReferenceCounter();

        doc.Save(writer.BaseStream);
        writer.Close();
    }

    /*
    public object Load(string location)
    {

    }
    */

    private object[] InternalSave(object objToSave, XmlDocument doc, XmlNode parent, out XmlElement currentElement)
    {
        if (references.Saving_HasRefCounterFor(objToSave)) { currentElement = null; return new object[] { }; }
        List<object> objectsToSave = new List<object>();
        Debug.Log("Saving: " + objToSave.GetType().Name);
        Debug.Log(objToSave.GetType().IsArray);
        FieldInfo[] f_infos = objToSave.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).Where((f => f.IsDefined(typeof(XmlSaveableVariableAttribute), true))).ToArray();
        PropertyInfo[] p_infos = objToSave.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).Where((f => f.IsDefined(typeof(XmlSaveableVariableAttribute), true))).ToArray();

        string elementName;
        elementName = GetMasterName(objToSave, objToSave.GetType().Name);
        currentElement = doc.CreateElement(elementName);
        currentElement.SetAttribute("ReferenceId", references.Saving_UseRefCounter(objToSave).ToString());
        currentElement.SetAttribute("ReferenceType", objToSave.GetType().FullName);


        for (int i = 0; i < f_infos.Length; i++)
        {
            Debug.Log("Field: " + f_infos[i].Name);
            object obj = f_infos[i].GetValue(objToSave);
            elementName = GetMasterName(obj, f_infos[i].Name);
            CreateSavingElementFor(elementName, obj, doc, currentElement);
            if (obj != null)
                objectsToSave.Add(obj);
        }

        for (int i = 0; i < p_infos.Length; i++)
        {
            Debug.Log("Property: " + p_infos[i].Name);
            object obj = p_infos[i].GetValue(objToSave, null);
            elementName = GetMasterName(obj, p_infos[i].Name);
            CreateSavingElementFor(elementName, obj, doc, currentElement);
            if (obj != null)
                objectsToSave.Add(obj);
        }

        parent.AppendChild(currentElement);

        return objectsToSave.ToArray();
    }

    private void CreateSavingElementFor(string name, object valueInside, XmlDocument document, XmlElement parent)
    {
        XmlElement elementCreating;
        elementCreating = document.CreateElement(name);
        parent.AppendChild(elementCreating);
        if (valueInside != null)
        {
            bool isRef = false;
            string text = valueInside.ToString();
            if (valueInside.GetType().IsClass)
            {
                text = references.Saving_GetRefCounterFor(valueInside).ToString();
                isRef = true;
            }
            elementCreating.SetAttribute("IsReference", isRef.ToString());
            elementCreating.AppendChild(document.CreateTextNode(text));
        }
    }

    private string GetMasterName(object objectToGetNameFor, string defaultName)
    {
        if (objectToGetNameFor == null) { return defaultName; }
        Type t = objectToGetNameFor.GetType();
        object[] nca = t.GetCustomAttributes(typeof(XmlSaveableNameChangeAttribute), true);
        string name = t.Name;
        if(t.IsGenericType)
            name = name.Remove(name.IndexOf('`'));

        if (nca.Length > 0)
        {
            XmlSaveableNameChangeAttribute ncaInstance = (XmlSaveableNameChangeAttribute)nca[0];
            name = ncaInstance.MasterName;
        }

        return name;
    }

    private string GetItemName(object masterObject, object item, string defaultName)
    {
        if (masterObject == null) { return defaultName; }
        Type t = masterObject.GetType();
        object[] nca = t.GetCustomAttributes(typeof(XmlSaveableNameChangeAttribute), true);
        string name = item.GetType().Name;
        name.Remove(name.IndexOf('`'));
        if (nca.Length > 0)
        {
            XmlSaveableNameChangeAttribute ncaInstance = (XmlSaveableNameChangeAttribute)nca[0];
            name = ncaInstance.SubName;
        }
        return name;
    }
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class XmlSaveableVariableAttribute : Attribute { }

public class XmlSaveableNameChangeAttribute : Attribute
{
    public string MasterName { get; private set; }
    public string SubName { get; private set; }

    public XmlSaveableNameChangeAttribute(string XmlName)
    {
        MasterName = XmlName;
        SubName = null;
    }

    public XmlSaveableNameChangeAttribute(string XmlName, string itemsName)
    {
        MasterName = XmlName;
        SubName = itemsName;
    }
}
