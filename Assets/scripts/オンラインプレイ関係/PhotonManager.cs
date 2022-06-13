using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

//photonView�ƁAPUN���Ăяo�����Ƃ̂ł��邷�ׂẴR�[���o�b�N/�C�x���g��񋟂��܂��B�g�p�������C�x���g/���\�b�h���I�[�o�[���C�h���Ă��������B
public class PhotonManager : MonoBehaviourPunCallbacks
{
    //�悭����h�L�������g�y�[�W
    //https://doc-api.photonengine.com/ja-jp/pun/current/class_photon_1_1_pun_1_1_photon_network.html
    //https://doc-api.photonengine.com/ja-jp/pun/current/class_photon_1_1_pun_1_1_mono_behaviour_pun_callbacks.html
    //https://doc-api.photonengine.com/ja-jp/pun/current/namespace_photon_1_1_realtime.html

    public static PhotonManager instance;//static
    public GameObject loadingPanel;//���[�h�p�l��
    public Text loadingText;//���[�h�e�L�X�g
    public GameObject buttons;//�{�^��


    public GameObject createRoomPanel;//���[���쐬�p�l��
    public Text enterRoomName;//���͂��ꂽ���[�����e�L�X�g


    public GameObject roomPanel;//���[���p�l��
    public Text roomName;//���[�����e�L�X�g


    public GameObject errorPanel;//�G���[�p�l��
    public Text errorText;//�G���[�e�L�X�g


    public GameObject roomListPanel;//���[���ꗗ�p�l��


    public Room originalRoomButton;//���[���{�^���i�[
    public GameObject roomButtonContent;//���[���{�^���̐e�I�u�W�F�N�g
    Dictionary<string, RoomInfo> roomsList = new Dictionary<string, RoomInfo>();//���[���̏�����������
    private List<Room> allRoomButtons = new List<Room>();//���[���{�^�����������X�g


    public Text playerNameText;//�v���C���[�e�L�X�g
    private List<Text> allPlayerNames = new List<Text>();//�v���C���[�̊Ǘ����X�g
    public GameObject playerNameContent;//�v���C���[�l�[���̐e�I�u�W�F�N�g


    public GameObject nameInputPanel;//���O���̓p�l��
    public Text placeholderText;//�\���e�L�X�g�A
    public InputField nameInput;//���O���̓t�H�[��
    private bool setName;//���O���͔���


    public GameObject startButton;//�Q�[���J�n���邽�߂̃{�^��


    public string levelToPlay;//�J�ڐ�̃V�[����


    private void Awake()
    {
        instance = this;//�i�[
    }


    void Start()
    {
        //���j���[�����ׂĕ���
        CloseMenuUI();

        //���[�h�p�l����\�����ăe�L�X�g�X�V
        loadingPanel.SetActive(true);
        loadingText.text = "�l�b�g���[�N�ɐڑ���...";

        //�l�b�g���[�N�ɐڑ����Ă���̂��m�F
        if (!PhotonNetwork.IsConnected)
        {
            //�ŏ��ɐݒ肵��PhotonServerSettings�t�@�C���̐ݒ�ɏ]����Photon�ɐڑ�
            PhotonNetwork.ConnectUsingSettings();
        }
    }


    /// <summary>
    /// ��U���ׂĂ��\���ɂ���
    /// </summary>
    void CloseMenuUI()//�Ȃ����̂��FUI�؂�ւ������Ɋy������
    {
        loadingPanel.SetActive(false);//���[�h�p�l����\��

        buttons.SetActive(false);//�{�^����\��

        createRoomPanel.SetActive(false);//���[���쐬�p�l��

        roomPanel.SetActive(false);//���[���p�l��

        errorPanel.SetActive(false);//�G���[�p�l��

        roomListPanel.SetActive(false);//���[���ꗗ�p�l��

        nameInputPanel.SetActive(false);//���O���̓p�l��
    }



    //�p�����̃��\�b�h�ł́uvirtual�v�̃L�[���[�h
    //�p����ł́uoverride�v�̃L�[���[�h
    /// <summary>
    /// �N���C�A���g��Master Server�ɐڑ�����Ă��āA�}�b�`���C�L���O�₻�̑��̃^�X�N���s���������������Ƃ��ɌĂяo����܂�
    /// </summary>
    public override void OnConnectedToMaster()//
    {

        PhotonNetwork.JoinLobby();//�}�X�^�[�T�[�o�[��ŁA�f�t�H���g���r�[�ɓ���܂�

        loadingText.text = "���r�[�ւ̎Q��...";//�e�L�X�g�X�V

        PhotonNetwork.AutomaticallySyncScene = true;//�}�X�^�[�Ɠ����V�[���ɍs���悤�ɐݒ�
    }


    /// <summary>
    /// �}�X�^�[�T�[�o�[�̃��r�[�ɓ���Ƃ��ɌĂяo����܂��B
    /// </summary>
    public override void OnJoinedLobby()//
    {

        LobbyMenuDisplay();


        roomsList.Clear();//�����̏�����


        PhotonNetwork.NickName = Random.Range(0, 1000).ToString();//���[�U�[�l�[�����Ƃ肠�����K���Ɍ��߂�


        ConfirmationName();//���O�����͂���Ă���΂��̖��O����̓e�L�X�g�ɔ��f������
    }


