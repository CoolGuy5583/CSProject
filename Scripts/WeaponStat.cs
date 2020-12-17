using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStat : MonoBehaviour
{
    public int ROF;
    public float hitForce;
    public float range;
    public float accuracy;
    public float scopedAccuracyFactor;
    public float damage;
    public ParticleSystem muzzleFlash;
    public ParticleSystem bulletEffect;
    public AudioSource shot;
    public int ammo;
    public int ammoPerClip;
    public string Name;
    public Sprite img;
}
