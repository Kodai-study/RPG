using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Enemy[] enemies;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            foreach (Enemy e in enemies)
            {
                e.gameObject.SetActive(true);
                e.HP = e.MAX_HP;
            }
        }
    }
}
