using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderValue : MonoBehaviour
{
    [SerializeField] private TMP_Text _valueText;
    [SerializeField] private Slider _slider;
    [SerializeField] private AudioManager.AudioChannels _audioChannelAffiliation;

    // Start is called before the first frame update
    void Start()
    {
        _slider.onValueChanged.AddListener(UpdateTextValue);
        _slider.onValueChanged.AddListener(RequestMixerChange);
        float volume;
        if(AudioManager.Instance.GetChannelVolume(_audioChannelAffiliation, out volume)){
            _slider.value =  volume/AudioManager.DB_TO_FLOAT_MULTIPLIER;
        }else _slider.value = 0;
        UpdateTextValue(_slider.value);
    }

    private void RequestMixerChange(float value){
        AudioManager.Instance.UpdateChannelVolume(_audioChannelAffiliation, value);
    }
    private void UpdateTextValue(float value)
    {
        value = value*AudioManager.DB_TO_FLOAT_MULTIPLIER;
        string output = "(";
        output = value > 0.0f ? output+="+": output;
        _valueText.text = output + value + "db)";
    }
    void OnDestroy()
    {
        _slider.onValueChanged.RemoveAllListeners();
    }

   
}
