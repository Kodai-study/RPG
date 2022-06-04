using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Tooltip("ËŒ‚ŠÔŠu")]
    public float shootInterval = .1f;
    [Tooltip("ˆĞ—Í")]
    public int shotDamage;
    [Tooltip("”`‚«‚İ‚ÌƒY[ƒ€")]
    public float adsZoom;
    [Tooltip("”`‚«‚İ‚Ì‘¬“x")]
    public float adsSpeed;

    public GameObject bulletImpact;
}