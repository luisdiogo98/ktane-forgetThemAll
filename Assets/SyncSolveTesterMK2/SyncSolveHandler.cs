using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KModkit;
using System.Linq;

public class SyncSolveHandler : MonoBehaviour {

	public GameObject disarmButtonObject, entireModule;
	public KMSelectable disarmButton;
	public KMBombInfo bombInfo;
	public KMBombModule modSelf;
	public KMAudio audioSelf;

	private bool hasDisarmed = false, hasActivated = false;
	private bool isPressedDisarm = false;

	private static int modID = 1;
	private int curmodID;
	protected sealed class SyncSolveGlobal //Lock down infomation to a single bomb, hopefully.
	{
		public List<SyncSolveHandler> syncSolveMods = new List<SyncSolveHandler>();// A collection of Singularity Button Handlers on 1 global handler.
		public bool canDisarm = false;
		public void DisarmAll()
		{
			canDisarm = true;
		}
		public int CountSyncSolveTesters()
		{
			return syncSolveMods.Count();
		}
		public bool IsEqualToNumberOnBomb(SyncSolveHandler buttonHandler)
		{
			return CountSyncSolveTesters() == buttonHandler.bombInfo.GetModuleNames().Where(a => a.Equals("Singularity Button")).Count();
		}
		public IEnumerator StartBootUpSequence()
		{
			if (syncSolveMods.Count <= 0) yield break;
			if (IsEqualToNumberOnBomb(syncSolveMods[0]))
			{
				
			}
			yield return null;
		}
	}
	private static readonly Dictionary<KMBomb, SyncSolveGlobal> groupedSyncSolveTesters = new Dictionary<KMBomb, SyncSolveGlobal>();
	private SyncSolveGlobal syncSolveTester;
	void Awake()
	{
		curmodID = modID++;
	}
	// Use this for initialization
	void Start () {
		disarmButton.OnInteract += delegate {
			audioSelf.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
			isPressedDisarm = true;
			return false;
		};
		disarmButton.OnInteractEnded += delegate
		{
			disarmButton.AddInteractionPunch();
			audioSelf.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonRelease, transform);
			isPressedDisarm = false;
			if (hasActivated)
			{
				syncSolveTester.DisarmAll();
			}
		};
		modSelf.OnActivate += delegate
		{
			// Setup Global Interaction
			KMBomb bombAlone = entireModule.GetComponentInParent<KMBomb>(); // Get the bomb that the module is attached on. Required for intergration due to modified value.
			//Required for Multiple Bombs stable interaction in case of different bomb seeds.

			if (!groupedSyncSolveTesters.ContainsKey(bombAlone))
				groupedSyncSolveTesters[bombAlone] = new SyncSolveGlobal();
			syncSolveTester = groupedSyncSolveTesters[bombAlone];
			syncSolveTester.syncSolveMods.Add(this);

			// Start Main Handling
			StartCoroutine(HandleGlobalModule());
			hasActivated = true;
		};
	}
	IEnumerator HandleGlobalModule()
	{
		StartCoroutine(syncSolveTester.StartBootUpSequence());
		while (!syncSolveTester.canDisarm)
		{
			yield return new WaitForSeconds(0);
		}
		hasDisarmed = true;
		modSelf.HandlePass();
		Debug.LogFormat("[Sync Solve Tester #{0}]: Module disarmed.", curmodID);
		yield return null;
	}
	// Update is called once per frame
	int frameDisarm = 45;
	void Update () {
		if (!hasActivated) return;
		if (!isPressedDisarm)
		{
			frameDisarm = Mathf.Min(frameDisarm + 1, 45);
		}
		else
			frameDisarm = Mathf.Max(frameDisarm - 1, 40);
		disarmButtonObject.transform.localPosition = new Vector3(0, 0.019f * (frameDisarm / 45f), 0);
	}
	// Handle Twitch Plays
	IEnumerator HandleForcedSolve()
	{
		disarmButton.OnInteract();
		yield return new WaitForSeconds(0.2f);
		disarmButton.OnInteractEnded();
	}

	void TwitchHandleForcedSolve()
	{
		Debug.LogFormat("[Sync Solve Tester #{0}]: A force solve has been issued viva TP Handler.", curmodID);
		StartCoroutine(HandleForcedSolve());
	}
	#pragma warning disable 0414
		string TwitchHelpMessage = "To press the disarm button: \"!{0} disarm\" This module is only here to test synchronized solves.";
	#pragma warning restore 0414
	IEnumerator ProcessTwitchCommand(string command)
	{
		string interpetedCommand = command.ToLower();
		string pressDisarm = @"^disarm$";

		if (hasDisarmed)
		{
			yield return "sendtochaterror Are you trying to interact the button when its already solved? You might want to think again. (This is an anarchy command prevention message.)";
			yield break;
		}
		else if (interpetedCommand.RegexMatch(pressDisarm))
		{
			yield return null;
			yield return disarmButton;
			yield return "solve";
			yield return new WaitForSeconds(0.2f);
			yield return disarmButton;
		}
		yield break;
	}
}
