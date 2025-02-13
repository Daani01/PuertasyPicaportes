using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayer : MonoBehaviour, IProcess
{
    public bool IsCompleted { get; private set; } = false;
    public GameObject playerPrefab;
    private FirstPersonController currentPlayer;

    public void ExecuteProcess(System.Action onComplete)
    {
        StartCoroutine(ProcessRoutine(onComplete));
    }

    private IEnumerator ProcessRoutine(System.Action onComplete)
    {

        yield return StartCoroutine(Spawn());

        IsCompleted = true;

        onComplete?.Invoke();
    }

    private IEnumerator Spawn()
    {
        if (currentPlayer != null)
        {
            Destroy(currentPlayer.gameObject);
        }

        GameObject playerInstance = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

        yield return null;
    }

}
