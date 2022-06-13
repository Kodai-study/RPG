using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField]public int MAX_HP;
    protected int hp;
    protected const string ATTACK_TAG_NAME = "Attack_Object";
    protected Animator oldCharaAnimator;
    protected private float hitCount = 0f;
    protected AnimationClip oldHitAnimation;
    [SerializeField]protected LayerMask groundLayer;


    // Start is called before the first frame update
    void Start()
    {
        hp = MAX_HP;
    }


    // Update is called once per frame
    protected virtual void Update()
    {
        hitCount += Time.deltaTime;
    }

    public virtual int HP
    {
        set
        {
            hp = value;
            if (HP < 0)
                Die();
        }
        get { return hp; }
    }

    public virtual void Dameged(float damage)
    {
        HP -= (int)damage;
    }

    virtual protected void OnTriggerEnter(Collider other)
    {
        Animator attackedAnimation = other.GetComponentInParent<Animator>();//a
        if (attackedAnimation == null)
        {
            Debug.Log("攻撃者のアニメーションを取得できませんでした");
        }

        if (other.tag == ATTACK_TAG_NAME || other.TryGetComponent<Animator>(out attackedAnimation))
        {
            var nowAnimation = attackedAnimation.GetCurrentAnimatorClipInfo(0)[0].clip;
            if (oldHitAnimation == null || oldHitAnimation != nowAnimation || hitCount > nowAnimation.length || attackedAnimation != oldCharaAnimator)
            {
                var attackedCharacotr = other.GetComponentInParent<PlayerController>();
                string st = attackedCharacotr.name;
                float damage = attackedCharacotr.GetDamage(nowAnimation);
                if (damage > 0)
                {
                    Dameged(damage);

                    st += "からの攻撃" + nowAnimation.name + " ダメージ " + damage;
                    Debug.Log(st);
                }
                else
                {
                    Debug.Log("nocode");
                }
                hitCount = 0;
            }
            oldHitAnimation = nowAnimation;
            oldCharaAnimator = attackedAnimation;
        }
    }

    abstract protected void Die();


}
