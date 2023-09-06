using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Takip : MonoBehaviour
{
    public Transform target; // Oyuncunun pozisyonunu takip edecek hedef
    public float moveSpeed = 5f; // D��man�n hareket h�z�
    public float detectionRange = 10f; // D��man�n oyuncuyu alg�lama mesafesi
    public float detectionAngle = 60f; // Alg�lama a��s� (derece)
    public string obstacleTag = "Obstacle"; // Engellerin tag ad�
    public GameObject cubePrefab; // F�rlat�lacak k�p prefab'�
    public float throwForce = 10f; // K�p�n f�rlatma g�c�
    public float shootingRange = 5f; // Ate� etme mesafesi
    public float throwInterval = 2f; // F�rlatma aral��� (saniye)
    public float cubeLifetime = 5f; // F�rlat�lan k�p�n �mr� (saniye)

    private float lastThrowTime;

    private void Update()
    {
        if (target == null)
        {
            Debug.LogError("Hedef (oyuncu) atanmam��!");
            return;
        }

        // Oyuncu ile d��man aras�ndaki mesafeyi ve a��y� hesapla
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        float angleToTarget = Vector3.Angle(transform.forward, target.position - transform.position);

        if (distanceToTarget <= detectionRange && angleToTarget <= detectionAngle / 2)
        {
            // Engel kontrol�
            RaycastHit hit;
            if (Physics.Raycast(transform.position, target.position - transform.position, out hit, detectionRange))
            {
                if (hit.collider.CompareTag(obstacleTag))
                {
                    // Engelden dolay� g�r�� engellendi
                    return;
                }
            }

            // Hedefe do�ru y�nelme ve hareket
            Vector3 direction = target.position - transform.position;
            direction.Normalize();
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

            // K�p f�rlatma kontrol�
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

            Destroy(cube, cubeLifetime); // K�p� belirledi�iniz s�re sonra yok et
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Alg�lama mesafesini g�rselle�tirme
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Alg�lama a��s�n� g�rselle�tirme
        Gizmos.color = Color.green;
        Vector3 rightDirection = Quaternion.Euler(0, detectionAngle / 2, 0) * transform.forward;
        Vector3 leftDirection = Quaternion.Euler(0, -detectionAngle / 2, 0) * transform.forward;
        Gizmos.DrawRay(transform.position, rightDirection * detectionRange);
        Gizmos.DrawRay(transform.position, leftDirection * detectionRange);

        // Ate� etme mesafesini g�rselle�tirme
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shootingRange);
    }
}
