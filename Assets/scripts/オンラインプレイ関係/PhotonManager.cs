using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

//photonViewと、PUNが呼び出すことのできるすべてのコールバック/イベントを提供します。使用したいイベント/メソッドをオーバーライドしてください。
public class PhotonManager : MonoBehaviourPunCallbacks
{
    //よく見るドキュメントページ
    //https://doc-api.photonengine.com/ja-jp/pun/current/class_photon_1_1_pun_1_1_photon_network.html
    //https://doc-api.photonengine.com/ja-jp/pun/current/class_photon_1_1_pun_1_1_mono_behaviour_pun_callbacks.html
    //https://doc-api.photonengine.com/ja-jp/pun/current/namespace_photon_1_1_realtime.html

    public static PhotonManager instance;//static
    public GameObject loadingPanel;//ロードパネル
    public Text loadingText;//ロードテキスト
    public GameObject buttons;//ボタン


    public GameObject createRoomPanel;//ルーム作成パネル
    public Text enterRoomName;//入力されたルーム名テキスト


    public GameObject roomPanel;//ルームパネル
    public Text roomName;//ルーム名テキスト


    public GameObject errorPanel;//エラーパネル
    public Text errorText;//エラーテキスト


    public GameObject roomListPanel;//ルーム一覧パネル


    public Room originalRoomButton;//ルームボタン格納
    public GameObject roomButtonContent;//ルームボタンの親オブジェクト
    Dictionary<string, RoomInfo> roomsList = new Dictionary<string, RoomInfo>();//ルームの情報を扱う辞書
    private List<Room> allRoomButtons = new List<Room>();//ルームボタンを扱うリスト


    public Text playerNameText;//プレイヤーテキスト
    private List<Text> allPlayerNames = new List<Text>();//プレイヤーの管理リスト
    public GameObject playerNameContent;//プレイヤーネームの親オブジェクト


    public GameObject nameInputPanel;//名前入力パネル
    public Text placeholderText;//表示テキスト、
    public InputField nameInput;//名前入力フォーム
    private bool setName;//名前入力判定


    public GameObject startButton;//ゲーム開始するためのボタン


    public string levelToPlay;//遷移先のシーン名


    private void Awake()
    {
        instance = this;//格納
    }


    void Start()
    {
        //メニューをすべて閉じる
        CloseMenuUI();

        //ロードパネルを表示してテキスト更新
        loadingPanel.SetActive(true);
        loadingText.text = "ネットワークに接続中...";

        //ネットワークに接続しているのか確認
        if (!PhotonNetwork.IsConnected)
        {
            //最初に設定したPhotonServerSettingsファイルの設定に従ってPhotonに接続
            PhotonNetwork.ConnectUsingSettings();
        }
    }


    /// <summary>
    /// 一旦すべてを非表示にする
    /// </summary>
    void CloseMenuUI()//なぜ作るのか：UI切り替えが非常に楽だから
    {
        loadingPanel.SetActive(false);//ロードパネル非表示

        buttons.SetActive(false);//ボタン非表示

        createRoomPanel.SetActive(false);//ルーム作成パネル

        roomPanel.SetActive(false);//ルームパネル

        errorPanel.SetActive(false);//エラーパネル

        roomListPanel.SetActive(false);//ルーム一覧パネル

        nameInputPanel.SetActive(false);//名前入力パネル
    }



    //継承元のメソッドでは「virtual」のキーワード
    //継承先では「override」のキーワード
    /// <summary>
    /// クライアントがMaster Serverに接続されていて、マッチメイキングやその他のタスクを行う準備が整ったときに呼び出されます
    /// </summary>
    public override void OnConnectedToMaster()//
    {

        PhotonNetwork.JoinLobby();//マスターサーバー上で、デフォルトロビーに入ります

        loadingText.text = "ロビーへの参加...";//テキスト更新

        PhotonNetwork.AutomaticallySyncScene = true;//マスターと同じシーンに行くように設定
    }


    /// <summary>
    /// マスターサーバーのロビーに入るときに呼び出されます。
    /// </summary>
    public override void OnJoinedLobby()//
    {

        LobbyMenuDisplay();


        roomsList.Clear();//辞書の初期化


        PhotonNetwork.NickName = Random.Range(0, 1000).ToString();//ユーザーネームをとりあえず適当に決める


        ConfirmationName();//名前が入力されていればその名前を入力テキストに反映させる
    }


