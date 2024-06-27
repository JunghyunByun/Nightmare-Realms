using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Slot[] slot;
    [SerializeField] private int playerDamage;
    [SerializeField] private int weaponDamage;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private LayerMask monsterLayers;
    private SpriteRenderer sprite;
    private Animator anim;
    private PlayerController playerController;
    private Player player;
    private bool isAttackCheck;
    public bool isWearingItem;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
        player = GetComponent<Player>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isAttackCheck)
        {
            playerController.isAttack = true;
            isAttackCheck = true;
            anim.Play("Attack");

            if (sprite.flipX)
            {
                attackPoint.localPosition = new Vector3(-0.75f, attackPoint.localPosition.y, attackPoint.localPosition.z);
            }
            else
            {
                attackPoint.localPosition = new Vector3(0.75f, attackPoint.localPosition.y, attackPoint.localPosition.z);
            }
            Collider2D[] hitMonsters = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, monsterLayers);

            foreach (Collider2D monster in hitMonsters)
            {
                if (monster.CompareTag("Skull"))
                {
                    StartCoroutine(monster.GetComponent<Skull>().TakeDamage(playerDamage + weaponDamage));
                }
            }
        }

        if (isWearingItem)
        {
            isWearingItem = false;
            weaponDamage = 0;
            player.maxHP = 100;

            foreach (Slot s in slot)
            {
                switch (s.itemName)
                {
                    case "DarkSoulSword":
                        weaponDamage += Resources.Load<Item>($"Prefabs/itemData/darkSoulSword").damage;
                        break;
                    case "ShadowSword":
                        weaponDamage += Resources.Load<Item>($"Prefabs/itemData/shadowSword").damage;
                        break;
                    case "BlackCrystalRing":
                        weaponDamage += Resources.Load<Item>($"Prefabs/itemData/blackCrystalRing").damage;
                        player.maxHP += Resources.Load<Item>($"Prefabs/itemData/blackCrystalRing").health;
                        break;
                }
            }
            UIManager.instance.SetPlayerHP(player.curHP, player.maxHP);
        }
        OnAttackAnimationEnd();
    }

    private void OnAttackAnimationEnd()
    {
        if (playerController.isAttack)
        {
            float animTime = anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;
            if (animTime >= 0.9f)
            {
                playerController.isAttack = false;
                isAttackCheck = false;
                anim.Play("Idle");
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}