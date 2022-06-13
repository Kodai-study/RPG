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
    public AnimationClip attackAnimation;
    private float effectTime;
    private float totalDamage;

    [Range(0f, 1f)]
    public float t;

    void Start()
    {
        damageView = GetComponentInChildren<TextMesh>();
        effectTime = attackAnimation.length;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            animator.SetTrigger(AnimationParams.GET_DAMAGE_TRIGGER_NAME);
        }
        damageView.color = Color.Lerp(startColor, endColor, colorChangeCurve.Evaluate(t));
    }

    public void damage(float damageValue, bool keepHit)
    {
        if (!keepHit && damageValue > 0)
        {
            totalDamage = damageValue;
        }
        else
        {
            totalDamage += damageValue;
        }
        damageView.text = ((int)totalDamage).ToString();
        if (damageValue > 0)
        {
            animator.SetTrigger(AnimationParams.GET_DAMAGE_TRIGGER_NAME);
        }
        animator.SetBool(AnimationParams.DAMAGE_FLAG_NAME, keepHit);
        Debug.Log(keepHit);
    }

    public float EffectLength
    {
        get { return effectTime; }
    }
}
