using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoiseVisualizer : MonoBehaviour
{
    private SpriteRenderer _renderer;
    public int width = 256;
    public int height = 256;
    public float scale = 30;
    public float xOrg = 0;
    public float yOrg = 0;
    
    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        RegenerateTexture(width, height, scale, xOrg, yOrg);
    }

    public void RegenerateTexture(int _width = 256, int _height = 256, float _scale = 1, float _xOrg = 0, float _yOrg = 0)
    {
        width = _width;
        height = _height;
        scale = _scale;
        xOrg = _xOrg;
        yOrg = _yOrg;
        
        var texture = new Texture2D(width, height);
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < width; y++)
            {
                var xCoord = xOrg + (float)x / width * scale;
                var yCoord = yOrg + (float)y / height * scale;
                var sample = Mathf.PerlinNoise(xCoord, yCoord);
                texture.SetPixel( y, x, new Color(sample, sample, sample));
                texture.Apply();
            }
            
        }
        _renderer.sprite = Sprite.Create(texture, new Rect(0, 0, 256, 256),new Vector2(0.5f,0.5f),100);
    }
}
