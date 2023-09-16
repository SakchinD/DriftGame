using System;
using Zenject;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum PoolType
{
    Room,
    Player
}

public class ObjectPoolController : MonoBehaviour
{
    [Serializable]
    public struct KeyValue
    {
        public PoolType ObjectType;
        public PoolItem Object;
    }
    [SerializeField] private List<KeyValue> _objects = new();

    private Dictionary<PoolType, List<PoolItem>> _poolsDictionary = new();

    public PoolItem GetPooledObject(PoolType ObjectType)
    {
        if (_poolsDictionary.TryGetValue(ObjectType, out var objects)
            && objects.Any(x => !x.gameObject.activeInHierarchy))
        {
            return objects.First(x => !x.gameObject.activeInHierarchy);
        }

        return CreateObject(ObjectType);
    }

    private PoolItem CreateObject(PoolType objectType)
    {
        var obj = _objects
            .FirstOrDefault(item => item.ObjectType == objectType);

        if (obj.Object)
        {
            if (!_poolsDictionary.ContainsKey(objectType))
            {
                _poolsDictionary.Add(objectType, new List<PoolItem>());
            }
            var pooledObject = Instantiate(obj.Object);
            pooledObject.transform.SetParent(transform);
            pooledObject.gameObject.SetActive(false);
            _poolsDictionary[objectType].Add(pooledObject);
            return pooledObject;
        }
        
        Debug.LogError($"Don't find object with name: {objectType}");
        return null;
    }
}