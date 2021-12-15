using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Resources/resource")]
public class Resource : ScriptableObject
{
    public ResourceData data;
}

[Serializable]
public class ResourceData
{
    public Sprite sprite;
    public string resourceName;
    public string description;
    public int count = 0;
}
    
