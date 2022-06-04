using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TatolEnemy : Enemy
{
    // Start is called before the first frame update

    NavMeshAgent navigation;
    public Transform chacePlayer;
    public float a;
    public float z;
    public float groundCheckLength;
    private float msCount = 0;
    private bool isNockBack = false;

    void Start()
    {
        HP = MAX_HP;
        navigation = GetComponent<NavMeshAgent>();
        Debug.Log(HP);
    }

    // Update is called once per frame
    void Update()
    {
        if(navigation != null && navigation.isActiveAndEnabled)
            navigation.destination = chacePlayer.position;

        if (Input.GetKeyDown(KeyCode.A))
        {
            NockBack(chacePlayer);
        }

        if (isNockBack && IsGround())
        {
            isNockBack = false;
            navigation.enabled = true;
            msCount = 0;
        }

    }

    public override void Dameged(float damage)
    {
        base.Dameged(damage);
        NockBack(oldCharaAnimator.gameObject.transform);
    }

    private void NockBack(Transform attackedPlayer)
    {

        Camera camera = attackedPlayer.GetComponentInChildren<Camera>();
        //Ray ray = camera.ViewportPointToRay(new Vector2(.5f, .5f));
        Ray ray = camera.ViewportPointToRay(new Vector3(.5f,0f,0.5f));
        Debug.DrawRay(ray.origin,ray.direction);

        Vector3 NockBackDirection = ray.direction;
        NockBackDirection.y = z;
        NockBackDirection *= a;
        GetComponent<Rigidbody>().AddForce(NockBackDirection,ForceMode.VelocityChange);

        isNockBack = true;
        navigation.enabled = false;

        Debug.Log(NockBackDirection);
    }

    private bool IsGround()
    {
        msCount += Time.deltaTime;
        if (msCount < 0.1)
            return false;
        return Physics.Raycast(transform.position, Vector3.down, groundCheckLength, groundLayer);
    }


}
