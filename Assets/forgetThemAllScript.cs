using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;

public class forgetThemAllScript : MonoBehaviour 
{
	public KMBombInfo bomb;
	public KMAudio Audio;

	static System.Random rnd = new System.Random();
	static String[] ignoredModules;

	public TextMesh stageNo;

	public GameObject[] wires;
	public KMSelectable[] wireInt;
	public Material[] wireColors;

	public GameObject[] LEDs;
	public Material[] lightColors;

	//Logging
	static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

	int[] colors = new int[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12};

	bool readyToSolve = false;

	int stageCount;
	int currentStage = 0;
	int solveCount = 0;
	List<int> wiresCut = new List<int>();
	StageInfo[] stages;
	int keyStage;
	List<int> cutOrder = new List<int>();

	int ticker = 0;
	List<String> solvedModules = new List<String>();

	int startTime;

	void Awake()
	{
		moduleId = moduleIdCounter++;

		if (ignoredModules == null)
            ignoredModules = GetComponent<KMBossModule>().GetIgnoredModules("Forget Them All", new string[]{
				"Forget Them All",
				"Alchemy",
				"Forget Everything",
				"Forget Infinity",
				"Forget Me Not",
				"Forget This",
				"Purgatory",
				"Souvenir",
				"Cookie Jars",
				"Divided Squares",
				"Hogwarts",
				"The Swan",
				"Turn the Keys",
				"The Time Keeper",
				"Timing is Everything",
				"Turn the Key"
            });

		wireInt[0].OnInteract += delegate () { CutWire(0); return false; };
		wireInt[1].OnInteract += delegate () { CutWire(1); return false; };
		wireInt[2].OnInteract += delegate () { CutWire(2); return false; };
		wireInt[3].OnInteract += delegate () { CutWire(3); return false; };
		wireInt[4].OnInteract += delegate () { CutWire(4); return false; };
		wireInt[5].OnInteract += delegate () { CutWire(5); return false; };
		wireInt[6].OnInteract += delegate () { CutWire(6); return false; };
		wireInt[7].OnInteract += delegate () { CutWire(7); return false; };
		wireInt[8].OnInteract += delegate () { CutWire(8); return false; };
		wireInt[9].OnInteract += delegate () { CutWire(9); return false; };
		wireInt[10].OnInteract += delegate () { CutWire(10); return false; };
		wireInt[11].OnInteract += delegate () { CutWire(11); return false; };
		wireInt[12].OnInteract += delegate () { CutWire(12); return false; };
	}

	void Start () 
	{
		startTime = (int) (bomb.GetTime() / 60);

		RandomizeColors();
		if(CheckAutoSolve()) return;
		CreateStages();
		DisplayNextStage();
	}
	
	void Update () 
	{
		ticker++;

		if(ticker == 5)
		{
			ticker = 0;

			List<String> newSolves = bomb.GetSolvedModuleNames().ToList();

			if(newSolves.Count() == solveCount)
				return;

			solveCount = newSolves.Count();

			foreach (String d in ignoredModules) { newSolves.Remove(d); }
			foreach (String d in solvedModules) { newSolves.Remove(d); }

			if(newSolves.Count() == 0)
				return;
			
			stages[currentStage - 1].SetModuleName(newSolves[0]);
			solvedModules.Add(newSolves[0]);
			DisplayNextStage();
		}
	}

	void CutWire(int wire)
	{
		GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.WireSnip, transform);
		wireInt[wire].enabled = false;
		wires[wire].transform.Find("default").gameObject.SetActive(false);
		wires[wire].transform.Find("hl").gameObject.SetActive(false);
		wires[wire].transform.Find("wireCut").gameObject.SetActive(true);

		foreach(Renderer r in wires[wire].GetComponentsInChildren<Renderer>())
			r.material = wireColors[colors[wire]];

		wiresCut.Add(wire);

		if(moduleSolved)
			return;

		if(!readyToSolve)
		{
			Debug.LogFormat("[Forget Them All #{0}] Strike! {1} wire cut before module is ready to be solved.", moduleId, GetColorName(colors[wire]));
			GetComponent<KMBombModule>().HandleStrike();
			return;
		}

		int index = cutOrder.FindIndex(x => x == colors[wire]);
		if(index != -1)
			cutOrder.RemoveAt(index);

		LEDs[wire].transform.Find("light").GetComponentInChildren<Renderer>().material = lightColors[13];

