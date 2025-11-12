using UnityEngine;
using UnityEngine.Events;

public class Music : MonoBehaviour
{

    [SerializeField] private double bpm;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private double firstBeatOffset;

    [SerializeField] private UnityEvent<int> onBeat;

    private double secsPerBeat;
    private double songPositionSecs;
    private double soneStartTime;
    private double songPositionBeats;
    private double songStartTime;
    private double nextBeatTime;

    private int beatNo = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        musicSource.clip.LoadAudioData();

        secsPerBeat = 60.0 / bpm;

        double dspTime = AudioSettings.dspTime;

        songStartTime = dspTime + 1.0;
        nextBeatTime = 0.0;

        musicSource.PlayScheduled(songStartTime);
    }

    // Update is called once per frame
    void Update()
    {
        songPositionSecs = AudioSettings.dspTime - songStartTime - firstBeatOffset;

        if (songPositionSecs < 0)
        {
            return;
        }

        songPositionBeats = songPositionSecs / secsPerBeat;

        if (songPositionBeats > nextBeatTime)
        {
            nextBeatTime += 1.0;
            onBeat.Invoke(beatNo);

            beatNo = (beatNo + 1) % 4;
        }
    }

    public void onBeatTest(int beat)
    {
        Debug.Log($"Asdf: {beat}");
    }
}
