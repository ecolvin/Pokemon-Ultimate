using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField] List<TKey> keys = new List<TKey>();
    [SerializeField] List<TValue> values = new List<TValue>();


    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        foreach(KeyValuePair<TKey, TValue> pair in this)
        {
            if(pair.Key != null && pair.Value != null)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }
    }

    public void OnAfterDeserialize()
    {
        this.Clear();

        if(keys.Count != values.Count)
        {
            Debug.LogError($"Tried to deserialize a SerializableDictionary, but the amount of keys ({keys.Count}) does not match the number of values ({values.Count}) which indicates that something went wrong");
        }

        for(int i = 0; i < keys.Count; i++)
        {
            this.Add(keys[i], values[i]);
        }
    }
}
