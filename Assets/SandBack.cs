using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandBack : Enemy
{
    public string damegedTagName;



    private TextMesh HPViewr;

    // Start is called before the first frame update
    void Start()
    {
      
        HPViewr = GetComponentInChildren<TextMesh>();  
        HP = MAX_HP;
        
        if(HPViewr != null)
        {
            Debug.Log("");
        }

       // charaAnimator = 
    }

    // Update is called once per frame
    void Update()
    {
        hitCount += Time.deltaTime;
    }


    public override int HP
    {
        set
        {
            base.HP = value;
            HPViewr.text = value.ToString();
        }
        get { return hp; }
    }

}
