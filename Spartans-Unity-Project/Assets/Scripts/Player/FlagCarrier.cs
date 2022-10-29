using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Netcode;

namespace Spartans.Players{
    public class FlagCarrier : NetworkBehaviour
    {
        [SerializeField] private Vector3 FlagLocalPosition;
        [SerializeField] private Vector3 FlagLocalRotation;


        private bool HaveFlag;
        private Transform Flag;
        private Collider _triggerTouched;


        // Start is called before the first frame update
        public void Init()
        {
            FlagLocalPosition = new Vector3();
            FlagLocalRotation = new Vector3();
            HaveFlag = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == "Flag")
            {
                //print("Touched flag");
                _triggerTouched = other;
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if(other.tag == "Flag")
            {
                //print("Left flag");
                _triggerTouched = null;
            }
        }

        public void InteractFlag()
        {
            if(!IsServer)
            {
                InteractFlagServerRpc();
                return;
            }
            if(HaveFlag)
            {
                Flag.transform.parent = null;
                HaveFlag = false;
            }
            else
            {
                if(_triggerTouched != null)
                {
                    Flag = _triggerTouched.transform;
                    Flag.parent = this.gameObject.transform;
                    HaveFlag = true;
                }
                else
                {
                    AudioManager.Instance.PlayAudio(AudioManager.AudioChannels.Channel2, AudioManager.SoundClipsIndex.score_SFX);
                }
            }
        }

        [ServerRpc]
        private void InteractFlagServerRpc()
        {
            if(HaveFlag)
            {
                Flag.transform.parent = null;
                HaveFlag = false;
            }
            else
            {
                if(_triggerTouched != null)
                {
                    Flag = _triggerTouched.transform;
                    Flag.parent = this.gameObject.transform;
                    HaveFlag = true;
                }
                else
                {
                    AudioManager.Instance.PlayAudio(AudioManager.AudioChannels.Channel2, AudioManager.SoundClipsIndex.score_SFX);
                }
            }
        }
    }
}
