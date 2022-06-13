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

    [Tooltip("�_���[�W�\�L�̂͂���")]
    public AnimationClip startAnimation;
    [Tooltip("�_���[�W�\�L�������Ă����A�j���[�V����")]
    public AnimationClip endAnimation;
    private float effectTime = 0;           //�R���{����ɂȂ鎞��(�A�j���[�V�����̒����ɐݒ�)
    private float totalDamage;          //�\�L�����_���[�W���B�R���{���Ɖ��Z����Ă���
    private float timeCount = 0;        //�O�񓖂��蔻�肪�����Ă���̎��Ԃ𑪂�J�E���^
    private bool ISCombo = false;
    [Range(0f, 1f)]
    public float t;

    void Start()
    {
        damageView = GetComponentInChildren<TextMesh>();
        effectTime = startAnimation.length + endAnimation.length;
    }

    void Update()
    {
        //T�̒l�������ƂȂ�A�J�[�u�̏c���̒l�����F��endColor�ɋ߂Â�
        damageView.color = Color.Lerp(startColor, endColor, colorChangeCurve.Evaluate(t));
        timeCount += Time.deltaTime;        //��ɃJ�E���g���

        /* �_���[�W�\�L�̃A�j���[�V�������I�������A�R���{��Ԃ��������ăG�t�F�N�g������ */
        if (timeCount > startAnimation.length)
        {
            ISCombo = false;
            animator.SetBool(AnimationParams.DAMAGE_FLAG_NAME, false);
        }
    }

    /* �_���[�W���󂯂��Ƃ��ɌĂ΂�A�G�t�F�N�g����������֐� */
    public void damage(float damageValue)
    {
        if (damageValue < 0)
            return;

        /* �O��̓����蔻�肩��A�_���[�W�G�t�F�N�g���I������܂łɐV���Ƀ_���[�W���󂯂���A�R���{���� */
        if (timeCount < effectTime)
        {
            animator.SetBool(AnimationParams.DAMAGE_FLAG_NAME, ISCombo = true);
        }

        timeCount = 0;

        /* �n�߂ē������� */
        if (!ISCombo)
        {
            ISCombo = true;
            totalDamage = damageValue;
        }
        /* �R���{��Ԃł͕\�L�_���[�W�����Z����� */
        else
        {
            totalDamage += damageValue;
        }
        damageView.text = ((int)totalDamage).ToString();
        animator.SetTrigger(AnimationParams.GET_DAMAGE_TRIGGER_NAME);
    }

    public float EffectLength
    {
        get { return effectTime; }
    }
}
