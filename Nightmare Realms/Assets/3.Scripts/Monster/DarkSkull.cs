using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkSkull : Monster
{
    public int damage;
    private float attackCooldown = 1.75f;
    private float lastAttackTime;

    protected override void Start()
    {
        detectionRange = 7.5f;
        attackRange = 1.5f;
        speed = 4f;
        lastAttackTime = -attackCooldown;
        base.Start();
    }

    protected override void Update()
    {
        sprite.flipX = playerTransform.position.x < transform.position.x;
        base.Update();
    }

    protected override void Idle()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRange, layer);
        if (hit != null)
        {
            ChangeStateTo(State.Chase);
        }
        else anim.Play("Idle");
    }

    protected override void Chase()
    {
        RaycastHit2D right = Physics2D.Raycast(new Vector3(transform.position.x, transform.position.y - 0.25f), transform.right, 0.7f, LayerMask.GetMask("Ground"));
        RaycastHit2D left = Physics2D.Raycast(new Vector3(transform.position.x, transform.position.y - 0.25f), -transform.right, 0.7f, LayerMask.GetMask("Ground"));
        RaycastHit2D groundHit = Physics2D.Raycast(new Vector3(transform.position.x, transform.position.y - 0.25f), -transform.up, 1f, LayerMask.GetMask("Ground"));

        anim.Play("Chase");

        if (right.collider == null && left.collider == null)
        {
            Debug.Log("이동!");
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            rigid.velocity = new Vector2(direction.x * speed, rigid.velocity.y);
        }
        else if (groundHit)
        {
            Debug.Log("점프!");
            rigid.AddForce(transform.up * 1f, ForceMode2D.Impulse);
        }
        Collider2D hit = Physics2D.OverlapCircle(transform.position, attackRange, layer);
        if (hit != null)
        {
            anim.Play("Idle");
            ChangeStateTo(State.Attack);
        }
        else
        {
            ChangeStateTo(State.Idle);
        }
    }

    protected override void Attack()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, attackRange, layer);

        if (hit != null)
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                anim.Play("Attack");

                if (hit != null && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f)
                {
                    hit.transform.GetComponent<Player>().TakeDamage(damage, 0.15f);
                    lastAttackTime = Time.time;
                }
            }
        }
        else
        {
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                ChangeStateTo(State.Idle);
            }
        }
    }

    protected override void Dead()
    {
        GameManager.instance.UpdateMonsterCount(1);
        GetComponent<CoinExplosion>().ExplodeCoins(35, 7.5f, 2f);
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(new Vector3(transform.position.x, transform.position.y - 0.25f), transform.right * 0.7f);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(new Vector3(transform.position.x, transform.position.y - 0.25f), -transform.right * 0.7f);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(new Vector3(transform.position.x, transform.position.y - 0.25f), -transform.up * 1f);
    }
}