using System.Collections.Generic;
using UnityEngine;

public enum BackgroundThemes{
    Spring = 0,
    Autumn = 1,
    Winter = 2,
    Summer = 3
}


public class ParallaxBackground : MonoBehaviour
{
    public Vector2 CameraPos;
    private Vector2 _oldCameraPos;
    private List<ParallaxLayer> _parallaxLayers = new List<ParallaxLayer>();

    private float _deltaX, _deltaY;
    private void Start()
    {
        CameraPos = Camera.main.transform.position;
        _oldCameraPos = CameraPos;
        SetLayers();
        InitPosition();
    }

    private void FixedUpdate()
    {
        CameraPos = Camera.main.transform.position;
        if (CameraPos != _oldCameraPos)
        {
            _deltaX = _oldCameraPos.x - CameraPos.x;
            _deltaY = _oldCameraPos.y - CameraPos.y;
            Move(_deltaX, _deltaY);
            _oldCameraPos = CameraPos;
        }
    }

    private void InitPosition()
      {
          //Not necessary, but gives some safety making sure that the player always spawns in front of the background
          _deltaX = transform.position.x - CameraPos.x;
          _deltaY = transform.position.y - CameraPos.y;
          //If the background is positioned higher than the character it should be perfectly aligned
          if (_deltaY > 0) {_deltaY = 0;}
          Move(_deltaX, _deltaY);
      }
    private void SetLayers()
    {
        _parallaxLayers.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            ParallaxLayer layer = transform.GetChild(i).GetComponent<ParallaxLayer>();
  
            if (layer != null)
            {
                layer.name = "Layer-" + i;
                _parallaxLayers.Add(layer);
            }
        }
    }

    private void Move(float deltaX, float deltaY) 
    {
        foreach (ParallaxLayer layer in _parallaxLayers)
        {
            layer.Move(deltaX, deltaY);
        }
    }

    private void SwitchBackground(BackgroundThemes theme)
    {
        foreach (var layer in _parallaxLayers)
        {
            layer.SwitchLayerSprite(theme);
        }
    }

    private void OnEnable()
    {
        BackgroundSwitchTrigger.BackgroundSwitchTriggered += SwitchBackground;
    }

    private void OnDisable()
    {
        BackgroundSwitchTrigger.BackgroundSwitchTriggered -= SwitchBackground;
    }
}
