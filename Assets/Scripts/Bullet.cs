using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] public float bulletSpeed, despawnTime;
    public Rigidbody rbBullet;
    // Start is called before the first frame update
    void Start()
    {
        rbBullet.AddForce(transform.forward * bulletSpeed * Time.deltaTime, ForceMode.Impulse);
        StartCoroutine(deleteBullet());
    }
    IEnumerator deleteBullet()
    {
        yield return new WaitForSeconds(despawnTime);
        Destroy(this.gameObject);
    }

}
