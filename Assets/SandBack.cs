using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandBack : Enemy
{
    public string damegedTagName;      //�_���[�W����̂���I�u�W�F�N�g�̃^�O��    
    private TextMesh HPViewr;          //HP�\����3D�e�L�X�g
    public EffectDamage damageEffect;   //HPView���A�j���[�V����������G�t�F�N�g�N���X
    private float timeCount = 0;        //�O�񓖂��蔻�肪�����Ă���̎��Ԃ𑪂�J�E���^
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
        timeCount += Time.deltaTime;        //��ɃJ�E���g���

        if (IsHitting) { 
            /* �O��̓����蔻�肩��A�_���[�W�G�t�F�N�g���I������܂łɐV���Ƀ_���[�W���󂯂���A�R���{���� */
            if (timeCount > damageEffect.EffectLength)
            {
                damageEffect.damage(0, false);
                IsHitting = false;
            }
        }
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
        damageEffect.damage(damage,IsHitting);
        IsHitting = true;
        timeCount = 0;
    }

    /* ���񂾂猩���Ȃ��Ȃ� */
    protected override void Die()
    {
        gameObject.SetActive(false);
    }
}
