using System;
using System.Collections.Generic;
using FunZaga.Audio;
using UnityEngine;
using UnityEngine.Pool;
using ZagaCore;

public class ActionInitData
{
    public GameObject Source;
    public GameObject Target;
    public ActionControl ActionControl;
}

public class ActionControl : MonoBehaviour
{
    [SerializeField] private List<TimelineAction> testActions;
    [SerializeField] private GameObject sourceGameObject;
    private Dictionary<string, List<GameObject>> pools = new Dictionary<string, List<GameObject>>();
    
    private ClientInput clientInput;
    private void Awake()
    {
        Refs.Instance.Get<EventService>().Subscribe<TryAutoAction>(TryAutoAction);
    }

    private void OnDestroy()
    {
        Refs.Instance.Get<EventService>().UnSubscribe<TryAutoAction>(TryAutoAction);
    }

    private TimelineAction currentAction = null;
    private TimelineAction lastAction = null;
    private int autoIndex = 0;
    private void TryAutoAction()
    {
        clientInput = Refs.Instance.Get<ClientInput>();
        if ((clientInput.ControlState & ActionState.ActionLocked) != 0)
            return;
        
        // determine the action to do here
        // This includes source, target, action, etc.
        // some how load these with addressables or something
        Debug.Log("Try Auto Action");

        GameObject timelineGO;
        if (pools.ContainsKey(testActions[autoIndex].name))
        {
            timelineGO = pools[testActions[autoIndex].name][0];
            pools[testActions[autoIndex].name].RemoveAt(0);
        }
        else
        {
            timelineGO = Instantiate(testActions[autoIndex].gameObject, sourceGameObject.transform.position, sourceGameObject.transform.rotation);
            timelineGO.name = testActions[autoIndex].name;
        }

        var timeline = timelineGO.GetComponent<TimelineAction>();

        if (currentAction == timeline)
            return;
        
        if (currentAction != null)
            lastAction = currentAction;
        
        currentAction = timeline;
        timeline.SetActionData(new ActionInitData
        {
            Source = sourceGameObject,
            Target = null,
            ActionControl = this
        });
        timelineGO.transform.position = sourceGameObject.transform.position;
        timeline.PlayTimeline();
        autoIndex = (autoIndex + 1) % testActions.Count;
    }
    
    public class ActionQueue
    {
        private List<TimelineAction> _queue = new List<TimelineAction>();

        public void Enqueue(TimelineAction timelineAction)
        {
            _queue.Add(timelineAction);
            _queue.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        }

        public TimelineAction Dequeue()
        {
            TimelineAction timelineAction = _queue[0];
            _queue.RemoveAt(0);
            return timelineAction;
        }
    }

    public void SetMovementLocked(TimelineAction action, bool value)
    {
        if (currentAction != action)
            return;
        
        clientInput = Refs.Instance.Get<ClientInput>();
        
        if(value)
            clientInput.ControlState |= ActionState.MovingLocked;
        else
            clientInput.ControlState &= ~ActionState.MovingLocked;
    }
    
    public void SetActionLocked(TimelineAction action, bool value)
    {
        if (currentAction != action)
            return;
        
        clientInput = Refs.Instance.Get<ClientInput>();
        
        if(value)
            clientInput.ControlState |= ActionState.ActionLocked;
        else
            clientInput.ControlState &= ~ActionState.ActionLocked;
    }

    public void RemoveOldAction()
    {
        if (lastAction == null)
            return;
        
        lastAction.InterruptTimeline();
        RecycleGameObject(lastAction);
    }
    
    public void RecycleGameObject(TimelineAction action)
    {
        if (!pools.ContainsKey(action.name))
        {
            pools.Add(action.name, new List<GameObject>());
        }
        action.StopAllCoroutines();
        action.gameObject.SetActive(false);
        pools[action.name].Add(action.gameObject);

        if (lastAction == action)
            lastAction = null;
        
        if (currentAction == action)
            currentAction = null;
    }

    public void PlayAudio(AudioDefinition audio, TimelineAction action)
    {
        if (audio == null || action == null)
            return;
        
        var audioController = action.GetComponent<AudioController3d>();
        if (audioController == null)
            return;
        audioController.SetAudioDefinition(audio);
        Refs.Instance.Get<AudioService>().PlaySound(audioController, action.transform);
    }
}

public class TryAutoAction : GameEvent<TryAutoAction>
{
    
}