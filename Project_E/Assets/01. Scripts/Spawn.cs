using UnityEngine;



public class Spawn : MonoBehaviour
{
    public GameObject targetPrefab;
    public Transform spawnPosition;

    public void SpawnTarget()
    {
             Instantiate(targetPrefab, transform.position, Quaternion.identity);
        
    }

}