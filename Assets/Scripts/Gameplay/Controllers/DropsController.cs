using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class DropsController : MonoBehaviour
{
    [SerializeField] private GameObject dropPrefab;
    private List<GameObject> dropList = new List<GameObject>();

    private void Start()
    {
        for (var i = 0; i < 10; i++)
        {
            dropList.Add(CreateDrop());
        }
    }

    private GameObject CreateDrop()
    {
        var obj = Instantiate(dropPrefab, Vector3.zero, quaternion.identity, transform);
        obj.SetActive(false);
        return obj;
    }

    public void DropItem(ResourceData data, Vector2 position)
    {
        foreach (var dropItem in from item in dropList where !item.activeSelf select item.GetComponent<DropItem>())
        {
            dropItem.gameObject.SetActive(true);
            dropItem.SetParams(data);
            dropItem.transform.position = position + Random.insideUnitCircle;
            return;
        }

        var newDropItem = CreateDrop().GetComponent<DropItem>();
        newDropItem.gameObject.SetActive(true);
        newDropItem.SetParams(data);
        newDropItem.transform.position = position + Random.insideUnitCircle;
        dropList.Add(newDropItem.gameObject);
    }
}
