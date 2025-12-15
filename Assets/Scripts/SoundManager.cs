using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }

    [Header("Weapon sounds")]
    public AudioSource shootingSound;
    public AudioSource reloadSound;

    [Header("Pickup sounds")]
    public AudioSource ammoPickupSound;
    public AudioSource healthPickupSound;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
}
