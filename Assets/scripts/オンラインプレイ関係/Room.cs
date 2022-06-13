using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;

public class Room : MonoBehaviour
{
    public Text buttonText;//ルーム名反映用のテキスト
    private RoomInfo info;// 部屋の情報を格納する変数

    /// <summary>
    /// ルームボタンに詳細を登録
    /// </summary>
    public void RegisterRoomDetails(RoomInfo info)
    {
        this.info = info;//ルームのinfoに引数のinfoを格納

        buttonText.text = this.info.Name;//テキスト更新
    }


    //このルームボタンが管理しているルームに参加する：ボタンUIから呼び出す
    public void OpenRoom()
    {
        //ルームに参加関数を呼び出す
        PhotonManager.instance.JoinRoom(info);
    }
}