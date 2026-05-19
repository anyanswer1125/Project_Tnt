using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    // 자주 사용하는 클립을 캐싱해두면 성능에 좋습니다 (중복 로드 방지)
    private Dictionary<string, AudioClip> clipCache = new Dictionary<string, AudioClip>();

    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioMixer audioMixer; // 오디오 믹서 (볼륨 조절용)

    private const string BGM_VOLUME_PARAM = "BGMVolume";
    private const string SFX_VOLUME_PARAM = "SFXVolume";

    private bool bgmValueSetting = false; // BGM 볼륨 설정이 완료되었는지 여부
    private bool sfxValueSetting = false; // SFX 볼륨 설정이 완료되었는지 여부

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }

        Initialize();
    }
    private void Initialize()
    {
        // 내 오브젝트에 붙은 오디오 소스를 가져옴
        var sources = GetComponents<AudioSource>();
        audioMixer = Resources.Load<AudioMixer>("AudioMixer");
        if (sources.Length >= 2)
        {
            bgmSource = sources[0];
            bgmSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups(BGM_VOLUME_PARAM)[0]; // BGM 믹서 그룹에 연결
            sfxSource = sources[1];
            sfxSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups(SFX_VOLUME_PARAM)[0]; // SFX 믹서 그룹에 연결
        }
    }

    //bgm 재생
    public void PlayBgm(int soundIndex)
    {
        // 1. JsonManager에서 데이터 조회
        if (!JsonManager.Instance.SoundDict.TryGetValue(soundIndex, out var table))
        {
            Debug.LogError($"[SoundManager] Index {soundIndex}를 찾을 수 없습니다.");
            return;
        }
        AudioClip clip = GetAudioClip(table.FileName);

        if (bgmSource.clip == clip && bgmSource.isPlaying) return; // 이미 같은 곡이 재생 중이면 무시

        if (clip != null)
        {
            // 데이터의 설정값 적용
            bgmSource.clip = clip;
            //bgmSource.volume = bgmValueSetting == true? bgmSource.volume:table.Volume;
            bgmSource.loop = table.Loop;

            bgmSource.Play(); // 재생
            Debug.Log($"{table.SoundName} 재생 시작 (볼륨: {table.Volume})");
        }
        else
        {
            Debug.Log($"{table.SoundName} clip이 없음");
        }
    }

    //sfx 재생
    public void PlaySFX(int soundIndex)
    {
        // 1. JsonManager에서 데이터 조회
        if (!JsonManager.Instance.SoundDict.TryGetValue(soundIndex, out var table))
        {
            Debug.LogError($"[SoundManager] Index {soundIndex}를 찾을 수 없습니다.");
            return;
        }
        AudioClip clip = GetAudioClip(table.FileName);

        if (clip != null)
        {
            // PlayOneShot은 기존 소리를 끊지 않고 중첩해서 재생함
            // (단, 이 방식은 루프 기능을 쓸 수 없음)
            // 데이터의 설정값 적용
            //sfxSource.volume = sfxValueSetting == true ? sfxSource.volume : table.Volume;
            sfxSource.loop = table.Loop;

            sfxSource.PlayOneShot(clip, table.Volume);
            Debug.Log($"{table.SoundName} 재생 시작 (볼륨: {table.Volume})");
        }
        else
        {
            Debug.Log($"{table.SoundName} clip이 없음");
        }
    }

    //Bgm 정지기능
    public void StopBgm()
    {
        bgmSource.Stop();
        bgmSource.clip = null; // 참조 해제
    }


    private AudioClip GetAudioClip(string fileName)
    {
        // 캐시에 있으면 즉시 반환
        if (clipCache.TryGetValue(fileName, out var cachedClip)) return cachedClip;

        // 없으면 Resources에서 로드 (경로는 프로젝트에 맞춰 수정)
        AudioClip newClip = Resources.Load<AudioClip>($"Sound/{fileName}");

        if (newClip != null) clipCache[fileName] = newClip;
        return newClip;
    }

    public void SetBgmVolume(int volume)
    {
        // 0~100을 0.0001~1.0 범위로 변환 후 dB로 계산 (선형 진폭과 일치)
        float linearVolume = Mathf.Max(0.0001f, volume / 100f); // 0.0001 ~ 1.0
        float dbVolume = 20f * Mathf.Log10(linearVolume);        // -80dB ~ 0dB
        audioMixer.SetFloat(BGM_VOLUME_PARAM, dbVolume);

        //bgmSource.volume = volume * 0.01f;
        bgmValueSetting = true;
    }

    public void SetSfxVolume(int volume)
    {
        // 0~100을 0.0001~1.0 범위로 변환 후 dB로 계산 (선형 진폭과 일치)
        float linearVolume = Mathf.Max(0.0001f, volume / 100f); // 0.0001 ~ 1.0
        float dbVolume = 20f * Mathf.Log10(linearVolume);        // -80dB ~ 0dB
        audioMixer.SetFloat(SFX_VOLUME_PARAM, dbVolume);

        //sfxSource.volume = volume * 0.01f;
        sfxValueSetting = true;
    }
}
