using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damageEffect : MonoBehaviour
{
    // Start is called before the first frame update

    public AnimationCurve colorChangeCurve;
    public Color startColor;
    public Color endColor;
    private TextMesh mycolor;
    public Animator animator;

    [Range(0f, 1f)]
    public float t;

    bool b = false;

    void Start()
    {
        mycolor = GetComponent<TextMesh>();
       // animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
            animator.SetBool("damage", b = !b);
        mycolor.color = Color.Lerp(startColor, endColor, colorChangeCurve.Evaluate(t));
    }
}
