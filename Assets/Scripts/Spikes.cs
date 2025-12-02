using UnityEngine;

public class Spikes : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.gameObject.TryGetComponent<Player>(out Player player))
        {
            GameManager.Instance.Kill(player);
        }
    }
}
