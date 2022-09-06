using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AnimationManager : NetworkBehaviour
{
    /*
    ///We Could have used this custom class but a dictionary is inbuilt and achieves similar functionality
    private class Param{
        protected string _name;
        protected string _type;
        public Param(string name, string type){
            this._name = name;
            this._type = type;
        }
        public override string ToString(){
            string var = $"Parameter '{_name}' is of type {_type}";
            return var;
        }
        public string GetName(){
            return _name;
        }
        public string GetValueType(){
            return _type;
        }
    }
    */
    Dictionary<string, string> _animatorParameters = new Dictionary<string, string>();
    private Animator _animator;
    //List<Param> _animatorParameters = new List<Param>();
    public void Awake(){
        _animator = GetComponentInChildren<Animator>();
    }
    public void Start(){

        //INITIALLIZE
        //This foreach loop starts by storing all the 
        //  parameter's names and their value types both as strings
        foreach(AnimatorControllerParameter single in _animator.parameters){
            _animatorParameters.Add(single.name, single.type.ToString());
        }

        //Just to see which parameters got added
        //and to test overridden ToString() method
        foreach(string key in _animatorParameters.Keys){
            print(key);
        }
    }

/*
    public bool SetParameter(string name, bool value){
        //first step is to verify that a valid input was passed for this parameter
        foreach(Param parameter in _animatorParameters){
            if(parameter.)
        }
    }
    public bool SetParameter(string name, float value){
        //first step is to verify that a valid input was passed for this parameter
        
    }
    public bool SetParameter(string name, int value){
        //first step is to verify that a valid input was passed for this parameter
        
    }
    */
}
