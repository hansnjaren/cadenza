using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using System.Collections.Generic;

public class Music : MonoBehaviour
{

    [SerializeField] private double bpm;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private double firstBeatOffset;

    [SerializeField] private BeatPattern[] customPatterns;
    [SerializeField] private BeatPattern normalPattern;
    [SerializeField] private BeatPattern syncopatedPattern;
    private BeatPattern enabledPattern;

    private double secsPerBeat;
    private double songPositionSecs;
    private double songStartTime;
    private double songPositionBeats;

    private BeatPattern[] allPatterns;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        musicSource.clip.LoadAudioData();

        secsPerBeat = 60.0 / bpm;

        double dspTime = AudioSettings.dspTime;

        songStartTime = dspTime + 1.0;

        musicSource.PlayScheduled(songStartTime);

        if ((GameManager.Instance.GetActiveEffects() & Effects.Syncopation) == Effects.Syncopation)
        {
            enabledPattern = syncopatedPattern;
        }
        else
        {
            enabledPattern = normalPattern;
        }
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

        foreach (var pattern in customPatterns.Append(enabledPattern))
        {

            if (songPositionBeats > pattern.nextOccurTime)
            {
                pattern.beatsEnumerator.MoveNext();
                if (pattern.beatsEnumerator.Current is (int beatNo, double nextOffset))
                {
                    pattern.nextOccurTime += nextOffset;
                    pattern.beatEvent.Invoke(beatNo);
                }
            }
        }
    }

    public double GetMeasurePosition()
    {
        return (AudioSettings.dspTime - songStartTime - firstBeatOffset) / secsPerBeat % 1.0;
    }

    public void OnValidate()
    {
        foreach (var pattern in customPatterns.Append(normalPattern).Append(syncopatedPattern))
        {
            pattern.nextOccurTime = pattern.initialOffset;
            pattern.beatsEnumerator = Enumerable.Zip(EnumerableExtensions.Cycle(Enumerable.Range(0, pattern.beats.Length)), EnumerableExtensions.Cycle(pattern.beats), (a, b) => (a, b)).GetEnumerator();
        }

        // TODO: Should we force normal and syncopated pattern to have the same listeners?
    }
}
