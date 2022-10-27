using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spartans.GameMode{
    //Intended for use on Server
    public class GameTimer : MonoBehaviour
    {
        public System.Action<int> OnSecondsChanged;
        public delegate void DoOnTimerFinish();

        private DoOnTimerFinish _executeOnEnd;
        private float _currentTime;
        private float _timerDuration;
        //private float _timeTillSecondChange;

        public int TimeInSeconds{get; private set;}
        public bool TimerActive {get; private set;}

        // Start is called before the first frame update
        void Start()
        {
            _currentTime = 0.0f;
            _timerDuration = 0.0f;
            
        }

        // Update is called once per frame
        void Update()
        {
            if(!TimerActive) return;

            if(_currentTime > 0)
            {
                _currentTime -= Time.deltaTime;
                //_timeTillSecondChange += Time.deltaTime;
                if((int)_currentTime < TimeInSeconds)
                {
                    TimeInSeconds = (int)_currentTime;
                    OnSecondsChanged?.Invoke(TimeInSeconds);
                }
            }
            else
            {
                TimerActive = false;

                if(_executeOnEnd != null){
                    _executeOnEnd.Invoke();
                    _executeOnEnd = null;
                }
            }
        }

        public void StartTimer(float duration, DoOnTimerFinish executeOnTimerFinish)
        {
            if(TimerActive)
            {
                Debug.LogWarning("Couldn't start time while running");
                return;
            }

            _timerDuration = duration;
            _executeOnEnd = executeOnTimerFinish;

            _currentTime = _timerDuration;
            TimeInSeconds = (int)_currentTime;
            TimerActive = true;
        }
        public void StartTimer(float duration)
        {
            if(TimerActive)
            {
                Debug.LogWarning("Couldn't start time while running");
                return;
            }
            _timerDuration = duration;

            _currentTime = _timerDuration;
            TimeInSeconds = (int)_currentTime;
            TimerActive = true;
        }
    }
}
