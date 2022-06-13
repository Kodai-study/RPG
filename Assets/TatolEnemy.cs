using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TatolEnemy : Enemy
{
    NavMeshAgent navigation;        //敵の移動(プレイヤーに向かう)を司るAI
    public Transform chacePlayer;   //追いかけるプレイヤーの位置情報(デバッグ用にプレイヤーをインスペクタで入力)
    public float nockBackLength;    //ノックバックで与える加速度の係数(カメラの向いている方向に与えられる)
    public float nockBack_Y;        //ノックバックで与えられる加速度の、Y軸(高さ軸)の係数
    public float groundCheckLength; //中心からどれくらい下に地面があれば地面にいる判定になるかの値
    private float secCount = 0;     //ノックバック状態からの復帰で使う、時間経過のカウンター
    private bool isNockBack = false; //ノックバック状態化を判定
    public float nockBackDelay;     //ノックバックが始まってから、地面判定がストップする時間
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
        /* ナビゲーションが有効なら、自分が向かう場所をプレイヤーの場所にする */
        if (navigation != null && navigation.isActiveAndEnabled)
            navigation.destination = chacePlayer.position;

        /* (デバッグ用)Aボタンでノックバック */
        if (Input.GetKeyDown(KeyCode.K))
            NockBack(chacePlayer);

        /* ノックバック状態で、地面に再び着いた時の処理 */
        if (isNockBack)
        {
            if (secCount >= nockBackDelay && IsGround())
            {
                isNockBack = false;         //ノックバック状態の解除
                navigation.enabled = true;  //ナビゲーションを再開
                secCount = 0;
            }
            else
                secCount += Time.deltaTime; //ノックバックが始まってからの時間をカウント(一定時間で地面判定の復活)
        }
    }


    /* ダメージを受けたときの処理 */
    public override void Dameged(float damage)
    {
        base.Dameged(damage);
        NockBack(oldCharaAnimator.gameObject.transform); //とりあえずでダメージ等によらず同じノックバック処理
    }


    /* ダメージを受けたときのノックバック処理 */
    private void NockBack(Transform attackedPlayer)
    {
        Camera camera;                          //プレイヤーが持っているカメラ
        Vector3 NockBackDirection;              //ノックバックで与えられる加速度
        camera = attackedPlayer.GetComponentInChildren<Camera>();   //攻撃してきたプレイヤーのカメラを取得
        /** TODO 攻撃してきたオブジェクトがプレイヤーかどうかを判定 */
        Ray ray = camera.ViewportPointToRay(new Vector3(1f, 0f, 1f));   //攻撃プレイヤーのカメラの向いている方向を取得
        Debug.DrawRay(ray.origin,ray.direction);                    ///Debug  

        NockBackDirection = ray.direction;                          //カメラが向いている方向を3軸で取得
        NockBackDirection.y = nockBack_Y;                           //ノックバックする高さを、クラス変数に設定
        NockBackDirection *= (nockBackLength * size);                        //ノックバックで与える
        GetComponent<Rigidbody>().AddForce(NockBackDirection,ForceMode.VelocityChange); //計算した加速度を与えてノックバック

        isNockBack = true;                              //ノックバック状態
        navigation.enabled = false;                     //ノックバック状態ではナビゲーションオフ(上方向にいけないため)
    }

    private bool IsGround()
    {
        /* 自分の中心から下方向を見て、変数値より近い部分に地面があったら地面に触れていたと判定 */
        return Physics.Raycast(transform.position, Vector3.down, groundCheckLength * size, groundLayer);
    }

    protected override void Die()
    {
        

    }

}
