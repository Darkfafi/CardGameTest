using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    protected void Awake()
    {
        MasterObject mobj = new MasterObject();
        XmlSaver s = new XmlSaver();
        s.Save(mobj, "Assets/cool.xml");
    }
}
	
