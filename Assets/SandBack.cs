using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandBack : Enemy
{
    public string damegedTagName;      //ダメージ判定のあるオブジェクトのタグ名    
    private TextMesh HPViewr;          //HP表示の3Dテキスト
    public EffectDamage damageEffect;   //HPViewをアニメーションさせるエフェクトクラス
    private float timeCount = 0;        //前回当たり判定があってからの時間を測るカウンタ
    private bool IsHitting = false;     

    void Start()
    {
        HPViewr = GetComponentInChildren<TextMesh>();  
        HP = MAX_HP;
        if(HPViewr != null)
        {
            Debug.Log(HPViewr.name);
        }
    }

    override protected void Update()
    {
        base.Update();              
        if(damageEffect != null)
        {
            damageEffect.gameObject.SetActive(true);
        }
        timeCount += Time.deltaTime;        //常にカウント状態

        if (IsHitting) { 
            /* 前回の当たり判定から、ダメージエフェクトが終了するまでに新たにダメージを受けたら、コンボ判定 */
            if (timeCount > damageEffect.EffectLength)
            {
                damageEffect.damage(0, false);
                IsHitting = false;
            }
        }
    }

    public override int HP
    {   
         /* HPを変化させると、3Dテキストの表示が切り替わる */
        set
        {
            base.HP = value;
            HPViewr.text = value.ToString();
        }
        get { return base.hp; }
    }

    public override void Dameged(float damage)
    {
        base.Dameged(damage);
        damageEffect.damage(damage,IsHitting);
        IsHitting = true;
        timeCount = 0;
    }

    /* 死んだら見えなくなる */
    protected override void Die()
    {
        gameObject.SetActive(false);
    }
}
