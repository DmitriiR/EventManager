/*  ╔═════════════════════════════╡  DinoTank 2018 ╞════════════════════════════╗            
    ║ Authors:  Dmitrii Roets                       Email:    roetsd@icloud.com ║
    ╟───────────────────────────────────────────────────────────────────────────╢░ 
    ║ Purpose: Registers and invokes custom game events                         ║░
    ║ Usage:   Insert the below OnEnable and OnDisable functions and register   ║░
    ║          them for events. Use EventManager.TriggerEvent("EventName");     ║░
    ║          to trigger all subscriers. In Order to make use of this class,   ║░
    ║          put the below in the listenging class, awaiting to be triggered  ║░
    ║          and we have to unsubscribe should the object be destroyed. It is ║░
    ║          very important to subscribe/unsubscribe in OnEnable and OnDisable║░ 
    ║          only. I order to make use of the class, put                      ║░
    ║          EventManager.TriggerEvent("EventName"); in any script and it will║░
    ║          call cause all susbscribers to call their respective methods.    ║░
    ╚═══════════════════════════════════════════════════════════════════════════╝░
       ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

/* Usage: 
In Order to make use of this class, put the below in the listenging class, awaiting to be triggered and we have to unsubscribe should the object be destroyed. It is very important to subscribe/unsubscribe in OnEnable and OnDisable only. I order to make use of the class, put EventManager.TriggerEvent("EventName"); in any script and it will call cause all susbscribers to call their methods.
void OnEnable() // always use this for events 
{
    EventManager.StartListening("SomeEventNameToListenTo", FunctionNameToBeTriggered); 
    // use with params  EventManager.StartListening("SomeEventNameToListenTo", delegate { AddLife(25); });
}
void OnDisable() // 
{
    EventManager.StopListening("SomeEventNameToListenTo", FunctionNameToBeTriggered);
}
    /// in order to call the event, any script can use this line
    EventManager.TriggerEvent("SomeEventNameToListenTo");
*/


public class EventManager : MonoBehaviour {
    /// <summary>
    ///  primary dictinary for storing events
    /// </summary>
    private Dictionary<string, UnityEvent> eventDictionary;

    /// <summary>
    /// used fo debugging, so one can see what listeneres register for a specific event
    /// </summary>
    public List<string> registeredEvents = new List<string>();

    private static EventManager eventManager;
    public static EventManager instance
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType(typeof (EventManager)) as EventManager;
                if (!eventManager)
                {
                   GameObject manager = Instantiate(Resources.Load("Managers/EventManager", typeof(GameObject)) as GameObject);
                    if (manager)
                    {
                        eventManager = manager.GetComponent<EventManager>();                    
                        eventManager.Init();
                    }
                }
                else
                {
                    eventManager.Init();
                }
            }
            return eventManager;
        }
    }

    void Awake()
    {
        transform.SetParent(null);
        DontDestroyOnLoad(this.gameObject);       
    }

    void Init()
    {
        if (eventDictionary == null)
            eventDictionary = new Dictionary<string, UnityEvent>();
    }

    public static void StartListening(string _eventName, UnityAction _listener)
    {
        instance.registeredEvents.Add(_eventName);
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(_eventName, out thisEvent))
        {
            thisEvent.AddListener(_listener);
        }
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(_listener);
            instance.eventDictionary.Add(_eventName, thisEvent);
        }
    }
    public static void StopListening(string _eventName, UnityAction _listener)
    {
        if (!eventManager) return;       
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(_eventName, out thisEvent))
        {
            thisEvent.RemoveListener(_listener);
        }
       
    }

    public static void TriggerEvent(string _eventName)
    {
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(_eventName, out thisEvent))
        {
            thisEvent.Invoke();
        }       
    }


}