    //���r�[���j���[�\��(�G���[�p�l�����鎞������)
    public void LobbyMenuDisplay()
    {
        CloseMenuUI();
        buttons.SetActive(true);
    }

    //�^�C�g���̕����쐬�{�^���������ɌĂԁFUI����Ăяo��
    public void OpenCreateRoomPanel()
    {
        CloseMenuUI();
        createRoomPanel.SetActive(true);
    }

    //�����쐬�{�^���������ɌĂԁFUI����Ăяo��
    public void CreateRoomButton()
    {
        //�C���v�b�g�t�B�[���h�̃e�L�X�g�ɉ������͂���Ă����ꍇ
        if (!string.IsNullOrEmpty(enterRoomName.text))
        {
            //���[���̃I�v�V�������C���X�^���X�����ĕϐ��ɓ���� 
            RoomOptions options = new RoomOptions();
            options.MaxPlayers = 6;// �v���C���[�̍ő�Q���l���̐ݒ�i�����ł�20�܂ŁB1�b�Ԃɂ����ł��郁�b�Z�[�W���Ɍ��肪����̂�10�ȏ�͓�Փx�オ��j

            //���[�������(���[�����F�����̐ݒ�)
            PhotonNetwork.CreateRoom(enterRoomName.text, options);

            CloseMenuUI();//���j���[����
            loadingText.text = "���[���쐬��...";
            loadingPanel.SetActive(true);
        }
    }


    //���[���ɎQ��������Ă΂��֐�
    public override void OnJoinedRoom()
    {
        CloseMenuUI();//��U���ׂĂ����
        roomPanel.SetActive(true);//���[���p�l���\��

        roomName.text = PhotonNetwork.CurrentRoom.Name;//���݂��郋�[�����擾���A�e�L�X�g�Ƀ��[�����𔽉f

        GetAllPlayer();//���[���ɎQ�����Ă���v���C���[��\��

        CheckRoomMaster();//���[���}�X�^�[�����肷��
    }




    //�ޏo�{�^���������ɌĂ΂��B�Q�����̕����𔲂���
    public void LeavRoom()
    {
        //���݂̃��[�����o�āA�}�X�^�[�T�[�o�[�ɖ߂��āA���[���ɎQ�������胋�[�����쐬������ł��܂�
        PhotonNetwork.LeaveRoom();

        CloseMenuUI();

        loadingText.text = "�ޏo���E�E�E";
        loadingPanel.SetActive(true);
    }


    //���[���𔲂����Ƃ��ɌĂ΂��
    public override void OnLeftRoom()
    {
        LobbyMenuDisplay();
    }

