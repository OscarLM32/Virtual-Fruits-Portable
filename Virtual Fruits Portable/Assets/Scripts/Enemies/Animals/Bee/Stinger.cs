using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Stinger : MonoBehaviour
{
    public GameObject Particles;

    void OnEnable()
    {
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -8f);
    }
    
    private void OnCollisionEnter2D(Collision2D col)
    {
        IKillable killableComponent = col.gameObject.GetComponent<IKillable>();
        killableComponent?.Kill(gameObject);
        InstantiateParticles();
    }
    
    private void InstantiateParticles()
    {
        var p = Instantiate(Particles, gameObject.transform.position, quaternion.identity);
        AudioManager.Instance.Play(p,SoundList.EnemyProjectileDestroy);
        EnemyProjectilePool.I.DeleteProjectile(gameObject);
    }
    


}
