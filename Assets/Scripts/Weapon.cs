using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using TMPro;

public class Weapon : MonoBehaviour
{
    [Header("Aiming")]
    public Vector3 aimPosition;
    public Vector3 aimRotation;
    public float aimSpeed = 10f;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    public bool IsAiming { get; private set; }

    public enum BulletType
    {
        Normal,
        DelayedDestroy
    };
    public BulletType currentBulletType;

    [Header("Bullet settings")]
    public GameObject normalBulletPrefab;
    public GameObject delayedBulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30;
    public float bulletPrefabLifeTime = 3f;

    public Camera playerCamera;

    [Header("UI")]
    public TextMeshProUGUI ammoText;

    public bool isShooting, readyToShoot;
    bool allowReset = true;
    public float shootingDelay = 2f;

    public int bulletsPerBurst = 3;
    public int burstBulletsLeft;

    public int ammoInMagazine = 7;
    public int ammoLeftInMagazine = 7;
    public int ammoLeft = 30;

    public float spreadIntensity;

    public enum ShootingMode
    {
        Single,
        Burst,
        Auto
    };

    public ShootingMode currentShootingMode;

    private void Awake()
    {
        readyToShoot = true;
        burstBulletsLeft = bulletsPerBurst;
    }

    private void Start()
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;

        UpdateAmmoUI();
    }

    private void Update()
    {
        HandleAiming();

        if(Input.GetKeyDown(KeyCode.Q))
        {
            SwitchBulletType();
        }

        if(currentShootingMode == ShootingMode.Auto)
        {
            isShooting = Input.GetKey(KeyCode.Mouse0);
        }
        else if(currentShootingMode == ShootingMode.Single || currentShootingMode == ShootingMode.Burst)
        {
            isShooting = Input.GetKeyDown(KeyCode.Mouse0);
        }

        if(readyToShoot && isShooting && ammoLeftInMagazine > 0)
        {
            burstBulletsLeft = bulletsPerBurst;
            FireWeapon();
            ammoLeftInMagazine--;
            UpdateAmmoUI();
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            if(ammoLeft > 0)
            {
                SoundManager.Instance.reloadSound.Play();
            }
            Invoke("reloadWeapon", 2.378f);
        }
    }

    private void reloadWeapon()
    {

        if(ammoLeft > 0)
        {
            if(ammoLeftInMagazine > 0 && ammoLeftInMagazine < 7)
            {
                if(ammoLeft < (ammoInMagazine - ammoLeftInMagazine))
                {
                    ammoLeftInMagazine += ammoLeft;
                    ammoLeft = 0;
                }
                else
                {
                    ammoLeft -= (ammoInMagazine - ammoLeftInMagazine);
                    ammoLeftInMagazine = ammoInMagazine;
                }
            }
            else
            {
                if(ammoLeft >= 7)
                {
                    ammoLeftInMagazine = ammoInMagazine;
                    ammoLeft -= ammoInMagazine;
                }
                else
                {
                    ammoLeftInMagazine += ammoLeft;
                    ammoLeft = 0;
                }
            }
        }
        UpdateAmmoUI();
    }

    public void UpdateAmmoUI()
    {
        if(ammoText != null)
        {
            ammoText.text = ammoLeftInMagazine + " / " + ammoLeft;
        }
    }

    private void SwitchBulletType()
    {
        if(currentBulletType == BulletType.Normal)
        {
            currentBulletType = BulletType.DelayedDestroy;
            print("Zmieniono na pociski o op�nionym usuni�ciu");
        }
        else
        {
            currentBulletType = BulletType.Normal;
            print("Zmieniono na normalne przyciski");
        }
    }

    private void FireWeapon()
    {
        readyToShoot = false;

        SoundManager.Instance.shootingSound.Play();

        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;


        // Stw�rz pocisk
        //GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

        GameObject bulletToFire;

        if(currentBulletType == BulletType.Normal)
        {
            bulletToFire = normalBulletPrefab;
        }
        else
        {
            bulletToFire = delayedBulletPrefab;
        }

        GameObject bullet = Instantiate(bulletToFire, bulletSpawn.position, Quaternion.identity);

        // Skierowanie pocisku w strone strza�u
        bullet.transform.forward = shootingDirection;
        // Wystrzel pocisk
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection *  bulletVelocity, ForceMode.Impulse);
        // Usu� pocisk
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));

        if(allowReset)
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }

        // BurstMode
        if(currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1) // wi�cej ni� jeden, bo ju� jeden pocisk by� wystrzelony
        {
            burstBulletsLeft--;
            Invoke("FireWeapon", shootingDelay);
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }

    public Vector3 CalculateDirectionAndSpread()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if(Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(100);
        }

        Vector3 direction = targetPoint - bulletSpawn.position;

        float x = UnityEngine.Random.Range(-spreadIntensity,spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        // Zwraca kierunek strza�u i rozrzut
        return direction + new Vector3(x, y, 0);
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }

    private void HandleAiming()
    {
        IsAiming = Input.GetMouseButton(1);

        Vector3 targetPosition = IsAiming ? aimPosition : originalPosition;
        Quaternion targetRotation = IsAiming ? Quaternion.Euler(aimRotation) : originalRotation;

        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * aimSpeed);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * aimSpeed);
    }
}