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


    public List<Gun> guns = new List<Gun>();//����̊i�[�z��
    private int selectedGun = 0;//�I�𒆂̕���Ǘ��p���l


    private float shotTimer;//�ˌ��Ԋu
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

    private void Awake()
    {
        //�^�O����UIManager��T��
        uIManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();

        //�^�O����SpawnManager��T��
        spawnManager = GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<SpawnManager>();

        animator = GetComponent<Animator>();

        string[] attackAnimationNames = { "Female Sword Attack 1" , "Female Sword Attack 2", "Female Sword Attack 3" };
        float[] damages = { 10, 15, 20 };

        Dictionary<string, float> a = new Dictionary<string, float>();
        for (int i = 0; i < damages.Length; i++)
            a.Add(attackAnimationNames[i], damages[i]);

        Motion_Damage = new Dictionary<AnimationClip, float>();

        var animation_clips = animator.runtimeAnimatorController.animationClips;
        var animation_names = new List<string>();

        foreach(var e in animator.runtimeAnimatorController.animationClips)
        {
            foreach(var n in attackAnimationNames)
            {
                if (e.ToString().Contains(n))
                {
                    Debug.Log(e.ToString() + "はあってあました");
                    Motion_Damage.Add(e, a[n]);
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
        uIManager.SettingBulletsText(ammoClip[selectedGun], ammunition[selectedGun]);

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

        //�e�̐؂�ւ�
        SwitchingGuns();

        //�`������
        Aim();

        //�ˌ��֐�
        Fire();

        //�����[�h�֐�
        Reload();

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
        uIManager.SettingBulletsText(ammoClip[selectedGun], ammunition[selectedGun]);
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
    public void SwitchingGuns()
    {
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            selectedGun++;//�����e���Ǘ����鐔�l�𑝂₷

            //���X�g���傫�����l�ɂȂ��Ă��Ȃ����m�F
            if (selectedGun >= guns.Count)
            {
                selectedGun = 0;//���X�g���傫�Ȑ��l�ɂȂ�΂O�ɖ߂�
            }
            switchGun();
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            selectedGun--;//�����e���Ǘ����鐔�l�����炷


            if (selectedGun < 0)
            {
                selectedGun = guns.Count - 1;//0��菬������΃��X�g�̍ő吔�|�P�̐��l�ɐݒ肷��
            }

            //���ۂɕ����؂�ւ���֐�
            switchGun();
        }

        //���l�L�[�̓��͌��m�ŕ����؂�ւ���
        for (int i = 0; i < guns.Count; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))//���[�v�̐��l�{�P�����ĕ�����ɕϊ��B���̌�A�����ꂽ������
            {
                selectedGun = i;//�e���������l��ݒ�

                //���ۂɕ����؂�ւ���֐�
                switchGun();

            }
        }
    }

    /// <summary>
    /// �e�̐؂�ւ�
    /// </summary>
    void switchGun()
    {
        foreach (Gun gun in guns)//���X�g�����[�v����
        {
            gun.gameObject.SetActive(false);//�e���\��
        }

        guns[selectedGun].gameObject.SetActive(true);//�I�𒆂̏e�̂ݕ\��
    }



    /// <summary>
    /// �E�N���b�N�Ŕ`������
    /// </summary>
    public void Aim()
    {
        //  �}�E�X�E�{�^�������Ă���Ƃ�
        if (Input.GetMouseButton(1))
        {
            //fieldOfView�R���|�[�l���g�̒l��ύX(�J�n�n�_�A�ړI�n�_�A�⊮���l)�@�@�J�n�n�_����ړI�n�_�܂ŕ⊮���l�̊����ŏ��X�ɋ߂Â���
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, guns[selectedGun].adsZoom, guns[selectedGun].adsSpeed * Time.deltaTime);
        }
        else
        {   //60�͏����ݒ萔�l
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 60f, guns[selectedGun].adsSpeed * Time.deltaTime);
        }
    }



    /// <summary>
    /// ���N���b�N�̌��m
    /// </summary>
    public void Fire()
    {

        if (Input.GetMouseButton(0) && ammoClip[selectedGun] > 0 && Time.time > shotTimer)
        {
            FiringBullet();
        }

    }

    /// <summary>
    /// �e�ۂ̔���
    /// </summary>
    private void FiringBullet()
    {
        //�I�𒆂̏e�̒e�򌸂炷
        ammoClip[selectedGun]--;

        //Ray(����)���J�����̒�������ɐݒ�
        Ray ray = cam.ViewportPointToRay(new Vector2(.5f, .5f));//�J�����̒��S�����̐��l


        //���C���΂��i�J�n�n�_�ƕ����A���������R���C�_�[�̏��i�[�j
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            //Debug.Log("���������I�u�W�F�N�g��" + hit.collider.gameObject.name);

            //�e���G�t�F�N�g (hit.point�̓R���C�_�[�Ƀq�b�g�����ʒu)�Fhit.point + (hit.normal * .002f)�͂�����Ȃ��悤�ɏ�����ɂ��Ă���
            //hit normal�͓��������I�u�W�F�N�g�ɑ΂��Ē��p�̕������Ԃ����
            //LookRotation�͎w�肵�������ɉ�
            GameObject bulletImpactObject = Instantiate(guns[selectedGun].bulletImpact, hit.point + (hit.normal * .002f), Quaternion.LookRotation(hit.normal, Vector3.up));

            //���Ԍo�߂ŏ�����悤�ɂ���
            Destroy(bulletImpactObject, 10f);
        }

        //�ˌ��Ԋu��ݒ�
        shotTimer = Time.time + guns[selectedGun].shootInterval;


    }


    /// <summary>
    /// �����[�h
    /// </summary>
    private void Reload()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            //�����[�h�ŕ�[����e�����擾����
            int amountNeed = maxAmmoClip[selectedGun] - ammoClip[selectedGun];

            //�K�v�Ȓe��ʂƏ����e��ʂ��r
            int ammoAvailable = amountNeed < ammunition[selectedGun] ? amountNeed : ammunition[selectedGun];

            //�e�򂪖��^���̎��̓����[�h�ł��Ȃ�&�e����������Ă���Ƃ�
            if (amountNeed != 0 && ammunition[selectedGun] != 0)
            {
                //�����e�򂩂烊���[�h����e�򕪂�����
                ammunition[selectedGun] -= ammoAvailable;
                //�e�ɑ��U����
                ammoClip[selectedGun] += ammoAvailable;
            }
        }
    }

    private void Attack()
    {
        animator.SetBool("Attack", Input.GetMouseButton(0));
    }



    public float GetDamage(AnimationClip hitAttack)
    {
        float damage;
        if (!Motion_Damage.TryGetValue(hitAttack, out damage)) 
            damage = -1;

        return damage;
    }


}