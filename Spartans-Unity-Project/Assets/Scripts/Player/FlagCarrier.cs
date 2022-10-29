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
        private Spartans.GameMode.Flag Flag;
        private Collider _triggerTouched;
        private PlayerController _playerController;


        // Start is called before the first frame update
        public void Init(PlayerController playerController)
        {
            _playerController = playerController;

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
                Flag.DroppedFlag();
            }
            else
            {
                if(_triggerTouched != null)
                {
                    Flag = _triggerTouched.gameObject.GetComponent<Spartans.GameMode.Flag>();
                    if(Flag.PickUp(_playerController.GetTeamAssociation().Value))
                    {
                        Flag.transform.parent = this.gameObject.transform;
                        HaveFlag = true;
                        AudioManager.Instance.PlayAudio(AudioManager.AudioChannels.Channel2, AudioManager.SoundClipsIndex.score_SFX);
                    }
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
                Flag.DroppedFlag();
            }
            else
            {
                if(_triggerTouched != null)
                {
                    Flag = _triggerTouched.gameObject.GetComponent<Spartans.GameMode.Flag>();
                    if(Flag.PickUp(_playerController.GetTeamAssociation().Value))
                    {
                        Flag.transform.parent = this.gameObject.transform;
                        HaveFlag = true;
                        InteractFlagSuccessClientRpc();
                    }
                }
            }
        }

        [ClientRpc]
        private void InteractFlagSuccessClientRpc()
        {
            //Follow up to send sound to only player which grabbed flag
            AudioManager.Instance.PlayAudio(AudioManager.AudioChannels.Channel2, AudioManager.SoundClipsIndex.score_SFX);
        }
    }
}
