using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstValue;
//import (パッケージ名).(作ったクラス名)

public class EffectDamage : MonoBehaviour
{

    public AnimationCurve colorChangeCurve; //エフェクト時に、色がどんなふうに遷移していくかを表したカーブ(0～1)
    public Color startColor;                //ダメージエフェクト開始時の、透明になっている色
    public Color endColor;                  //ダメージエフェクト終わりの、透明度が0の色
    private TextMesh damageView;            //子オブジェクト、ダメージ数値を表示する3Dテキスト
    public Animator animator;

    [Tooltip("ダメージ表記のはじめ")]
    public AnimationClip startAnimation;
    [Tooltip("ダメージ表記が消えていくアニメーション")]
    public AnimationClip endAnimation;
    private float effectTime = 0;           //コンボ判定になる時間(アニメーションの長さに設定)
    private float totalDamage;          //表記されるダメージ数。コンボだと加算されていく
    private float timeCount = 0;        //前回当たり判定があってからの時間を測るカウンタ
    private bool ISCombo = false;
    [Range(0f, 1f)]
    public float t;

    void Start()
    {
        damageView = GetComponentInChildren<TextMesh>();
        effectTime = startAnimation.length + endAnimation.length;
    }

    void Update()
    {
        //Tの値が横軸となり、カーブの縦軸の値だけ色がendColorに近づく
        damageView.color = Color.Lerp(startColor, endColor, colorChangeCurve.Evaluate(t));
        timeCount += Time.deltaTime;        //常にカウント状態

        /* ダメージ表記のアニメーションが終わったら、コンボ状態を解除してエフェクトも消滅 */
        if (timeCount > startAnimation.length)
        {
            ISCombo = false;
            animator.SetBool(AnimationParams.DAMAGE_FLAG_NAME, false);
        }
    }

    /* ダメージを受けたときに呼ばれ、エフェクト処理をする関数 */
    public void damage(float damageValue)
    {
        if (damageValue < 0)
            return;

        /* 前回の当たり判定から、ダメージエフェクトが終了するまでに新たにダメージを受けたら、コンボ判定 */
        if (timeCount < effectTime)
        {
            animator.SetBool(AnimationParams.DAMAGE_FLAG_NAME, ISCombo = true);
        }

        timeCount = 0;

        /* 始めて当たった */
        if (!ISCombo)
        {
            ISCombo = true;
            totalDamage = damageValue;
        }
        /* コンボ状態では表記ダメージが加算される */
        else
        {
            totalDamage += damageValue;
        }
        damageView.text = ((int)totalDamage).ToString();
        animator.SetTrigger(AnimationParams.GET_DAMAGE_TRIGGER_NAME);
    }

    public float EffectLength
    {
        get { return effectTime; }
    }
}