    //ロビーメニュー表示(エラーパネル閉じる時もこれ)
    public void LobbyMenuDisplay()
    {
        CloseMenuUI();
        buttons.SetActive(true);
    }

    //タイトルの部屋作成ボタン押下時に呼ぶ：UIから呼び出す
    public void OpenCreateRoomPanel()
    {
        CloseMenuUI();
        createRoomPanel.SetActive(true);
    }

    //部屋作成ボタン押下時に呼ぶ：UIから呼び出す
    public void CreateRoomButton()
    {
        //インプットフィールドのテキストに何か入力されていた場合
        if (!string.IsNullOrEmpty(enterRoomName.text))
        {
            //ルームのオプションをインスタンス化して変数に入れる 
            RoomOptions options = new RoomOptions();
            options.MaxPlayers = 6;// プレイヤーの最大参加人数の設定（無料版は20まで。1秒間にやり取りできるメッセージ数に限りがあるので10以上は難易度上がる）

            //ルームを作る(ルーム名：部屋の設定)
            PhotonNetwork.CreateRoom(enterRoomName.text, options);

            CloseMenuUI();//メニュー閉じる
            loadingText.text = "ルーム作成中...";
            loadingPanel.SetActive(true);
        }
    }


    //ルームに参加したら呼ばれる関数
    public override void OnJoinedRoom()
    {
        CloseMenuUI();//一旦すべてを閉じる
        roomPanel.SetActive(true);//ルームパネル表示

        roomName.text = PhotonNetwork.CurrentRoom.Name;//現在いるルームを取得し、テキストにルーム名を反映

        GetAllPlayer();//ルームに参加しているプレイヤーを表示

        CheckRoomMaster();//ルームマスターか判定する
    }




    //退出ボタン押下時に呼ばれる。参加中の部屋を抜ける
    public void LeavRoom()
    {
        //現在のルームを出て、マスターサーバーに戻って、ルームに参加したりルームを作成したりできます
        PhotonNetwork.LeaveRoom();

        CloseMenuUI();

        loadingText.text = "退出中・・・";
        loadingPanel.SetActive(true);
    }


    //ルームを抜けたときに呼ばれる
    public override void OnLeftRoom()
    {
        LobbyMenuDisplay();
    }