    //�T�[�o�[�����[�����쐬�ł��Ȃ������Ƃ��ɌĂяo����܂��B
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "���[���̍쐬�Ɏ��s���܂���" + message;
        CloseMenuUI();
        errorPanel.SetActive(true);
    }


    //���[���ꗗ��ʂ��J���F�{�^������Ă�
    public void FindRoom()
    {
        CloseMenuUI();
        roomListPanel.SetActive(true);

    }


    //Master Server�̃��r�[�ɂ���ԂɁA���[�����X�g���X�V���邽�߂ɌĂяo����܂��B
    public override void OnRoomListUpdate(List<RoomInfo> roomList)//
    {
        RoomUIinitialization();//���[��UI�̏�����

        UpdateRoomList(roomList);//���[�����������Ɋi�[
    }

    //���[���̏���������
    public void UpdateRoomList(List<RoomInfo> roomList)
    {
        for (int i = 0; i < roomList.Count; i++)//���[���̐������[�v
        {
            RoomInfo info = roomList[i];//���[������ϐ��Ɋi�[

            if (info.RemovedFromList)//���r�[�Ŏg�p����A���X�g����Ȃ��Ȃ����������}�[�N���܂��i�����A���A�܂��͔�\���j
            {
                roomsList.Remove(info.Name);//��������폜
            }
            else
            {
                roomsList[info.Name] = info;//���[�������L�[�ɂ��āA�����ɒǉ�
            }
        }

        RoomListDisplay(roomsList);//�����ɂ��邷�ׂẴ��[����\��
    }

    //���[���\��
    void RoomListDisplay(Dictionary<string, RoomInfo> cachedRoomList)
    {
        //�����̃L�[/�l�@��foreach����
        foreach (var roomInfo in cachedRoomList)
        {
            //���[���{�^���쐬
            Room newButton = Instantiate(originalRoomButton);
            //���������{�^���Ƀ��[���̏���ݒ�
            newButton.RegisterRoomDetails(roomInfo.Value);
            //���������{�^���ɐe�̐ݒ�
            newButton.transform.SetParent(roomButtonContent.transform);
            //���X�g�ɒǉ�
            allRoomButtons.Add(newButton);
        }


    }

    //���[���{�^��UI������
    void RoomUIinitialization()
    {
        foreach (Room rm in allRoomButtons)// ���[���I�u�W�F�N�g�̐������[�v
        {
            Destroy(rm.gameObject);// �{�^���I�u�W�F�N�g���폜
        }

        allRoomButtons.Clear();//���X�g�v�f�폜

    }



    /// <summary>
    /// �����̃��[���ɓ���֐�
    /// </summary>
    /// <param name="roomInfo"></param>
    public void JoinRoom(RoomInfo roomInfo)
    {
        //�����̃��[���ɎQ������
        PhotonNetwork.JoinRoom(roomInfo.Name);

        CloseMenuUI();
        loadingText.text = "���[���Q����";
        loadingPanel.SetActive(true);
    }



    /// <summary>
    /// ���[���ɂ���v���C���[���擾����֐�
    /// </summary>
    public void GetAllPlayer()
    {
        //������
        InitializePlayerList();

        //�v���C���[�\��
        PlayerDisplay();
    }


    //�v���C���[�ꗗ������
    void InitializePlayerList()
    {
        //���X�g�ŊǗ����Ă��鐔�����[�v
        foreach (var rm in allPlayerNames)
        {
            //text�폜
            Destroy(rm.gameObject);
        }

        //���X�g������
        allPlayerNames.Clear();

    }

    //���[���ɂ���v���C���[��\������
    void PlayerDisplay()
    {
        //���[���ɐڑ����Ă���v���C���[�̐������[�v
        foreach (var players in PhotonNetwork.PlayerList)
        {
            //�e�L�X�g�̐���
            PlayerTextGeneration(players);
        }
    }

    //�v���C���[�e�L�X�g����
    void PlayerTextGeneration(Player players)
    {
        Text newPlayerText = Instantiate(playerNameText);//�p�ӂ��Ă���e�L�X�g���x�[�X�Ƀv���C���[�e�L�X�g�𐶐�
        newPlayerText.text = players.NickName;//�e�L�X�g�ɖ��O�𔽉f
        newPlayerText.transform.SetParent(playerNameContent.transform);//�e�̐ݒ�


        allPlayerNames.Add(newPlayerText);//���X�g�ɒǉ�
    }



    //���O�̔���
    private void ConfirmationName()
    {
        if (!setName)//���O����͂��Ă��Ȃ��ꍇ
        {
            CloseMenuUI();

            nameInputPanel.SetActive(true);


            if (PlayerPrefs.HasKey("playerName"))//�L�[���ۑ�����Ă��邩�m�F
            {

                placeholderText.text = PlayerPrefs.GetString("playerName");

                nameInput.text = PlayerPrefs.GetString("playerName");//�C���v�b�g�t�B�[���h�ɖ��O��\�����Ă���



            }

        }
        else//��x���͂������Ƃ�����ꍇ�͎����I�ɖ��O���Z�b�g����i���[�������Ė߂������Ƃ����������\������Ȃ��悤�Ɂj
        {
            PhotonNetwork.NickName = PlayerPrefs.GetString("playerName");
        }
    }

    //���O�ۑ�����͔���؂�ւ��B�{�^������Ă�
    public void SetName()
    {
        if (!string.IsNullOrEmpty(nameInput.text))//���͂���Ă���ꍇ
        {
            PhotonNetwork.NickName = nameInput.text;//���[�U�[���ɓ��͂��ꂽ���O�𔽉f

            PlayerPrefs.SetString("playerName", nameInput.text);//���O��ۑ�����

            LobbyMenuDisplay();//���r�[�ɖ߂�

            setName = true;//���O���͍ςݔ���
        }
    }


    //�v���C���[�����[���ɓ������Ƃ��ɌĂяo����܂��B
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //�e�L�X�g�𐶐�����
        PlayerTextGeneration(newPlayer);
    }


    //�v���C���[�����[���𗣂�邩�A��A�N�e�B�u�ɂȂ����Ƃ��ɌĂяo����܂��B
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //���[�����ɂ���v���C���[���X�g�̍X�V���ĕ\���𐳂�������
        GetAllPlayer();
    }


    //�����̃}�X�^�[���m�F����
    private void CheckRoomMaster()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //�}�X�^�[�݂̂ɊJ�n�{�^����\��������
            startButton.gameObject.SetActive(true);
        }
        else
        {
            startButton.gameObject.SetActive(false);
        }
    }

    //���݂�MasterClient���I�������Ƃ��ɐV����MasterClient�ɐ؂�ւ�����ɌĂяo����܂�
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        //�����̃}�X�^�[���m�F����
        if (PhotonNetwork.IsMasterClient)
        {
            //�����̃}�X�^�[�ɂȂ��Ă�����J�n�{�^����\������
            startButton.gameObject.SetActive(true);
        }

    }



    //�J�n�{�^���Ŏw��V�[���ɑJ�ڂ���FUI����Ă�
    public void PlayGame()
    {
        //�����̃X�e�[�W��ǂݍ���
        PhotonNetwork.LoadLevel(levelToPlay);

    }

}