using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 抽象クラスEnemy
 * 作成 : 五十里 航大
 * 機能 : HPの管理、当たり判定の管理を行う。 被ダメージ時、死亡時の抽象メソッドの定義
 * ＊*/

public abstract class Enemy : MonoBehaviour
{
    [SerializeField]public int MAX_HP;
    protected int hp;
    protected const string ATTACK_TAG_NAME = "Attack_Object";  //当たり判定となるオブジェクトのタグ名 

    /**TODO 攻撃してくるキャラの共通点(親クラス、パラメータなど)を定義し、分かりやすく書き換える */
    protected Animator oldCharaAnimator;           
    protected private float hitCount = 0f;      //前回の当たり判定からの時間
    protected AnimationClip oldHitAnimation;    //前回の当たり判定時に相手がしていた攻撃モーション
    [SerializeField]protected LayerMask groundLayer;    //地面判定となるオブジェクトのレイヤ名


    protected virtual void Start()
    {
        hp = MAX_HP;
    }


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
            {
                Die();
            }
        }
        get { return hp; }
    }

    /* ダメージを受けたときの処理。オーバーライド前提 */
    public virtual void Dameged(float damage)
    {
        HP -= (int)damage;
    }

    /* 当たり判定が起きたときに、ダメージを受けるかどうかの処理 */
    virtual protected void OnTriggerEnter(Collider other)
    {
        /* 当たり判定の相手がアニメーションコンポーネントを持っていたら攻撃可能者として認識 */
        Animator attackedAnimation = other.GetComponentInParent<Animator>();
        if (attackedAnimation == null)
        {
            Debug.Log("攻撃者のアニメーションを取得できませんでした");
            return;
        }

        if (other.tag == ATTACK_TAG_NAME || other.TryGetComponent<Animator>(out attackedAnimation))
        {
            //現在動いているアニメーションの取得
            var nowAnimation = attackedAnimation.GetCurrentAnimatorClipInfo(0)[0].clip;
            /* ・初めての当たり判定 ・前と違う種類のアニメーションによる攻撃 ・攻撃のアニメーションよりも長い時間がたってからの攻撃
             *  の3つの内どれかを満たしていればダメージを受ける
             */
            if (oldHitAnimation == null || oldHitAnimation != nowAnimation || hitCount > nowAnimation.length || attackedAnimation != oldCharaAnimator)
            {
                var attackedCharacotr = other.GetComponentInParent<PlayerController>();
                string st = attackedCharacotr.name; ///Debug 攻撃者の名前を取得
                float damage = attackedCharacotr.GetDamage(nowAnimation);
                if (damage > 0)
                {
                    Dameged(damage);
                    st += "からの攻撃" + nowAnimation.name + " ダメージ " + damage;
                    Debug.Log(st);
                }
                hitCount = 0;
            }
            oldHitAnimation = nowAnimation;
            oldCharaAnimator = attackedAnimation;
        }
    }

    abstract protected void Die();

}
