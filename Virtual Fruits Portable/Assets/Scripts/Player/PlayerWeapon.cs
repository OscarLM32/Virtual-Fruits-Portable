using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public Action WeaponRetrieved;
    
    private const int _throwSpeed = 15;
    private const int _returnSpeed = 25;
    
    [SerializeField]private Transform _player;
    [SerializeField]private Collider2D _coll;
    private float _speed = _throwSpeed;
    private Vector2 _currentTarget;
    private bool _throw;

    private void FixedUpdate()
    {
        if (_throw)
        {
            transform.position = Vector3.MoveTowards(transform.position, _currentTarget, _speed * Time.deltaTime);
            if ((Vector2) transform.position == _currentTarget)
            {
                _throw = !_throw;
                _coll.enabled = false;
                _speed = _returnSpeed;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, _player.position, _speed * Time.deltaTime);
            if ((Vector2) transform.position == (Vector2) _player.position)
            {
                WeaponRetrieved?.Invoke();
                _speed = _throwSpeed;
                gameObject.SetActive(false);
            }
        }
        transform.Rotate(Vector3.forward, 50);
    }

    public void Throw(Vector2 target)
    {
        gameObject.SetActive(true);
        
        transform.position = _player.position;
        _currentTarget = target;
        _throw = true;
        _coll.enabled = true;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        IKillable killable = col.gameObject.GetComponent<IKillable>();
        killable?.Kill(gameObject);
        
        _throw = false;
        _coll.enabled = false;
        _speed = _returnSpeed;
    }
}
