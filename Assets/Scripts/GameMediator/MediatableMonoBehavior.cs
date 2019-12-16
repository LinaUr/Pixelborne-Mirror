using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MediatableMonoBehavior : MonoBehaviour
{
    private GameMediator _gameMediator;

    protected GameMediator gameMediator {
        get { 
                if (_gameMediator == null){
                    var gameMediatorObject = GameObject.FindGameObjectWithTag("GameMediator");
                    if(gameMediatorObject != null){
                        _gameMediator = gameMediatorObject.GetComponent<GameMediator>();
                    } else {
                        Debug.Log("ERROR: NO GAME_MEDIATOR_OBJECT WAS FOUND!!!");
                    }
                }
                return _gameMediator;
            }
        set { _gameMediator = value; }
    }
}
