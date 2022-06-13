using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;

public class Room : MonoBehaviour
{
    public Text buttonText;//���[�������f�p�̃e�L�X�g
    private RoomInfo info;// �����̏����i�[����ϐ�

    /// <summary>
    /// ���[���{�^���ɏڍׂ�o�^
    /// </summary>
    public void RegisterRoomDetails(RoomInfo info)
    {
        this.info = info;//���[����info�Ɉ�����info���i�[

        buttonText.text = this.info.Name;//�e�L�X�g�X�V
    }


    //���̃��[���{�^�����Ǘ����Ă��郋�[���ɎQ������F�{�^��UI����Ăяo��
    public void OpenRoom()
    {
        //���[���ɎQ���֐����Ăяo��
        PhotonManager.instance.JoinRoom(info);
    }
}