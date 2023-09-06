using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Takip : MonoBehaviour
{
    public Transform target; // Oyuncunun pozisyonunu takip edecek hedef
    public float moveSpeed = 5f; // Düþmanýn hareket hýzý
    public float detectionRange = 10f; // Düþmanýn oyuncuyu algýlama mesafesi
    public float detectionAngle = 60f; // Algýlama açýsý (derece)
    public string obstacleTag = "Obstacle"; // Engellerin tag adý
    public GameObject cubePrefab; // Fýrlatýlacak küp prefab'ý
    public float throwForce = 10f; // Küpün fýrlatma gücü
    public float shootingRange = 5f; // Ateþ etme mesafesi
    public float throwInterval = 2f; // Fýrlatma aralýðý (saniye)
    public float cubeLifetime = 5f; // Fýrlatýlan küpün ömrü (saniye)

    private float lastThrowTime;

    private void Update()
    {
        if (target == null)
        {
            Debug.LogError("Hedef (oyuncu) atanmamýþ!");
            return;
        }

        // Oyuncu ile düþman arasýndaki mesafeyi ve açýyý hesapla
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        float angleToTarget = Vector3.Angle(transform.forward, target.position - transform.position);

        if (distanceToTarget <= detectionRange && angleToTarget <= detectionAngle / 2)
        {
            // Engel kontrolü
            RaycastHit hit;
            if (Physics.Raycast(transform.position, target.position - transform.position, out hit, detectionRange))
            {
                if (hit.collider.CompareTag(obstacleTag))
                {
                    // Engelden dolayý görüþ engellendi
                    return;
                }
            }

            // Hedefe doðru yönelme ve hareket
            Vector3 direction = target.position - transform.position;
            direction.Normalize();
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

            // Küp fýrlatma kontrolü
            if (distanceToTarget <= shootingRange && Time.time - lastThrowTime >= throwInterval)
            {
                ThrowCube();
                lastThrowTime = Time.time;
            }
        }
    }

    private void ThrowCube()
    {
        if (cubePrefab != null)
        {
            GameObject cube = Instantiate(cubePrefab, transform.position + transform.forward, Quaternion.identity);
            Rigidbody cubeRb = cube.GetComponent<Rigidbody>();
            if (cubeRb != null)
            {
                Vector3 throwDirection = target.position - transform.position;
                throwDirection.Normalize();
                cubeRb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
            }

            Destroy(cube, cubeLifetime); // Küpü belirlediðiniz süre sonra yok et
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Algýlama mesafesini görselleþtirme
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Algýlama açýsýný görselleþtirme
        Gizmos.color = Color.green;
        Vector3 rightDirection = Quaternion.Euler(0, detectionAngle / 2, 0) * transform.forward;
        Vector3 leftDirection = Quaternion.Euler(0, -detectionAngle / 2, 0) * transform.forward;
        Gizmos.DrawRay(transform.position, rightDirection * detectionRange);
        Gizmos.DrawRay(transform.position, leftDirection * detectionRange);

        // Ateþ etme mesafesini görselleþtirme
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shootingRange);
    }
}
