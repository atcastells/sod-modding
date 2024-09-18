using System;
using System.Collections.Generic;

// Token: 0x02000668 RID: 1640
public class EvidenceSurveillance : Evidence
{
	// Token: 0x06002419 RID: 9241 RVA: 0x001DCC80 File Offset: 0x001DAE80
	public EvidenceSurveillance(EvidencePreset newPreset, string evID, Controller newController, List<object> newPassedObjects) : base(newPreset, evID, newController, newPassedObjects)
	{
		Interactable.Passed passed = newPassedObjects[0] as Interactable.Passed;
		this.captureID = (int)passed.value;
		if (this.savedCapture == null)
		{
			this.savedCapture = Player.Instance.sceneRecorder.interactable.sCap.Find((SceneRecorder.SceneCapture item) => item.id == this.captureID);
		}
		if (this.savedCapture == null)
		{
			foreach (SceneRecorder sceneRecorder in CityData.Instance.surveillanceDirectory)
			{
				if (sceneRecorder.interactable.sCap.Count > 0)
				{
					this.savedCapture = sceneRecorder.interactable.sCap.Find((SceneRecorder.SceneCapture item) => item.id == this.captureID);
					if (this.savedCapture != null)
					{
						return;
					}
				}
			}
		}
		if (this.savedCapture == null)
		{
			Game.LogError("Unable to locate saved capture " + this.captureID.ToString(), 2);
		}
	}

	// Token: 0x0600241A RID: 9242 RVA: 0x001DCD94 File Offset: 0x001DAF94
	public override string GenerateName()
	{
		if (this.savedCapture == null)
		{
			Game.Log("Saved capture is null", 2);
			return string.Empty;
		}
		string text = string.Empty;
		NewGameLocation captureGamelocation = this.savedCapture.GetCaptureGamelocation();
		if (captureGamelocation != null)
		{
			text = captureGamelocation.name + " ";
		}
		return text + SessionData.Instance.TimeAndDate(this.savedCapture.t, true, true, true);
	}

	// Token: 0x0600241B RID: 9243 RVA: 0x001DCE08 File Offset: 0x001DB008
	public override void OnDiscovery()
	{
		if (this.savedCapture == null)
		{
			return;
		}
		if (this.savedCapture.recorder.interactable.node.gameLocation != null)
		{
			EvidenceCreator.Instance.CreateFact("For", this, this.savedCapture.recorder.interactable.node.gameLocation.evidenceEntry, null, null, true, null, null, null, false);
		}
		foreach (SceneRecorder.ActorCapture actorCapture in this.savedCapture.aCap)
		{
			Human human = actorCapture.GetHuman();
			if (human != null)
			{
				EvidenceCreator.Instance.CreateFact("FeaturesIn", human.evidenceEntry, this, null, null, true, null, null, null, false);
			}
		}
		base.OnDiscovery();
	}

	// Token: 0x04002DFA RID: 11770
	public int captureID;

	// Token: 0x04002DFB RID: 11771
	public SceneRecorder.SceneCapture savedCapture;
}
