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
            Debug.Log(HPViewr.name);
        }
    }

    // Update is called once per frame
    override protected void Update()
    {
        base.Update();
    }


    public override int HP
    {
        set
        {
            base.HP = value;
            HPViewr.text = value.ToString();
        }
        get { return base.hp; }
    }


    ///Debug 

    /*override protected void OnTriggerEnter(Collider other) {
        base.OnTriggerEnter(other);
        Debug.Log(HP);
    }

    public override void Dameged(float damage)
    {
        base.Dameged(damage);
    }*/

    protected override void Die()
    {
        base.Die();
        gameObject.SetActive(false);
    }

}
