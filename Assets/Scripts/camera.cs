using UnityEngine;

public class camera : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float maxXPanning;
    [SerializeField] private float maxYPanning;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraPos = cameraTransform.position;
        cameraPos.x = Mathf.Clamp(cameraTransform.position.x - playerTransform.position.x, -maxXPanning, maxXPanning) + playerTransform.position.x;
        cameraPos.y = Mathf.Clamp(cameraTransform.position.y - playerTransform.position.y, -maxYPanning, maxYPanning) + playerTransform.position.y;

        cameraTransform.position = cameraPos;
    }
}
