using System;
using UnityEngine;

public class PlayerSteps : MonoBehaviour
{
	public event Action StepRight;
	public event Action StepLeft;
	
	public void AnimationEventStepRight() => StepRight?.Invoke();
	
	public void AnimationEventStepLeft() => StepLeft?.Invoke();
}
