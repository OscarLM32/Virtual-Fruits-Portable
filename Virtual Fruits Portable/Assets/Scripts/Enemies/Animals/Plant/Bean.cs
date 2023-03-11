using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Bean : MonoBehaviour
{
    public GameObject Particles;
    public int Direction = 1;

    private void OnEnable()
    {
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(8 * Direction, 0);
    }
    
    private void OnCollisionEnter2D(Collision2D col)
    {
        IKillable killableComponent = col.gameObject.GetComponent<IKillable>();
        killableComponent?.Kill(gameObject);
        InstantiateParticles();
    }
    
    private void InstantiateParticles()
    {
        var p =Instantiate(Particles, gameObject.transform.position, quaternion.identity);
        AudioManager.Instance.Play(p,SoundList.EnemyProjectileDestroy);
        EnemyProjectilePool.I.DeleteProjectile(gameObject);
    }
}
