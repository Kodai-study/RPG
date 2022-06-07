using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandBack : Enemy
{
    public string damegedTagName;
    private TextMesh HPViewr;
    public EffectDamage damageEffect;
    private float timeCount = 0;
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
        timeCount += Time.deltaTime;
        if (IsHitting) { 
            if (timeCount > damageEffect.EffectLength)
            {
                damageEffect.damage(0, false);
                IsHitting = false;
            }
        }
    }


    public override int HP
    {
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

    protected override void Die()
    {
        gameObject.SetActive(false);
    }
}
