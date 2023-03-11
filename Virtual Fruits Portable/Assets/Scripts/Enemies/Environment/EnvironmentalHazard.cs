using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class EnvironmentalHazard : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D col)
    {
        //We only want this environmental hazards to affect the player
        if (col.gameObject.layer == (int) LayerValues.PlayerLayer)
        {
            IKillable killableComponent = col.gameObject.GetComponent<IKillable>();
            killableComponent?.Kill(gameObject);
        }
    }
}
