using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

[System.Serializable]
public class BeatPattern
{
    public string patternName = "New Pattern";
    public double[] beats;
    public UnityEvent<int> beatEvent;
    public double initialOffset;

    // TODO: Having to initialize these violates SRP.
    [HideInInspector]
    public double nextOccurTime;
    [HideInInspector]
    public IEnumerator<(int, double)> beatsEnumerator;
}
