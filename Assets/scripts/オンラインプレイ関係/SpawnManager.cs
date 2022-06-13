using UnityEngine;
using Photon.Pun;

public class SpawnManager : MonoBehaviour
{
    //スポーンポイントオブジェクト格納配列
    public Transform[] spawnPositons;
    public GameObject playerPrefab;     //生成オブジェクト
    private GameObject player;          //生成したプレイヤーを格納
    public float respawnInterval = 5f;  //復活までに要する時間


    private void Start()
    {
        //スポーンポイントオブジェクトをすべて非表示
        foreach (var pos in spawnPositons)
        {
            pos.gameObject.SetActive(false);
        }


        //ネットワークにつながっているならスポーン
        if (PhotonNetwork.IsConnected)
        {
            SpawnPlayer();
        }
    }

    /// <summary>
    /// リスポーン地点をランダム取得する関数
    /// </summary>
    public Transform GetSpawnPoint()
    {
        //ランダムでスポーンポイント１つ選んで位置情報を返す
        return spawnPositons[Random.Range(0, spawnPositons.Length)];
    }

    //Playerを生成する
    public void SpawnPlayer()
    {
        Transform spawnPoint = GetSpawnPoint();

        //ネットワーク上のプレイヤーを生成する：第一引数ではオブジェクトの名前が必要
        player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);//後で削除するときのために格納
    }
}