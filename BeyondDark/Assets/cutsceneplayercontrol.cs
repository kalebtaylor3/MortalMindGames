using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class cutsceneplayercontrol : MonoBehaviour
{

    public UnityEvent OnEnd;
    public UnityEvent OnStart;
    public PlayableDirector director;

    private void Awake()
    {
       // OnStart?.Invoke();
    }

    private void Update()
    {

        if (director.state == PlayState.Playing)
        {
            OnStart?.Invoke();
        }
        else
        {
            OnEnd?.Invoke();
            Destroy(this, 5);
        }
    }
}
