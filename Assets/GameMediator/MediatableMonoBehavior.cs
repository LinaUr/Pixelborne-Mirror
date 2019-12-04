using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MediatableMonoBehavior : MonoBehaviour
{
    protected GameMediator _gameMediator {
        get { return _gameMediator; }
        set { _gameMediator = value; }
    }

    //NOTE: maybe a super() is missing?
    public void Start(){
        var gameMediatorObject = GameObject.Find("GameMediatorObject");
        if(_gameMediator != null){
            _gameMediator = gameMediatorObject.GetComponent<GameMediator>();
        } else {
            Debug.Log("ERROR: NO GAME_MEDIATOR_OBJECT WAS FOUND!!!");
        }
    }
}
