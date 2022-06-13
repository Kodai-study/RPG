using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks
{
    public Transform viewPoint;//�J�����̈ʒu�I�u�W�F�N�g
    public float mouseSensitivity = 1f;//���_�ړ��̑��x
    private Vector2 mouseInput;//���[�U�[�̃}�E�X���͂��i�[
    private float verticalMouseInput;//y���̉�]���i�[�@��]�𐧌�����������
    private Camera cam;//�J����
    Animator animator;
    [SerializeField] private Rigidbody _rigidbody;

    private Vector3 moveDir;//�v���C���[�̓��͂��i�[�i�ړ��j
    private Vector3 movement;//�i�ޕ������i�[����ϐ�
    private float activeMoveSpeed = 4;//���ۂ̈ړ����x

    public Vector3 jumpForce = new Vector3(0, 6, 0);//�W�����v�� 
    public Transform groundCheckPoint;//�n�ʂɌ����ă��C���΂��I�u�W�F�N�g 
    public LayerMask groundLayers;//�n�ʂ��ƔF�����郌�C���[ 
    Rigidbody rb;//

    public float stopSpeed = 0 ,walkSpeed = 4, runSpeed = 8;//�����̑��x�A����̑��x

    private bool cursorLock = true;//�J�[�\���̕\��/��\�� 

    [Tooltip("�����e��")]
    public int[] ammunition;
    [Tooltip("�ō������e��")]
    public int[] maxAmmunition;
    [Tooltip("�}�K�W�����̒e��")]
    public int[] ammoClip;
    [Tooltip("�}�K�W���ɓ���ő�̐�")]
    public int[] maxAmmoClip;


    UIManager uIManager;//UI�Ǘ�


    SpawnManager spawnManager;//�X�|�[���}�l�[�W���[�Ǘ�

    AnimationClip nowAnimation;

    Dictionary<AnimationClip, float> Motion_Damage;

    private readonly string[] attackAnimationNames = { "Female Sword Attack 1", "Female Sword Attack 2", "Female Sword Attack 3" };
    private readonly float[] damages = { 10, 15, 20 };

    private void Awake()
    {
        //�^�O����UIManager��T��
        uIManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();

        //�^�O����SpawnManager��T��
        spawnManager = GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<SpawnManager>();

        animator = GetComponent<Animator>();


        Dictionary<string, float> a = new Dictionary<string, float>();
        for (int i = 0; i < damages.Length; i++)
            a.Add(attackAnimationNames[i], damages[i]);

        Motion_Damage = new Dictionary<AnimationClip, float>();

        foreach(var animClip in animator.runtimeAnimatorController.animationClips)
        {
            foreach(var animationName in attackAnimationNames)
            {
                if (animClip.ToString().Contains(animationName))
                {
                    Debug.Log(animClip.ToString() + "はあってあました");
                    Motion_Damage.Add(animClip, a[animationName]);
                }
            }
        }
    }

    private void Start()
    {
        _rigidbody = this.GetComponent<Rigidbody> ();

        //�ϐ��Ƀ��C���J�������i�[
        cam = Camera.main;


        //Rigidbody���i�[
        rb = GetComponent<Rigidbody>();


        //�J�[�\����\��
        UpdateCursorLock();


        //�e��e�L�X�g�X�V

        this.animator = GetComponent<Animator>();

        nowAnimation = animator.GetCurrentAnimatorClipInfo(0)[0].clip;

        //�����_���Ȉʒu�ŃX�|�[��������
        //transform.position = spawnManager.GetSpawnPoint().position;
    }

    private void Update() 
    {
        //�����ȊO��
        if (!photonView.IsMine)
        {
            //�߂��Ă���ȍ~�̏������s��Ȃ�
            return;
        }

        nowAnimation = animator.GetCurrentAnimatorClipInfo(0)[0].clip;

        Attack();

        //���_�ړ��֐�
        PlayerRotate();

        //�ړ��֐�
        PlayerMove();

        //�n�ʂɂ��Ă���̂����������
        if (IsGround())
        {
            //����̊֐����Ă�
            Run();

            //�W�����v�֐����Ă�
            Jump();
        }


        //�J�[�\����\��
        UpdateCursorLock();
    }

    //Update�֐����Ă΂ꂽ��Ɏ��s�����
    private void LateUpdate()
    {
        //�����ȊO��
        if (!photonView.IsMine)
        {
            //�߂��Ă���ȍ~�̏������s��Ȃ�
            return;
        }

        //�J�������v���C���[�̎q�ɂ���̂ł͂Ȃ��A�X�N���v�g�ňʒu�����킹��
        cam.transform.position = viewPoint.position;
        cam.transform.rotation = viewPoint.rotation;
    }

    //�����ݒ�ł�0.02�b���ƂɌĂ΂��
    private void FixedUpdate()
    {
        //�����ȊO��
        if (!photonView.IsMine)
        {
            //�߂��Ă���ȍ~�̏������s��Ȃ�
            return;
        }

        //�e��e�L�X�g�X�V
    }

    /// <summary>
    /// Player�̉���]�Əc�̎��_�ړ����s��
    /// </summary>
    public void PlayerRotate()
    {
        //�ϐ��Ƀ��[�U�[�̃}�E�X�̓������i�[
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X") * mouseSensitivity,
            Input.GetAxisRaw("Mouse Y") * mouseSensitivity);

        //����]�𔽉f(transform.eulerAngles�̓I�C���[�p�Ƃ��Ă̊p�x���Ԃ����)
        transform.rotation = Quaternion.Euler
            (transform.eulerAngles.x,
            transform.eulerAngles.y + mouseInput.x, //�}�E�X��x���̓��͂𑫂�
            transform.eulerAngles.z);


        //�ϐ���y���̃}�E�X���͕��̐��l�𑫂�
        verticalMouseInput += mouseInput.y;

        //�ϐ��̐��l���ۂ߂�i�㉺�̎��_����j
        verticalMouseInput = Mathf.Clamp(verticalMouseInput, -60f, 60f);

        //�c�̎��_��]�𔽉f(-��t���Ȃ��Ə㉺���]���Ă��܂�)
        viewPoint.rotation = Quaternion.Euler
            (-verticalMouseInput,
            viewPoint.transform.rotation.eulerAngles.y,
            viewPoint.transform.rotation.eulerAngles.z);
    }



    /// <summary>
    /// Player�̈ړ�
    /// </summary>
    public void PlayerMove()
    {
        //�ϐ��̐����Ɛ����̓��͂��i�[����iwasd����̓��́j
        moveDir = new Vector3(Input.GetAxisRaw("Horizontal"),
            0, Input.GetAxisRaw("Vertical"));

        //Debug.Log(moveDir);�����p

        //�Q�[���I�u�W�F�N�g�̂�����x���ɓ��͂��ꂽ�l��������Ɛi�ޕ������o����
        movement = ((transform.forward * moveDir.z) + (transform.right * moveDir.x)).normalized;

        //���݈ʒu�ɐi�ޕ������ړ��X�s�[�h���t���[���ԕb���𑫂�
        transform.position += movement * activeMoveSpeed * Time.deltaTime;
    }


    /// <summary>
    /// �n�ʂɂ��Ă���Ȃ�true
    /// </summary>
    /// <returns></returns>
    public bool IsGround()
    {
        return Physics.Raycast(groundCheckPoint.position, Vector3.down, 0.5f, groundLayers);
    }


    public void Jump()
    {
        //�W�����v�ł���̂�����
        if (IsGround() && Input.GetKeyDown(KeyCode.Space))
        {
            //�u�ԓI�ɐ^��ɗ͂�������
            rb.AddForce(jumpForce, ForceMode.Impulse);
            animator.SetBool("jump", true);
        }else{
            animator.SetBool("jump", false);
        }
    }


    public void Run()
    {
        if (nowAnimation.name.Contains("Attack"))
        {
            activeMoveSpeed = 0f;
            return;
        }

        //���V�t�g�������Ă���Ƃ��̓X�s�[�h��؂�ւ���
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.W))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                activeMoveSpeed = runSpeed;
            }
            else
            {
                activeMoveSpeed = walkSpeed;
            }
        }
        else
        {
            activeMoveSpeed = stopSpeed;
        }
        animator.SetFloat("speed", activeMoveSpeed);
    }



    public void UpdateCursorLock()
    {
        //���͂�������cursorLock��؂�ւ���
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            cursorLock = false;
        }
        else if (Input.GetMouseButton(0))
        {
            cursorLock = true;
        }

        //cursorLock����ŃJ�[�\���̕\����؂�ւ���
        if (cursorLock)
        {
            //�J�[�\���𒆉��ɌŒ肵�A��\���@https://docs.unity3d.com/ScriptReference/CursorLockMode.html
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            //�\��
            Cursor.lockState = CursorLockMode.None;
        }
    }


    /// <summary>
    /// �e�̐؂�ւ��L�[���͂����m����
    /// </summary>

    /// <summary>
    /// �e�̐؂�ւ�
    /// </summary>



    public float GetDamage(AnimationClip hitAttack)
    {
        float damage;
        if (!Motion_Damage.TryGetValue(hitAttack, out damage)) 
            damage = -1;

        return damage;
    }
    private void Attack()
    {
        animator.SetBool("Attack", Input.GetMouseButton(0));
    }

}