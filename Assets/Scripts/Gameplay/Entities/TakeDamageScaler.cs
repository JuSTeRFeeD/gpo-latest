using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamageScaler : MonoBehaviour
{
    private static TakeDamageScaler _instance;
    public float time = 0.5f;
    public float speed = 5f;

    public static TakeDamageScaler Instance => _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayEffect(Transform item, float scaler = 0.75f)
    {
        if (item == null) return;
        StartCoroutine(PlayScaler(item, scaler));
    }

    private IEnumerator PlayScaler(Transform item, float scaler)
    {
        var initScale = item.localScale;
        var targetScale = new Vector3(initScale.x * scaler, initScale.y * scaler, 1);

        var current = time / 2;
        while (current > 0)
        {
            if (!item) yield break;
            current -= Time.deltaTime;
            item.localScale = Vector3.Lerp (item.transform.localScale, targetScale, speed * Time.deltaTime);
            yield return null;
        }

        current = time / 2;
        while (current > 0)
        {
            if (!item) yield break;
            current -= Time.deltaTime;
            item.localScale = Vector3.Lerp (item.transform.localScale, initScale, speed * Time.deltaTime);
            yield return null;
        }
        item.transform.localScale = initScale;
    }
}
