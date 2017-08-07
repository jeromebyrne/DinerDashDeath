using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private const float kTimeBetweenSongs = 10.0f;

    private int m_CurrentLevelMusicIndex = -1;

    public AudioClip[] m_LevelMusic;

    private AudioSource m_AudioSourceCurrentMusic;

    private float currentTimeBetweenSongs = kTimeBetweenSongs;

    private int currentLoopIndex = 0;

    // Use this for initialization
    void Start()
    {
        m_AudioSourceCurrentMusic = gameObject.AddComponent<AudioSource>();

        m_CurrentLevelMusicIndex = Random.Range(0, m_LevelMusic.Length - 1);

        PlayNextLevelMusic();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_AudioSourceCurrentMusic)
        {
            if (!m_AudioSourceCurrentMusic.isPlaying)
            {
                if (currentLoopIndex < 2)
                {
                    currentLoopIndex++;

                    m_AudioSourceCurrentMusic.Play();
                }
                else
                {
                    currentTimeBetweenSongs -= Time.deltaTime;

                    if (currentTimeBetweenSongs <= 0.0f)
                    {
                        PlayNextLevelMusic();
                    }
                }
            }
            else
            {
                float currentTimeScale = TimeManager.GetInstance().GetCurrentTimescale();

                if (m_AudioSourceCurrentMusic.pitch != currentTimeScale)
                {
                    m_AudioSourceCurrentMusic.pitch = currentTimeScale;
                }
            }
        }
    }

    public void PlayNextLevelMusic()
    {
        ++m_CurrentLevelMusicIndex;
        currentLoopIndex = 0;

        if (m_CurrentLevelMusicIndex > m_LevelMusic.Length - 1)
        {
            m_CurrentLevelMusicIndex = 0;
        }

        if (m_AudioSourceCurrentMusic)
        {
            m_AudioSourceCurrentMusic.Stop();

            m_AudioSourceCurrentMusic.clip = m_LevelMusic[m_CurrentLevelMusicIndex];

            m_AudioSourceCurrentMusic.Play();
        }

        currentTimeBetweenSongs = kTimeBetweenSongs;
    }
}
