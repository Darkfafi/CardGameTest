using System.Collections.Generic;
using UnityEngine;

public class XmlObjectReferences
{
    public uint RefCounter { get { return refCounter; } private set { refCounter = value; } }
    private Dictionary<object, uint> instanceToRefCounterMap = new Dictionary<object, uint>();
    private Dictionary<object, uint> referenceWithoutCounterUseReservations = new Dictionary<object, uint>();

    private Dictionary<uint, object> setterCounterMap = new Dictionary<uint, object>();
    private Dictionary<uint, System.Action<uint, object>> waitingForSetterCounter = new Dictionary<uint, System.Action<uint, object>>();

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
    public void Loading_GetReferenceFrom(uint counterId, System.Action<uint, object> objectCreatedCallback)
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
