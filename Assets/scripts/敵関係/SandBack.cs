using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandBack : Enemy
{
    public string damegedTagName;      //ダメージ判定のあるオブジェクトのタグ名    
    private TextMesh HPViewr;          //HP表示の3Dテキスト
    public EffectDamage damageEffect;  //HPViewをアニメーションさせるエフェクトクラス

    override protected void Start()
    {
        HPViewr = GetComponentInChildren<TextMesh>();
        HP = MAX_HP;
        if (damageEffect != null)
        {
            damageEffect.gameObject.SetActive(true);
        }
    }

    override protected void Update()
    {
        base.Update();              
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
        damageEffect.damage(damage);
    }

    /* 死んだら見えなくなる */
    protected override void Die()
    {
        gameObject.SetActive(false);
    }
}
