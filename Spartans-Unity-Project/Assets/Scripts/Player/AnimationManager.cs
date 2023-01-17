using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AnimationManager : NetworkBehaviour
{
    //NEED TO ADD
    //--state memory to stop, duplicate parameter updates being sent--
    //----------------------------------------------------------------
    Dictionary<string, string> _animatorParameters = new Dictionary<string, string>();
    private Animator _animator;
    //List<Param> _animatorParameters = new List<Param>();s
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
        
        /*
        foreach(string key in _animatorParameters.Keys){
            string value;
            _animatorParameters.TryGetValue(key, out value);
            print(key + " is a " + value);
        }
        

        // -------------Unit tests--------------
        SetParameter("dead", false);
        SetParameter("speed", 1.2f);
        SetParameter("player", 1);
        */
    }

    [ServerRpc]
    public void UpdateAnimatorServerRpc(string name, string value, string type){
        print("recieved rpc to Update Animator param: " + name);
        bool _bool;
        float _float;
        int _int;
        if(type == "Bool"){
            _bool = bool.Parse(value);
            _animator.SetBool(name, _bool);
        }else if(type == "Float"){
            _float = float.Parse(value);
            _animator.SetFloat(name, _float);

        }else if(type == "Int"){
            _int = int.Parse(value);
            _animator.SetInteger(name, _int);
        }else{
            Debug.Log("ONLY int bool float TYPES SHOULD HAVE BEEN PASSED");
        }
    }

    public bool SetParameter(string name, bool value){
        //first step is to verify that a valid inputs were passed
        string val;
        if(_animatorParameters.ContainsKey(name)){
            //print("found parameter");
            _animatorParameters.TryGetValue(name, out val);
            if(val == "Bool"){
                _animator.SetBool(name, value);
            }else{
                Debug.LogError($"Parameter {name} is not of type Bool");
                return false;
            }
        }else{
            print("Parameter not found");
            return false;
        }
        //print($"{name} set to {value}");
        if(!IsServer) UpdateAnimatorServerRpc(name, value.ToString(), val);
        return true;
    }
    public bool SetParameter(string name, float value){
        //first step is to verify that a valid input was passed for this parameter
        //first step is to verify that a valid inputs were passed
        string val;
        if(_animatorParameters.ContainsKey(name)){
            //print("found parameter " + name);
            _animatorParameters.TryGetValue(name, out val);
            if(val == "Float"){
                _animator.SetFloat(name, value);
            }else{
                Debug.LogError($"Parameter {name} is not of type Float");
                return false;
            }
        }else{
            print("Parameter not found");
            return false;
        }
        //print($"{name} set to {value}");
        if(!IsServer) UpdateAnimatorServerRpc(name, value.ToString(), val);
        return true;
    }
    public bool SetParameter(string name, int value){
        //first step is to verify that a valid input was passed for this parameter
        string val;
        if(_animatorParameters.ContainsKey(name)){
            //print("found parameter");
            _animatorParameters.TryGetValue(name, out val);
            if(val == "Int"){
                _animator.SetInteger(name, value);
            }else{
                Debug.LogError($"Parameter {name} is not of type Int");
                return false;
            }
        }else{
            print("Parameter not found");
            return false;
        }
        //print($"{name} set to {value}");
        if(!IsServer) UpdateAnimatorServerRpc(name, value.ToString(), val);
        return true;
    }
    public void Play(string name, int layer)
    {
        _animator.Play(name, layer);
    }
}

