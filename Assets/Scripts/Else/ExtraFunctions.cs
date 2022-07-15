using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtraFunctions
{
    public static Transform CreateTransformWithName(string objName)
    {
        return new GameObject(objName).transform;
    }

    public static Transform CreateGroupObject(string objName)
    {
        string s = "GROUP";
        GameData.groupObjectNames.TryGetValue(objName, out s);
        if (s.Length > 16) s = s.Substring(0, 16);
        int otherLength = 16 - s.Length;
        int startLength = Mathf.FloorToInt(otherLength / 2);
        s = " " + s + " ";

        for (int i = 0; i < startLength; i++) s = "-" + s;
        for (int i = 0; i < otherLength - startLength; i++) s = s + "-";

        return CreateTransformWithName(s);
    }

    public static Vector3 MultiplyVector(Vector3 vector, float multiplier)
    {
        return new Vector3(vector.x * multiplier, vector.y * multiplier, vector.z * multiplier);
    }

    public static GameObject GetRandomObject(this GameObject[] obj)
    {
        if (obj.Length < 1)
        {
            return default;
        }
        var index = UnityEngine.Random.Range(0, obj.Length);
        return obj[index];
    }

    public static bool Contains(this LayerMask mask, int layer)
    {
        return mask == (mask | (1 << layer));
    }

    public static bool IsInt(double d)
    {
        return d == (int)d;
    }

    public static void CheckQuitGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
    }
}