		if(index == 0)
		{
			if(cutOrder.Count() == 0)
			{
				moduleSolved = true;
				stageNo.text = "";
				Debug.LogFormat("[Forget Them All #{0}] Successfully cut {1} wire. Module solved.", moduleId, GetColorName(colors[wire]));
				GetComponent<KMBombModule>().HandlePass();
			}
			else
			{
				Debug.LogFormat("[Forget Them All #{0}] Successfully cut {1} wire. Remaining wires order: {2}.", moduleId, GetColorName(colors[wire]), ListToString(cutOrder));
			}
		}
		else
		{
			Debug.LogFormat("[Forget Them All #{0}] Strike! {1} wire cut. Expecting {2} wire. Remaining wires order: {3}.", moduleId, GetColorName(colors[wire]), GetColorName(cutOrder[0]), ListToString(cutOrder));
			GetComponent<KMBombModule>().HandleStrike();
		}
	}

	String GetColorName(int color)
	{
		switch(color)
		{
			case 0:
				return "Yellow";
			case 1:
				return "Grey";
			case 2:
				return "Blue";
			case 3:
				return "Green";
			case 4:
				return "Orange";
			case 5:
				return "Red";
			case 6:
				return "Lime";
			case 7:
				return "Cyan";
			case 8:
				return "Brown";
			case 9:
				return "White";
			case 10:
				return "Purple";
			case 11:
				return "Magenta";
			case 12:
				return "Pink";
		}

		return "";
	}

	void RandomizeColors()
	{
		colors = colors.OrderBy(x => rnd.Next()).ToArray();

		for(int i = 0; i < wires.Count(); i++)
		{
			foreach(Renderer r in wires[i].GetComponentsInChildren<Renderer>())
				r.material = wireColors[colors[i]];
		}
	}

	bool CheckAutoSolve()
	{
		stageCount = bomb.GetSolvableModuleNames().Where(x => !ignoredModules.Contains(x)).Count();
		if(stageCount == 0)
		{
			moduleSolved = true;
			GetComponent<KMBombModule>().HandlePass();
			Debug.LogFormat("[Forget Them All #{0}] No not ignored modules on this bomb. Autosolving.", moduleId);
			return true;
		}

		return false;
	}

	void CreateStages()
	{
		stages = new StageInfo[stageCount];
		for(int i = 0; i < stages.Count(); i++)
		{
			stages[i] = new StageInfo(i+1, moduleId, rnd);
		}
	}

	void DisplayNextStage()
	{
		currentStage++;
		if(currentStage > stageCount)
		{
			readyToSolve = true;
			
			ShowFinalStage();

			if(wiresCut.Count() == 13)
			{
				Debug.LogFormat("[Forget Them All #{0}] All wires cut before module is ready to be solved. Autosolving. (Like, seriously?? 13 strikes and you're still alive? Jeez...)", moduleId);
				GetComponent<KMBombModule>().HandlePass();
			}

			CalcFinalSolution();
			CalcWireOrder();
			return;
		}

		foreach(GameObject LED in LEDs)
		{
			LED.transform.Find("light").GetComponentInChildren<Renderer>().material = lightColors[13];
		}

		StageInfo s = stages[currentStage - 1];

		Debug.LogFormat("[Forget Them All #{0}] --------------------------- Stage {1} ---------------------------", moduleId, currentStage);
		Debug.LogFormat("[Forget Them All #{0}] Stage {1} LED: {2}", moduleId, currentStage, s.GetOnLED());

		for(int i = 0; i < s.LED.Count(); i++)
		{
			if(s.LED[i])
			{
				LEDs[Array.IndexOf(colors, i)].GetComponentInChildren<Renderer>().material = lightColors[i];
			}
		}

		String stageText = "";

		if(currentStage < 100)
			stageText += "0";
		if(currentStage < 10)
			stageText += "0";
		stageText += currentStage + "";

		stageNo.text = stageText;
	}

	void ShowFinalStage()
	{
		stageNo.text = "---";

		for(int i = 0; i < 13; i++)
		{
			LEDs[Array.IndexOf(colors, i)].GetComponentInChildren<Renderer>().material = lightColors[i];
		}
	}

	void CalcFinalSolution()
	{
		Debug.LogFormat("[Forget Them All #{0}] --------------------------- Solving ---------------------------", moduleId, currentStage);

		int[] totalLED = new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};

		foreach(StageInfo si in stages)
		{
			totalLED = totalLED.Select((x, index) => x + (si.LED[index] ? 1 : 0)).ToArray();
		}

		Debug.LogFormat("[Forget Them All #{0}] ", moduleId, currentStage);


		int aaBattery = bomb.GetBatteryCount(Battery.AA);
		int portPlates = bomb.GetPortPlateCount();
		int dupPorts = bomb.CountDuplicatePorts();
		int moduleCount = bomb.GetModuleNames().Count();
		int strikeCount = bomb.GetStrikes();
		int snTotal = bomb.GetSerialNumberNumbers().Sum();
		int snLetters = bomb.GetSerialNumberLetters().Count();
		int portTypes = bomb.CountUniquePorts();
		int onInd = bomb.GetOnIndicators().Count();
		int offInd = bomb.GetOffIndicators().Count();
		int dBattery = bomb.GetBatteryCount(Battery.D);

		int yellow = totalLED[0] * aaBattery;
		int grey = totalLED[1] * portPlates;
		int blue = totalLED[2] * startTime;
		int green = totalLED[3] * dupPorts;
		int orange = totalLED[4] * moduleCount;
		int red = totalLED[5] * strikeCount;
		int lime = totalLED[6] * snTotal;
		int cyan = totalLED[7] * snLetters; 
		int brown =	totalLED[8] * portTypes; 
		int white = totalLED[9] * onInd; 
		int purple = totalLED[10] * totalLED[10];
		int magenta = totalLED[11] * offInd;
		int pink = totalLED[12] * dBattery;

		Debug.LogFormat("[Forget Them All #{0}] Yellow ocurrences = {1}. Multiplier = {2}. LED value = {3}.", moduleId, totalLED[0], aaBattery, yellow);
		Debug.LogFormat("[Forget Them All #{0}] Grey ocurrences = {1}. Multiplier = {2}. LED value = {3}.", moduleId, totalLED[1], portPlates, grey);
		Debug.LogFormat("[Forget Them All #{0}] Blue ocurrences = {1}. Multiplier = {2}. LED value = {3}.", moduleId, totalLED[2], startTime, blue);
		Debug.LogFormat("[Forget Them All #{0}] Green ocurrences = {1}. Multiplier = {2}. LED value = {3}.", moduleId, totalLED[3], dupPorts, green);
		Debug.LogFormat("[Forget Them All #{0}] Orange ocurrences = {1}. Multiplier = {2}. LED value = {3}.", moduleId, totalLED[4], moduleCount, orange);
		Debug.LogFormat("[Forget Them All #{0}] Red ocurrences = {1}. Multiplier = {2}. LED value = {3}.", moduleId, totalLED[5], strikeCount, red);
		Debug.LogFormat("[Forget Them All #{0}] Lime ocurrences = {1}. Multiplier = {2}. LED value = {3}.", moduleId, totalLED[6], snTotal, lime);
		Debug.LogFormat("[Forget Them All #{0}] Cyan ocurrences = {1}. Multiplier = {2}. LED value = {3}.", moduleId, totalLED[7], snLetters, cyan);
		Debug.LogFormat("[Forget Them All #{0}] Brown ocurrences = {1}. Multiplier = {2}. LED value = {3}.", moduleId, totalLED[8], portTypes, brown);
		Debug.LogFormat("[Forget Them All #{0}] White ocurrences = {1}. Multiplier = {2}. LED value = {3}.", moduleId, totalLED[9], onInd, white);
		Debug.LogFormat("[Forget Them All #{0}] Purple ocurrences = {1}. Multiplier = {2}. LED value = {3}.", moduleId, totalLED[10], totalLED[10], purple);
		Debug.LogFormat("[Forget Them All #{0}] Magenta ocurrences = {1}. Multiplier = {2}. LED value = {3}.", moduleId, totalLED[11], offInd, magenta);
		Debug.LogFormat("[Forget Them All #{0}] Pink ocurrences = {1}. Multiplier = {2}. LED value = {3}.", moduleId, totalLED[12], dBattery, pink);

		int value = yellow + grey + blue + green + orange + red + lime + cyan + brown + white + purple + magenta + pink;

		keyStage = value % stageCount;
		if(keyStage == 0)
			keyStage = stageCount;

		Debug.LogFormat("[Forget Them All #{0}] Final value = {1}. Key stage is {2} - {3}.", moduleId, value, keyStage, stages[keyStage - 1].moduleName);
	}

	void CalcWireOrder()
	{
		List<int> wiresToCut = new List<int>();

		for(int i = 0; i < 13; i++)
		{
			if(wiresCut.FindIndex(x => x == colors[i]) < 0)
				wiresToCut.Add(colors[i]);
		}

		for(int i = 0; i < stages[keyStage - 1].moduleName.Length; i++)
		{
			int charColor = GetCharColor(stages[keyStage - 1].moduleName[i]);
			int index = wiresToCut.FindIndex(x => x == charColor);
			if(index >= 0)
			{
				cutOrder.Add(charColor);
				wiresToCut.RemoveAt(index);
			}
		}

		Debug.LogFormat("[Forget Them All #{0}] Wire cut order is {1}.", moduleId, ListToString(cutOrder));
	}

	int GetCharColor(char c)
	{
		char conv = Char.ToUpper(c);
		switch(conv)
		{
			case 'A':
			case 'N':
			case '0':
				return 0;
			case 'B':
			case 'O':
			case '1':
				return 1;
			case 'C':
			case 'P':
			case '2':
				return 2;
			case 'D':
			case 'Q':
			case '3':
				return 3;
			case 'E':
			case 'R':
			case '4':
				return 4;
			case 'F':
			case 'S':
			case '5':
				return 5;
			case 'G':
			case 'T':
			case '6':
				return 6;
			case 'H':
			case 'U':
			case '7':
				return 7;
			case 'I':
			case 'V':
			case '8':
				return 8;
			case 'J':
			case 'W':
			case '9':
				return 9;
			case 'K':
			case 'X':
				return 10;
			case 'L':
			case 'Y':
				return 11;
			case 'M':
			case 'Z':
				return 12;
		}

		return -1;
	}

	String ListToString(List<int> l)
	{
		String res = "[";

		for(int i = 0; i < l.Count(); i++)
		{
			res += GetColorName(l[i]);
			if(i != l.Count() - 1)
				res += " ";
		}

		return res + "]";
	}
}
