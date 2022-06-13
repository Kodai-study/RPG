using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstValue;
//import (�p�b�P�[�W��).(������N���X��)

public class EffectDamage : MonoBehaviour
{

    public AnimationCurve colorChangeCurve; //�G�t�F�N�g���ɁA�F���ǂ�Ȃӂ��ɑJ�ڂ��Ă�������\�����J�[�u(0�`1)
    public Color startColor;                //�_���[�W�G�t�F�N�g�J�n���́A�����ɂȂ��Ă���F
    public Color endColor;                  //�_���[�W�G�t�F�N�g�I���́A�����x��0�̐F
    private TextMesh damageView;            //�q�I�u�W�F�N�g�A�_���[�W���l��\������3D�e�L�X�g
    public Animator animator;
    public AnimationClip attackAnimation;
    private float effectTime;
    private float totalDamage;

    [Range(0f, 1f)]
    public float t;

    void Start()
    {
        damageView = GetComponentInChildren<TextMesh>();
        effectTime = attackAnimation.length;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            animator.SetTrigger(AnimationParams.GET_DAMAGE_TRIGGER_NAME);
        }
        damageView.color = Color.Lerp(startColor, endColor, colorChangeCurve.Evaluate(t));
    }

    public void damage(float damageValue, bool keepHit)
    {
        if (!keepHit && damageValue > 0)
        {
            totalDamage = damageValue;
        }
        else
        {
            totalDamage += damageValue;
        }
        damageView.text = ((int)totalDamage).ToString();
        if (damageValue > 0)
        {
            animator.SetTrigger(AnimationParams.GET_DAMAGE_TRIGGER_NAME);
        }
        animator.SetBool(AnimationParams.DAMAGE_FLAG_NAME, keepHit);
        Debug.Log(keepHit);
    }

    public float EffectLength
    {
        get { return effectTime; }
    }
}
