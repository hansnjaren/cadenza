using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private bool isActive = true;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActive && collision.gameObject.CompareTag("Player") && collision.gameObject.TryGetComponent<Player>(out Player player))
        {
            GameManager.Instance.UpdateCheckpointPos(transform.position);
            GetComponent<SpriteRenderer>().color = Color.red;
            isActive = false;
        }
    }
}
