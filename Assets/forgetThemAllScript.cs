using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using rnd = UnityEngine.Random;
using System.Text.RegularExpressions;

public class forgetThemAllScript : MonoBehaviour
{
	public KMBombInfo bomb;
	public KMAudio Audio;

	static string[] ignoredModules;

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
	int lastCalcStage = -1;
	List<int> wiresCut = new List<int>();
	StageInfo[] stages;
	int keyStage;
	List<int> cutOrder = new List<int>();


	int delayer = 0;
	List<string> solvedModuleNames = new List<string>();
	List<string> queuedSolvedNames = new List<string>();
	int startTime;

	void Awake()
	{
		moduleId = moduleIdCounter++;
        GetComponent<KMBombModule>().OnActivate += Activate;

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

		for (int x = 0; x < wireInt.Length; x++)
		{
			int y = x;
			wireInt[x].OnInteract += delegate () { CutWire(y); return false; };
		}
		// Inefficient coding, more efficient coding is provided above.
		/*		wireInt[0].OnInteract += delegate () { CutWire(0); return false; };
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
				wireInt[12].OnInteract += delegate () { CutWire(12); return false; };*/
		stageNo.text = "---";
	}
	bool canStart = false;
	void Activate()
	{
		startTime = (int) (bomb.GetTime() / 60);
		if (CheckAutoSolve())
		{
			Debug.LogFormat("[Forget Them All #{0}] There are 0 modules not ignored on this bomb. Autosolving...", moduleId);
			StartCoroutine(HandleSolving());
			return;
		}
		CreateStages();
		canStart = true;
	}
	IEnumerator HandleSolving()
	{
		moduleSolved = true;
		GetComponent<KMBombModule>().HandlePass();
		while (stageNo.text.Length > 0)
		{
			string curText = stageNo.text;
			stageNo.text = curText.Substring(0, curText.Length - 1);
			yield return new WaitForSeconds(0.1f);
		}
		stageNo.text = "";
		yield return null;
	}
	void Start ()
	{
		RandomizeColors();
	}

