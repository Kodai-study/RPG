using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TatolEnemy : Enemy
{
    NavMeshAgent navigation;        //�G�̈ړ�(�v���C���[�Ɍ�����)���i��AI
    public Transform chacePlayer;   //�ǂ�������v���C���[�̈ʒu���(�f�o�b�O�p�Ƀv���C���[���C���X�y�N�^�œ���)
    public float nockBackLength;    //�m�b�N�o�b�N�ŗ^��������x�̌W��(�J�����̌����Ă�������ɗ^������)
    public float nockBack_Y;        //�m�b�N�o�b�N�ŗ^����������x�́AY��(������)�̌W��
    public float groundCheckLength; //���S����ǂꂭ�炢���ɒn�ʂ�����Βn�ʂɂ��锻��ɂȂ邩�̒l
    private float secCount = 0;     //�m�b�N�o�b�N��Ԃ���̕��A�Ŏg���A���Ԍo�߂̃J�E���^�[
    private bool isNockBack = false; //�m�b�N�o�b�N��ԉ��𔻒�
    public float nockBackDelay;     //�m�b�N�o�b�N���n�܂��Ă���A�n�ʔ��肪�X�g�b�v���鎞��
    private float size;

    void Start()
    {
        HP = MAX_HP;
        navigation = GetComponent<NavMeshAgent>();
        size = transform.localScale.x;
    }

    // Update is called once per frame
    override protected void Update()
    {
        base.Update();
        /* �i�r�Q�[�V�������L���Ȃ�A�������������ꏊ���v���C���[�̏ꏊ�ɂ��� */
        if (navigation != null && navigation.isActiveAndEnabled)
            navigation.destination = chacePlayer.position;

        /* (�f�o�b�O�p)A�{�^���Ńm�b�N�o�b�N */
        if (Input.GetKeyDown(KeyCode.K))
            NockBack(chacePlayer);

        /* �m�b�N�o�b�N��ԂŁA�n�ʂɍĂђ��������̏��� */
        if (isNockBack)
        {
            if (secCount >= nockBackDelay && IsGround())
            {
                isNockBack = false;         //�m�b�N�o�b�N��Ԃ̉���
                navigation.enabled = true;  //�i�r�Q�[�V�������ĊJ
                secCount = 0;
            }
            else
                secCount += Time.deltaTime; //�m�b�N�o�b�N���n�܂��Ă���̎��Ԃ��J�E���g(��莞�ԂŒn�ʔ���̕���)
        }
    }


    /* �_���[�W���󂯂��Ƃ��̏��� */
    public override void Dameged(float damage)
    {
        base.Dameged(damage);
        NockBack(oldCharaAnimator.gameObject.transform); //�Ƃ肠�����Ń_���[�W���ɂ�炸�����m�b�N�o�b�N����
    }


    /* �_���[�W���󂯂��Ƃ��̃m�b�N�o�b�N���� */
    private void NockBack(Transform attackedPlayer)
    {
        Camera camera;                          //�v���C���[�������Ă���J����
        Vector3 NockBackDirection;              //�m�b�N�o�b�N�ŗ^����������x
        camera = attackedPlayer.GetComponentInChildren<Camera>();   //�U�����Ă����v���C���[�̃J�������擾
        /** TODO �U�����Ă����I�u�W�F�N�g���v���C���[���ǂ����𔻒� */
        Ray ray = camera.ViewportPointToRay(new Vector3(1f, 0f, 1f));   //�U���v���C���[�̃J�����̌����Ă���������擾
        Debug.DrawRay(ray.origin,ray.direction);                    ///Debug  

        NockBackDirection = ray.direction;                          //�J�����������Ă��������3���Ŏ擾
        NockBackDirection.y = nockBack_Y;                           //�m�b�N�o�b�N���鍂�����A�N���X�ϐ��ɐݒ�
        NockBackDirection *= (nockBackLength * size);                        //�m�b�N�o�b�N�ŗ^����
        GetComponent<Rigidbody>().AddForce(NockBackDirection,ForceMode.VelocityChange); //�v�Z���������x��^���ăm�b�N�o�b�N

        isNockBack = true;                              //�m�b�N�o�b�N���
        navigation.enabled = false;                     //�m�b�N�o�b�N��Ԃł̓i�r�Q�[�V�����I�t(������ɂ����Ȃ�����)
    }

    private bool IsGround()
    {
        /* �����̒��S���牺���������āA�ϐ��l���߂������ɒn�ʂ���������n�ʂɐG��Ă����Ɣ��� */
        return Physics.Raycast(transform.position, Vector3.down, groundCheckLength * size, groundLayer);
    }

    protected override void Die()
    {
        

    }

}
