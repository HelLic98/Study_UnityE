using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    [Header("Player Prefab (with Rigidbody)")]
    public GameObject playerPrefab;

    [Header("Spawn Point")]
    public Transform spawnPoint;

    [Tooltip("If true, any existing spawned player will be destroyed before spawning a new one.")]
    public bool destroyExisting = true;

    GameObject currentPlayer;

    void Start()
    {
        SpawnPlayer();
    }

    // Spawns the player prefab at the assigned spawn point.
    public void SpawnPlayer()
    {
        if (playerPrefab == null)
        {
            Debug.LogWarning("[PlayerSpawn] playerPrefab is not assigned.");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogWarning("[PlayerSpawn] spawnPoint is not assigned.");
            return;
        }

        if (destroyExisting && currentPlayer != null)
        {
            Destroy(currentPlayer);
            currentPlayer = null;
        }

        currentPlayer = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        currentPlayer.name = "Player";
        try { currentPlayer.tag = "Player"; } catch { }

        var rb = currentPlayer.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning("[PlayerSpawn] spawned prefab does not have a Rigidbody component.");
        }
    }

    // Convenience method to respawn from other scripts or UI buttons.
    public void Respawn()
    {
        SpawnPlayer();
    }
}
