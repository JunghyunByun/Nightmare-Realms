using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    [SerializeField] private BossPattern1 pattern1;
    [SerializeField] private BossPattern2 pattern2;
    [SerializeField] private BossPattern3 pattern3;
    [SerializeField] private BossPattern4 pattern4;
    [SerializeField] private MonsterData monsterData;
    [SerializeField] private Transform player;
    private SpriteRenderer sprite;
    private Animator anim;
    public int curHP;
    public int maxHP;
    private float time = 10f;

    private void Start()
    {
        curHP = monsterData.curHP;
        maxHP = monsterData.maxHP;

        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        StartCoroutine(Patterns());
    }

    IEnumerator Patterns()
    {
        while (true)
        {
            if (curHP <= (maxHP / 2))
            {
                Color color = Color.red;
                sprite.color = color;
                time = 5f;
            }
            else
            {
                time = 10f;
            }

            yield return new WaitForSeconds(time);
            anim.Play("Idle");
            int index = Random.Range(0, 4);
            switch (index)
            {
                case 0:
                    pattern1.StartCoroutine(pattern1.PatternRoutine());
                    break;
                case 1:
                    pattern2.Pattern(player.position);
                    break;
                case 2:
                    pattern3.Pattern(player.position);
                    break;
                case 3:
                    pattern4.Pattern();
                    break;
            }
        }
    }

    public IEnumerator TakeDamage(int damage)
    {
        AudioClip clip = SoundManager.instance.swordHit;
        SoundManager.instance.PlaySFX_2(clip);

        anim.Play("TakeDamage");

        curHP -= damage;
        SetHPSliderValue();

        Color color = Color.red;
        sprite.color = color;
        yield return new WaitForSecondsRealtime(0.35f);
        color = new Color(150 / 255f, 150 / 255f, 150 / 255f);
        sprite.color = color;

        anim.Play("Idle");

        if (curHP <= 0)
        {
            Dead();
        }
    }

    private void SetHPSliderValue()
    {
        UIManager.instance.bossHPSlider.value = (float)curHP / maxHP;
    }

    private void Dead()
    {
        
    }
}