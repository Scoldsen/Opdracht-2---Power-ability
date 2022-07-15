using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ResourceLoader
{
    public static Object[] loadResources(string path, System.Type type)
    {
        Object[] objs = Resources.LoadAll(path, type);
        return objs;
    }
}
