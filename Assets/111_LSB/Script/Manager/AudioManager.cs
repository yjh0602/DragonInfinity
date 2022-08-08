using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// =================== AUDIO MANAGER CLASS (Singleton) =================================
// �����, ȿ���� ���ҽ��� �����ϰ� ��� ���ִ� �Ŵ��� Ŭ����
// =====================================================================================

[System.Serializable]
public class AudioContainer
{
    public string name;
    public AudioClip audioClip;
}

public class AudioManager : MonoBehaviour
{
    // ���� �Ŵ��� �ν��Ͻ�
    public static AudioManager instance = null;

    [SerializeField] AudioSource bgmPlayer;             // ����� �÷��̾�
    [SerializeField] AudioSource[] sfxPlayer;           // ȿ���� �÷��̾�

    [SerializeField] AudioContainer[] bgmContainer;     // ����� â��
    [SerializeField] AudioContainer[] sfxContainer;     // ȿ���� â��

    [SerializeField] Slider bgmSlider;                  // BGM ���� ���� �����̴�
    [SerializeField] Slider sfxSlider;                  // SFX ���� ���� �����̴�

    private void Awake()
    {
        // �ν��Ͻ��� ���ٸ� �ν��Ͻ��� ����ش�.
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);
        }

        // ���� �Ѿ� ������ ���� ���� �����Ϸ��� �õ��Ѵٸ� �ı��Ѵ�.
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // ���� ���� ������Ʈ
        bgmPlayer.volume = bgmSlider.value;

        for(int i=0; i<sfxPlayer.Length; ++i)
        {
            sfxPlayer[i].volume = bgmSlider.value;
        }
    }

    // ȿ���� ���
    public void PlaySFX(string _sfxName)
    {
        // �ش��ϴ� �̸��� ����� �ҽ��� �ִ��� Ž��
        foreach (AudioContainer audioContainer in sfxContainer)
        {
            // ���� ���� �Ѵٸ�
            if(audioContainer.name == _sfxName)
            {
                // ��� ������ ���� sfx �÷��̾� Ž��
                for (int i = 0; i < sfxPlayer.Length; ++i)
                {
                    // ��� ������ ���� sfx �÷��̾ �ִٸ�
                    if(!sfxPlayer[i].isPlaying)
                    {
                        sfxPlayer[i].volume = sfxSlider.value; // ���� ����
                        sfxPlayer[i].clip = audioContainer.audioClip;
                        sfxPlayer[i].Play();
                        return;
                    }
                }
                Debug.Log("Notice: All of SFX Player is Playing");
            }
        }
    }

    // ����� ���
    public void PlayBGM(string _sceneName)
    {
        Debug.Log("Notice: Current Scene Name: " + _sceneName);

        bgmPlayer.Stop();
        bgmPlayer.loop = true;              // �ݺ� ���
        bgmPlayer.volume = bgmSlider.value; // ���� ����

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
