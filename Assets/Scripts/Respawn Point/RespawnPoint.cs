using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    public Vector2 respawnPosition;

    private void Start()
    {
        PlayerLifeManager.Instance.SetStartPos(respawnPosition);
    }
}
