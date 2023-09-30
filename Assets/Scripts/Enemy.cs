using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Suckable, IDamageable
{
    private Player _player;
    public float movementSpeed;
    [SerializeField] private Lifebar _lifebar;
    protected override void Initialization()
    {
        base.Initialization();
        _player = Player.instance;
        _lifebar = GetComponentInChildren<Lifebar>();
    }
    protected override void IdleState()
    {
        base.IdleState();

        if (_player != null)
        {
            //Vector3 target = PlayerBreadCrumbs.instance.GetClosestBreadCrumb(transform.position);
            Vector3 target = _player.transform.position;
            rb.AddForce((target - transform.position).normalized * movementSpeed);
            //rb.AddForce((_player.transform.position - transform.position).normalized * movementSpeed);
            if (rb.velocity.magnitude > movementSpeed)
            {
                rb.velocity = rb.velocity.normalized * movementSpeed;
            }
            //rb.velocity = (_player.transform.position - transform.position).normalized * movementSpeed;
        }
    }

    protected override void HandleCollisionWhileShot(Collision2D other)
    {
        if (other.gameObject.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            damageable.Damage(damage);
            if (other.transform.tag == "Enemy")
            {
                // Self damage
                Damage(5);
            }
        }
    }

    protected override void HandleOnTriggerStay(Collider2D other)
    {
        if (other.tag == "Player" && suckableState == SuckableState.Idle)
        {
            other.GetComponent<IDamageable>().Damage(damage);
            Debug.Log("!!");
        }
    }

    protected override void HandleDamageTaken(int amount)
    {
        DamagePopupManager.instance.CreatePopup(transform.position + Vector3.up * _yDamagePopupOffset, amount);
        _lifebar.SetLife((float)currentLife / maxLife);
    }

    protected override void HandleShoot()
    {
        gameObject.layer = LayerMask.NameToLayer("Shot");
    }

    protected override void GoToIdleState()
    {
        base.GoToIdleState();
        gameObject.layer = LayerMask.NameToLayer("Enemy");
    }

}