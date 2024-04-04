using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class DontDestroy : MonoBehaviour
{
    public string dontDestroyTag;

    private static Dictionary<string, GameObject> dontDestroyObjects = new Dictionary<string, GameObject>();

    void Start()
    {
        if (CheckObjectExists(dontDestroyTag))
        {
            Destroy(gameObject);
        }
        else
        {
            AddObjectToDictionary(dontDestroyTag, gameObject);
        }
    }

    private void AddObjectToDictionary(string tag, GameObject obj)
    {
        if (!dontDestroyObjects.ContainsKey(tag))
        {
            dontDestroyObjects[tag] = obj;
            DontDestroyOnLoad(obj);
        }
    }

    private bool CheckObjectExists(string tag)
    {
        return dontDestroyObjects.ContainsKey(tag);
    }

}