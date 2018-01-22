using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterObject
{
    [XmlSaveableVariable]
    public List<SubObject> SubObjectField = new List<SubObject>();

    public MasterObject()
    {
        for(int i = 0; i < 10; i++)
        {
            SubObjectField.Add(new SubObject());

            if(i > 5)
                SubObjectField[i].SubObjectInstance = SubObjectField[i - 1];
        }
    }
}
