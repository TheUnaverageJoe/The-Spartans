using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public enum SoundClipsIndex : int{
        bg_music = 0,
        spear_attack = 1
    }
    public enum AudioChannels{
        Master_Volume,
        Channel1,
        Channel2,
        Channel3
    }
    [SerializeField] private List<AudioClip> sounds;
    //[SerializeField] private List<AudioSource> _audioChannels = new List<AudioSource>(); 
    [SerializeField] private AudioSource channel1;
    [SerializeField] private AudioSource channel2;
    [SerializeField] private AudioSource channel3;
    [Header("*Mandatory Refs*")]
    [SerializeField] private AudioMixer _mixer;
    //[SerializeField] private Slider _masterSlider;
    //[SerializeField] private Slider _channel1Slider;

    private const string MASTER_VOLUME = "MasterVolume";
    private const string CHANNEL1 = "Channel1Volume";
    private const string CHANNEL2 = "Channel2Volume";
    private const string CHANNEL3 = "Channel3Volume";
    public const float DB_TO_FLOAT_MULTIPLIER = 4f;

    public static AudioManager Instance{get; private set;}

    void Awake(){
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }else
        {
            print("Destroyed an spawned audio manager");
            Destroy(this.gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if(sounds == null)
        {
            print("NO AUDIO CLIPS PROVIDED");
        }
        
        PlayAudio(AudioChannels.Channel1, SoundClipsIndex.bg_music);

    }

    public void PlayAudio(AudioChannels channel, SoundClipsIndex index)
    {
        switch(channel){
            case AudioChannels.Channel1:
                channel1.clip = sounds[(int)index];
                channel1.Play();
                break;
            case AudioChannels.Channel2:
                //channel2.clip = sounds[(int)index];
                channel2.PlayOneShot(sounds[(int)index]);
                break;
            case AudioChannels.Channel3:
                channel3.clip = sounds[(int)index];
                channel3.Play();
                break;
        }
        
    }


    public void UpdateChannelVolume(AudioChannels channel, float value)
    {
        string name = AudioChannelToString(channel);
        _mixer.SetFloat(name, value*DB_TO_FLOAT_MULTIPLIER);
    }
    public bool GetChannelVolume(AudioChannels channel, out float outValue)
    {
        string name = AudioChannelToString(channel);
        if(_mixer.GetFloat(name, out outValue))
        {
            return true;
        }
        return false;
    }
    private string AudioChannelToString(AudioChannels channel){
        switch(channel){
            case AudioChannels.Master_Volume:
                return MASTER_VOLUME;
            case AudioChannels.Channel1:
                return CHANNEL1;
            case AudioChannels.Channel2:
                return  CHANNEL2;
            case AudioChannels.Channel3:
                return CHANNEL3;
            default:
                return "";
        }

    }


}
