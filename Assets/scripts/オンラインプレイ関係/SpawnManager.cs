using UnityEngine;
using Photon.Pun;

public class SpawnManager : MonoBehaviour
{
    //�X�|�[���|�C���g�I�u�W�F�N�g�i�[�z��
    public Transform[] spawnPositons;
    public GameObject playerPrefab;     //�����I�u�W�F�N�g
    private GameObject player;          //���������v���C���[���i�[
    public float respawnInterval = 5f;  //�����܂łɗv���鎞��


    private void Start()
    {
        //�X�|�[���|�C���g�I�u�W�F�N�g�����ׂĔ�\��
        foreach (var pos in spawnPositons)
        {
            pos.gameObject.SetActive(false);
        }


        //�l�b�g���[�N�ɂȂ����Ă���Ȃ�X�|�[��
        if (PhotonNetwork.IsConnected)
        {
            SpawnPlayer();
        }
    }

    /// <summary>
    /// ���X�|�[���n�_�������_���擾����֐�
    /// </summary>
    public Transform GetSpawnPoint()
    {
        //�����_���ŃX�|�[���|�C���g�P�I��ňʒu����Ԃ�
        return spawnPositons[Random.Range(0, spawnPositons.Length)];
    }

    //Player�𐶐�����
    public void SpawnPlayer()
    {
        Transform spawnPoint = GetSpawnPoint();

        //�l�b�g���[�N��̃v���C���[�𐶐�����F�������ł̓I�u�W�F�N�g�̖��O���K�v
        player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);//��ō폜����Ƃ��̂��߂Ɋi�[
    }
}