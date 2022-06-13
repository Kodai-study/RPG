using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * ���ۃN���XEnemy
 * �쐬 : �܏\�� �q��
 * �@�\ : HP�̊Ǘ��A�����蔻��̊Ǘ����s���B ��_���[�W���A���S���̒��ۃ��\�b�h�̒�`
 * ��*/

public abstract class Enemy : MonoBehaviour
{
    [SerializeField]public int MAX_HP;
    protected int hp;
    protected const string ATTACK_TAG_NAME = "Attack_Object";  //�����蔻��ƂȂ�I�u�W�F�N�g�̃^�O�� 

    /**TODO �U�����Ă���L�����̋��ʓ_(�e�N���X�A�p�����[�^�Ȃ�)���`���A������₷������������ */
    protected Animator oldCharaAnimator;           
    protected private float hitCount = 0f;      //�O��̓����蔻�肩��̎���
    protected AnimationClip oldHitAnimation;    //�O��̓����蔻�莞�ɑ��肪���Ă����U�����[�V����
    [SerializeField]protected LayerMask groundLayer;    //�n�ʔ���ƂȂ�I�u�W�F�N�g�̃��C����


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

    /* �_���[�W���󂯂��Ƃ��̏����B�I�[�o�[���C�h�O�� */
    public virtual void Dameged(float damage)
    {
        HP -= (int)damage;
    }

    /* �����蔻�肪�N�����Ƃ��ɁA�_���[�W���󂯂邩�ǂ����̏��� */
    virtual protected void OnTriggerEnter(Collider other)
    {
        /* �����蔻��̑��肪�A�j���[�V�����R���|�[�l���g�������Ă�����U���\�҂Ƃ��ĔF�� */
        Animator attackedAnimation = other.GetComponentInParent<Animator>();
        if (attackedAnimation == null)
        {
            Debug.Log("�U���҂̃A�j���[�V�������擾�ł��܂���ł���");
            return;
        }

        if (other.tag == ATTACK_TAG_NAME || other.TryGetComponent<Animator>(out attackedAnimation))
        {
            //���ݓ����Ă���A�j���[�V�����̎擾
            var nowAnimation = attackedAnimation.GetCurrentAnimatorClipInfo(0)[0].clip;
            /* �E���߂Ă̓����蔻�� �E�O�ƈႤ��ނ̃A�j���[�V�����ɂ��U�� �E�U���̃A�j���[�V���������������Ԃ������Ă���̍U��
             *  ��3�̓��ǂꂩ�𖞂����Ă���΃_���[�W���󂯂�
             */
            if (oldHitAnimation == null || oldHitAnimation != nowAnimation || hitCount > nowAnimation.length || attackedAnimation != oldCharaAnimator)
            {
                var attackedCharacotr = other.GetComponentInParent<PlayerController>();
                string st = attackedCharacotr.name; ///Debug �U���҂̖��O���擾
                float damage = attackedCharacotr.GetDamage(nowAnimation);
                if (damage > 0)
                {
                    Dameged(damage);
                    st += "����̍U��" + nowAnimation.name + " �_���[�W " + damage;
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
