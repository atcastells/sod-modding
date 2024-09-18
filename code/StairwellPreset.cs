using System;
using UnityEngine;

// Token: 0x020007D0 RID: 2000
[CreateAssetMenu(fileName = "stairwell_data", menuName = "Database/Stairwell Preset")]
public class StairwellPreset : SoCustomComparison
{
	// Token: 0x04003D62 RID: 15714
	[Header("Setup")]
	public GameObject spawnObject;

	// Token: 0x04003D63 RID: 15715
	public GameObject objectTop;

	// Token: 0x04003D64 RID: 15716
	public GameObject centralSteps;

	// Token: 0x04003D65 RID: 15717
	[Header("Elevator")]
	[Tooltip("Does this stairwell feature an elevator?")]
	public bool featuresElevator = true;

	// Token: 0x04003D66 RID: 15718
	[Tooltip("The elevator object to spawn")]
	public GameObject elevatorObject;

	// Token: 0x04003D67 RID: 15719
	[Tooltip("How fast the elevator can travel")]
	public float elevatorMaxSpeed = 2f;

	// Token: 0x04003D68 RID: 15720
	[Tooltip("How fast the elevator can accelerate")]
	public float elevatorAcceleration = 0.25f;

	// Token: 0x04003D69 RID: 15721
	[Tooltip("The elevator accelerates if further away than this from its destination")]
	public float accelerateWhileThisFarAway = 10f;

	// Token: 0x04003D6A RID: 15722
	[Tooltip("How long the lift stays at a destination when there is somewhere else to go")]
	public float liftDelay = 3f;

	// Token: 0x04003D6B RID: 15723
	[Tooltip("How long the lift stays put after a new call when beginning movement")]
	public float movementDelay = 1f;
}