	void Update ()
	{
        if (moduleSolved || !canStart)
            return;

		if(delayer > 0)
			delayer--;

		List<string> curSolvedModules = bomb.GetSolvedModuleNames().Where(a => !ignoredModules.Contains(a)).ToList();

		foreach (string solvedModName in solvedModuleNames)
			curSolvedModules.Remove(solvedModName);
		foreach (string queuedSolvedName in queuedSolvedNames)
			curSolvedModules.Remove(queuedSolvedName);
		if (curSolvedModules.Count > 0)
			queuedSolvedNames.AddRange(curSolvedModules);

		if (delayer <= 0 && (lastCalcStage == -1 || queuedSolvedNames.Count > 0) && lastCalcStage < currentStage && !readyToSolve)
		{
			lastCalcStage++;
			if (lastCalcStage - 1 >= 0 && lastCalcStage - 1 < stages.Length)
			{
				stages[lastCalcStage - 1].SetModuleName(queuedSolvedNames[0]);
				solvedModuleNames.Add(queuedSolvedNames[0]);
				queuedSolvedNames.RemoveAt(0);
			}
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

		if(!readyToSolve && !moduleSolved)
		{
			Debug.LogFormat("[Forget Them All #{0}] Strike! [{1}] wire cut before module is ready to be solved.", moduleId, GetColorName(colors[wire]));
			GetComponent<KMBombModule>().HandleStrike();
			return;
		}
		LEDs[wire].transform.Find("light").GetComponentInChildren<Renderer>().material = lightColors[13];
		if (moduleSolved)
			return;
		int index = cutOrder.FindIndex(x => x == colors[wire]);
		if(index != -1)
			cutOrder.RemoveAt(index);

		if (index == 0)
		{
			Debug.LogFormat("[Forget Them All #{0}] Successfully cut {1} wire.", moduleId, GetColorName(colors[wire]));
			if (cutOrder.Count() == 0)
			{
				Debug.LogFormat("[Forget Them All #{0}] All necessary wires have been cut in order successfully. Module disarmed.", moduleId, GetColorName(colors[wire]));
				StartCoroutine(HandleSolving());
			}
			else
			{
				Debug.LogFormat("[Forget Them All #{0}] The remaining wires to cut in order are: {2}.", moduleId, GetColorName(colors[wire]), ListToString(cutOrder));
			}
		}
		else if (cutOrder.Any())
		{
			Debug.LogFormat("[Forget Them All #{0}] Strike! {1} wire cut. Expecting {2} wire. Remaining wires order: {3}.", moduleId, GetColorName(colors[wire]), GetColorName(cutOrder[0]), ListToString(cutOrder));
			GetComponent<KMBombModule>().HandleStrike();
		}
		else if (!moduleSolved)
		{
			Debug.LogFormat("[Forget Them All #{0}] There are no wires to cut in order, module disarmed.", moduleId);
			StartCoroutine(HandleSolving());
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
			default:
				return "";
		}
	}

	void RandomizeColors()
	{
		colors = colors.OrderBy(x => rnd.Range(0, 1000)).ToArray();

		for(int i = 0; i < wires.Count(); i++)
		{
			foreach(Renderer r in wires[i].GetComponentsInChildren<Renderer>())
				r.material = wireColors[colors[i]];
		}
	}

	bool CheckAutoSolve()
	{
		stageCount = bomb.GetSolvableModuleNames().Where(x => !ignoredModules.Contains(x)).Count();
		return stageCount == 0;
	}

	void CreateStages()
	{
		stages = new StageInfo[stageCount];
		for(int i = 0; i < stages.Count(); i++)
		{
			stages[i] = new StageInfo(i+1, moduleId);
		}
	}

	void DisplayNextStage()
	{
		currentStage++;
		delayer = 300;
		if(currentStage > stageCount)
		{
			if(readyToSolve)
				return;

			readyToSolve = true;

			ShowFinalStage();

			if(wiresCut.Count() == 13)
			{
				Debug.LogFormat("[Forget Them All #{0}] All wires were cut upon the module being ready to calculate. Disarming...", moduleId);
				StartCoroutine(HandleSolving());
				return;
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
		// Commented out, ToString with one overload method takes up less space.
/*		String stageText = "";

		if(currentStage < 100)
			stageText += "0";
		if(currentStage < 10)
			stageText += "0";
		stageText += currentStage + "";*/

		stageNo.text = currentStage.ToString("000");
	}

	void ShowFinalStage()
	{
		stageNo.text = "---";

		for(int i = 0; i < 13; i++)
		{
			
			LEDs[Array.IndexOf(colors, i)].GetComponentInChildren<Renderer>().material = wiresCut.Contains(Array.IndexOf(colors, i)) ? lightColors[13] : lightColors[i];
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
			if(!wiresCut.Contains(i))
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
		if (cutOrder.Any())
			Debug.LogFormat("[Forget Them All #{0}] Wire cut order is {1}.", moduleId, ListToString(cutOrder));
		else
		{
			Debug.LogFormat("[Forget Them All #{0}] Remaining uncut wires have no characters in relation to the module name! Cut any wire to solve the module.", moduleId);
		}

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
			default:
				return -1;
		}
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
	IEnumerator FakeSolveHandling()
	{
		stageNo.text = "---";
		moduleSolved = true;
		List<int> wiresToCutScrambled = new List<int>();
		while (wiresToCutScrambled.Count < wireInt.Length - wiresCut.Count)
		{
			int x = rnd.Range(0, wireInt.Length);
			if (!(wiresCut.Contains(x) || wiresToCutScrambled.Contains(x)))
			{
				wiresToCutScrambled.Add(x);
			}
		}
		for (var x = 0; x < wiresToCutScrambled.Count; x++)
		{
			wireInt[wiresToCutScrambled[x]].OnInteract();
			if (x < wiresToCutScrambled.Count - 1)
			yield return new WaitForSeconds(0.1f);
		}
		yield return null;
		StartCoroutine(HandleSolving());
	}
	void TwitchHandleForcedSolve()
	{
		if (!moduleSolved)
			Debug.LogFormat("[Forget Them All #{0}] A force solve has been issued viva TP Handler.", moduleId);
		StartCoroutine(FakeSolveHandling());
	}

	private readonly string TwitchHelpMessage = "Cut the following wires with \"!{0} cut 1 2 3 ...\" Wires are numbered 1–13 from left to right on the module.";
    IEnumerator ProcessTwitchCommand(string command)
    {
        var m = Regex.Match(command, @"^\s*(?:cut\s+)?([\d ]+)\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        if (!m.Success)
            yield break;

        int value;
        var wiresToCut = new List<KMSelectable>();
        foreach (var number in m.Groups[1].Value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
        {
			if (!int.TryParse(number, out value) || value < 1 || value > 13)
			{
				yield return string.Format("sendtochaterror The following wire in position {0} does not exist! Wires are numbered 1 – 13 from left to right.", value);
				yield break;
			}
			else if (wiresToCut.Contains(wireInt[value - 1]))
			{
				yield return string.Format("sendtochaterror You’re trying to cut wire {0} twice! The full command has been voided.", value);
				yield break;
			}
			else if (wiresCut.Contains(value - 1))
			{
				yield return string.Format("sendtochaterror You’re trying to cut wire {0} that is already cut! The full command has been voided.", value);
				yield break;
			}
            wiresToCut.Add(wireInt[value - 1]);
        }

        yield return null;
        yield return wiresToCut;
    }
}
