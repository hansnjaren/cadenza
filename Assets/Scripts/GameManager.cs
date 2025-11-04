using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Effects ActiveEffects = Effects.None;

    private void Awake()
    {
        // This is the core singleton logic
        if (Instance == null)
        {
            // If this is the first instance, set it to this object
            Instance = this;

            // Optional: Keep this object alive when loading new scenes
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            // If an instance already exists, and it's not this one,
            // destroy this new, duplicate object.
            Destroy(gameObject);
        }
    }
}
