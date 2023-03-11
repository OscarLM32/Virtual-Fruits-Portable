using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;


public class ParallaxLayer : MonoBehaviour
{
    [Tooltip("They have to be ordered: Spring, Autumn, Winter and Summer")]
    public List<Sprite> Images = new List<Sprite>();
    
    public float ParallaxFactorX;
    public float ParallaxFactorY;

    private float _layerRepositionDistance;
    private float _layerLength;
    private float _distX = 0; //The distance that the camera has travelled in relation to the layer

    private void Start()
    {
        float cameraBounds = Camera.main.orthographicSize * Camera.main.aspect * 2;
        _layerLength = GetComponent<SpriteRenderer>().bounds.size.x;
        _layerRepositionDistance = _layerLength;
        
        if (_layerLength < cameraBounds)
        {
            float difference = Math.Abs(cameraBounds - _layerLength);
            _layerRepositionDistance -=difference / 2;
        }
    }

    public void Move(float deltaX, float deltaY)
    {
        //The - sign is necessary because the sign on "deltaX" is contrary to the movement direction
        _distX += -(deltaX * (1 - ParallaxFactorX));

        Vector2 newPos = transform.position;
        newPos.x -= deltaX * ParallaxFactorX;
        newPos.y -= deltaY * ParallaxFactorY;

        transform.position = newPos;

        if (_distX > _layerRepositionDistance)
        {
            transform.position += new Vector3(_layerLength , 0, 0);
            _distX -= _layerLength ;
        }
        else if (_distX < -_layerRepositionDistance)
        {
            transform.position -= new Vector3(_layerLength, 0, 0);
            _distX += _layerLength;
        }
    }

    public void SwitchLayerSprite(BackgroundThemes theme)
    {
        Sprite newLayerSprite = Images[(int) theme];

        GetComponent<SpriteRenderer>().sprite = newLayerSprite;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = newLayerSprite;
        }
    } 
}