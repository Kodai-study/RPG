using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Tooltip("�ˌ��Ԋu")]
    public float shootInterval = .1f;
    [Tooltip("�З�")]
    public int shotDamage;
    [Tooltip("�`�����ݎ��̃Y�[��")]
    public float adsZoom;
    [Tooltip("�`�����ݎ��̑��x")]
    public float adsSpeed;

    public GameObject bulletImpact;
}