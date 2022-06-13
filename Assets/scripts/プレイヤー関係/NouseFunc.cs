using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NouseFunc : PlayerController
{
    public List<Gun> guns = new List<Gun>();//����̊i�[�z��
    private Camera cam;//�J����
    private int selectedGun = 0;//�I�𒆂̕���Ǘ��p���l
    private float shotTimer;//�ˌ��Ԋu
    UIManager uIManager;

    private void Start()
    {
        uIManager.SettingBulletsText(ammoClip[selectedGun], ammunition[selectedGun]);
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

    void switchGun()
    {
        foreach (Gun gun in guns)//���X�g�����[�v����
        {
            gun.gameObject.SetActive(false);//�e���\��
        }

        guns[selectedGun].gameObject.SetActive(true);//�I�𒆂̏e�̂ݕ\��
    }

}