    //サーバーがルームを作成できなかったときに呼び出されます。
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "ルームの作成に失敗しました" + message;
        CloseMenuUI();
        errorPanel.SetActive(true);
    }


    //ルーム一覧画面を開く：ボタンから呼ぶ
    public void FindRoom()
    {
        CloseMenuUI();
        roomListPanel.SetActive(true);

    }


    //Master Serverのロビーにいる間に、ルームリストを更新するために呼び出されます。
    public override void OnRoomListUpdate(List<RoomInfo> roomList)//
    {
        RoomUIinitialization();//ルームUIの初期化

        UpdateRoomList(roomList);//ルーム情報を辞書に格納
    }

    //ルームの情報を辞書に
    public void UpdateRoomList(List<RoomInfo> roomList)
    {
        for (int i = 0; i < roomList.Count; i++)//ルームの数分ループ
        {
            RoomInfo info = roomList[i];//ルーム情報を変数に格納

            if (info.RemovedFromList)//ロビーで使用され、リストされなくなった部屋をマークします（満室、閉鎖、または非表示）
            {
                roomsList.Remove(info.Name);//辞書から削除
            }
            else
            {
                roomsList[info.Name] = info;//ルーム名をキーにして、辞書に追加
            }
        }

        RoomListDisplay(roomsList);//辞書にあるすべてのルームを表示
    }

    //ルーム表示
    void RoomListDisplay(Dictionary<string, RoomInfo> cachedRoomList)
    {
        //辞書のキー/値　でforeachを回す
        foreach (var roomInfo in cachedRoomList)
        {
            //ルームボタン作成
            Room newButton = Instantiate(originalRoomButton);
            //生成したボタンにルームの情報を設定
            newButton.RegisterRoomDetails(roomInfo.Value);
            //生成したボタンに親の設定
            newButton.transform.SetParent(roomButtonContent.transform);
            //リストに追加
            allRoomButtons.Add(newButton);
        }


    }

    //ルームボタンUI初期化
    void RoomUIinitialization()
    {
        foreach (Room rm in allRoomButtons)// ルームオブジェクトの数分ループ
        {
            Destroy(rm.gameObject);// ボタンオブジェクトを削除
        }

        allRoomButtons.Clear();//リスト要素削除

    }



    /// <summary>
    /// 引数のルームに入る関数
    /// </summary>
    /// <param name="roomInfo"></param>
    public void JoinRoom(RoomInfo roomInfo)
    {
        //引数のルームに参加する
        PhotonNetwork.JoinRoom(roomInfo.Name);

        CloseMenuUI();
        loadingText.text = "ルーム参加中";
        loadingPanel.SetActive(true);
    }



    /// <summary>
    /// ルームにいるプレイヤーを取得する関数
    /// </summary>
    public void GetAllPlayer()
    {
        //初期化
        InitializePlayerList();

        //プレイヤー表示
        PlayerDisplay();
    }


    //プレイヤー一覧初期化
    void InitializePlayerList()
    {
        //リストで管理している数分ループ
        foreach (var rm in allPlayerNames)
        {
            //text削除
            Destroy(rm.gameObject);
        }

        //リスト初期化
        allPlayerNames.Clear();

    }

    //ルームにいるプレイヤーを表示する
    void PlayerDisplay()
    {
        //ルームに接続しているプレイヤーの数分ループ
        foreach (var players in PhotonNetwork.PlayerList)
        {
            //テキストの生成
            PlayerTextGeneration(players);
        }
    }

    //プレイヤーテキスト生成
    void PlayerTextGeneration(Player players)
    {
        Text newPlayerText = Instantiate(playerNameText);//用意してあるテキストをベースにプレイヤーテキストを生成
        newPlayerText.text = players.NickName;//テキストに名前を反映
        newPlayerText.transform.SetParent(playerNameContent.transform);//親の設定


        allPlayerNames.Add(newPlayerText);//リストに追加
    }



    //名前の判定
    private void ConfirmationName()
    {
        if (!setName)//名前を入力していない場合
        {
            CloseMenuUI();

            nameInputPanel.SetActive(true);


            if (PlayerPrefs.HasKey("playerName"))//キーが保存されているか確認
            {

                placeholderText.text = PlayerPrefs.GetString("playerName");

                nameInput.text = PlayerPrefs.GetString("playerName");//インプットフィールドに名前を表示しておく



            }

        }
        else//一度入力したことがある場合は自動的に名前をセットする（ルーム入って戻った時とかいちいち表示されないように）
        {
            PhotonNetwork.NickName = PlayerPrefs.GetString("playerName");
        }
    }

    //名前保存や入力判定切り替え。ボタンから呼ぶ
    public void SetName()
    {
        if (!string.IsNullOrEmpty(nameInput.text))//入力されている場合
        {
            PhotonNetwork.NickName = nameInput.text;//ユーザー名に入力された名前を反映

            PlayerPrefs.SetString("playerName", nameInput.text);//名前を保存する

            LobbyMenuDisplay();//ロビーに戻る

            setName = true;//名前入力済み判定
        }
    }


    //プレイヤーがルームに入ったときに呼び出されます。
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //テキストを生成する
        PlayerTextGeneration(newPlayer);
    }


    //プレイヤーがルームを離れるか、非アクティブになったときに呼び出されます。
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //ルーム内にいるプレイヤーリストの更新して表示を正しくする
        GetAllPlayer();
    }


    //部屋のマスターか確認する
    private void CheckRoomMaster()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //マスターのみに開始ボタンを表示させる
            startButton.gameObject.SetActive(true);
        }
        else
        {
            startButton.gameObject.SetActive(false);
        }
    }

    //現在のMasterClientが終了したときに新しいMasterClientに切り替えた後に呼び出されます
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        //部屋のマスターか確認する
        if (PhotonNetwork.IsMasterClient)
        {
            //部屋のマスターになっていたら開始ボタンを表示する
            startButton.gameObject.SetActive(true);
        }

    }



    //開始ボタンで指定シーンに遷移する：UIから呼ぶ
    public void PlayGame()
    {
        //引数のステージを読み込む
        PhotonNetwork.LoadLevel(levelToPlay);

    }

}