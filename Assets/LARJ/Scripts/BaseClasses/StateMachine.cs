using System.Collections;
using System.Collections.Generic;
using UnityEngine;// State Machine 
public class StateMachine
{

	public class State
	{
		public string name;


		public System.Action onFrame;

		public System.Action onEnter;

		public System.Action onExit;

		public override string ToString()
		{
			return name;
		}
	}

	Dictionary<string, State> states = new Dictionary<string, State>();

	public State currentState { get; private set; }

	public State initState;


	public State CreateState(string name, System.Action onEnter,System.Action onFrame = null,System.Action onExit = null) 
	{
		var newState = new State();

		newState.name = name;
		newState.onEnter = onEnter;
		newState.onFrame = onFrame;
		newState.onExit = onExit;

		if(states.Count == 0)
        {
			initState = newState;
        }

		states[name] = newState;

		return newState;
	}
	public State CreateState(string name)
	{
		var newState = new State();

		newState.name = name;

		if (states.Count == 0)
		{
			initState = newState;
		}

		states[name] = newState;

		return newState;
	}

	public void Update()
	{
		if (states.Count == 0 || initState == null)
		{
			Debug.LogErrorFormat("State Machine has no states!");
			return;
		}

		if (currentState == null)
		{
			TransitionTo(initState);
		}

		if (currentState.onFrame != null)
		{
			currentState.onFrame();
		}
	}

	public void TransitionTo(State newState)
	{
		if (newState == null)
		{
			Debug.LogErrorFormat("Cannot transition to a null state!");
			return;
		}

		if (currentState != null && currentState.onExit != null)
		{
			currentState.onExit();
		}

		currentState = newState;

		if (newState.onEnter != null)
		{
			newState.onEnter();
		}
	}

	public void TransitionTo(string name)
	{

		if (states.ContainsKey(name) == false)
		{
			Debug.LogErrorFormat("State Machine doesn't contain a state named {0}!", name);
			return;
		}

		var newState = states[name];

		TransitionTo(newState);

	}
}









































