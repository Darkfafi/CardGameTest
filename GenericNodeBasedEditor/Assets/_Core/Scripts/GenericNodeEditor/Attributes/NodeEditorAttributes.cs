using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NodePathAttribute : Attribute
{
    public const string DO_NOT_MENTION = "System<_>DoNotMentionPath";

    public string Path { get; private set; }

    public NodePathAttribute(string path)
    {
        Path = path;
    }
	
}
