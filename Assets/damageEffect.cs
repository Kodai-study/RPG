using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEffect : MonoBehaviour
{

    public AnimationCurve colorChangeCurve;  //�G�t�F�N�g���ɁA�F���ǂ�Ȃӂ���
    public Color startColor;
    public Color endColor;
    private TextMesh mycolor;
    public Animator animator;
    public AnimationClip attackAnimation;
    private float effectTime;
    private float totalDamage;

    [Range(0f, 1f)]
    public float t;

    bool isEffeect = false; //���݃_���[�W���̃G�t�F�N�g�����s���Ă��邩

    void Start()
    {
        mycolor = GetComponentInChildren<TextMesh>();
        effectTime = attackAnimation.length;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
           // animator.SetBool("damage", isEffeect = !isEffeect);
            animator.SetTrigger("getDamage");
        }
        mycolor.color = Color.Lerp(startColor, endColor, colorChangeCurve.Evaluate(t));
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
        mycolor.text = ((int)totalDamage).ToString();
        if (damageValue > 0)
            animator.SetTrigger("getDamage");
        animator.SetBool("damage", keepHit);
        Debug.Log(keepHit);
    }

    public float EffectLength
    {
        get { return effectTime; }
    }
}
