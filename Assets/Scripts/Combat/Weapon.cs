using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObjects/Weapon")]
public class Weapon : ScriptableObject
{
    [Header("Movement")]
    public float acceleration = 10.0f;
    public float bodyLookSpeed = 1.0f;

    [Header("Firing")]
    public float fireDelay = 0.1f;
    public float fireForce = 500.0f;
    public float bulletDrag = 0;
    public float recoil = 0.1f;
    public float damage = 50f;
    public float range = 1000f;
    public int maxAmmo = 10;
    public GameObject spread;
    public bool fireFastAsTrigger = false;
    public void Equip() {
        var player = FindObjectOfType<PlayerController>();
        player.acceleration = acceleration;
        player.bodyLookSpeed = bodyLookSpeed;
        player.fireDelay = fireDelay;
        player.fireForce = fireForce;
        player.bulletDrag = bulletDrag;
        player.recoil = recoil;
        player.damage = damage;
        player.maxAmmo = maxAmmo;
        player.ammo = maxAmmo;
        player.spread = spread;
        player.fireFastAsTrigger = fireFastAsTrigger;
        player.range = range;
    }
}
