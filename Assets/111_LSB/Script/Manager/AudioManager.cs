using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// =================== AUDIO MANAGER CLASS (Singleton) =================================
// 배경음, 효과음 리소스를 관리하고 출력 해주는 매니저 클래스
// =====================================================================================

[System.Serializable]
public class AudioContainer
{
    public string name;
    public AudioClip audioClip;
}

public class AudioManager : MonoBehaviour
{
    // 사운드 매니저 인스턴스
    public static AudioManager instance = null;

    [SerializeField] AudioSource bgmPlayer;             // 배경음 플레이어
    [SerializeField] AudioSource[] sfxPlayer;           // 효과음 플레이어

    [SerializeField] AudioContainer[] bgmContainer;     // 배경음 창고
    [SerializeField] AudioContainer[] sfxContainer;     // 효과음 창고

    [SerializeField] Slider bgmSlider;                  // BGM 볼륨 조절 슬라이더
    [SerializeField] Slider sfxSlider;                  // SFX 볼륨 조절 슬라이더

    private void Awake()
    {
        // 인스턴스가 없다면 인스턴스를 담아준다.
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);
        }

        // 씬이 넘어 갔을때 만약 새로 생성하려고 시도한다면 파괴한다.
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // 볼륨 조절 업데이트
        bgmPlayer.volume = bgmSlider.value;

        for(int i=0; i<sfxPlayer.Length; ++i)
        {
            sfxPlayer[i].volume = bgmSlider.value;
        }
    }

    // 효과음 재생
    public void PlaySFX(string _sfxName)
    {
        // 해당하는 이름의 오디오 소스가 있는지 탐색
        foreach (AudioContainer audioContainer in sfxContainer)
        {
            // 만약 존재 한다면
            if(audioContainer.name == _sfxName)
            {
                // 재생 중이지 않은 sfx 플레이어 탐색
                for (int i = 0; i < sfxPlayer.Length; ++i)
                {
                    // 재생 중이지 않은 sfx 플레이어가 있다면
                    if(!sfxPlayer[i].isPlaying)
                    {
                        sfxPlayer[i].volume = sfxSlider.value; // 볼륨 설정
                        sfxPlayer[i].clip = audioContainer.audioClip;
                        sfxPlayer[i].Play();
                        return;
                    }
                }
                Debug.Log("Notice: All of SFX Player is Playing");
            }
        }
    }

    // 배경음 재생
    public void PlayBGM(string _sceneName)
    {
        Debug.Log("Notice: Current Scene Name: " + _sceneName);

        bgmPlayer.Stop();
        bgmPlayer.loop = true;              // 반복 재생
        bgmPlayer.volume = bgmSlider.value; // 볼륨 설정

        foreach (AudioContainer audioContainer in bgmContainer)
        {
            if (audioContainer.name == _sceneName)
            {
                bgmPlayer.clip = audioContainer.audioClip;
                bgmPlayer.Play();

                return;
            }
        }

        Debug.Log("Notice: " + _sceneName + "does not Exist");
    }

    public void StopBGM()
    {
        bgmPlayer.Stop();
    }

    // ================== PROPERTY ==================
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }

            return instance;
        }
    }
}
