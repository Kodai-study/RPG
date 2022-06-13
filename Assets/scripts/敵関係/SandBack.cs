using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandBack : Enemy
{
    public string damegedTagName;      //�_���[�W����̂���I�u�W�F�N�g�̃^�O��    
    private TextMesh HPViewr;          //HP�\����3D�e�L�X�g
    public EffectDamage damageEffect;  //HPView���A�j���[�V����������G�t�F�N�g�N���X

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
         /* HP��ω�������ƁA3D�e�L�X�g�̕\�����؂�ւ�� */
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

    /* ���񂾂猩���Ȃ��Ȃ� */
    protected override void Die()
    {
        gameObject.SetActive(false);
    }
}
