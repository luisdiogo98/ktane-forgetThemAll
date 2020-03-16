using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using rnd = UnityEngine.Random;

public class StageInfo {

	public int stageNo;
	public int moduleId;

	public bool[] LED = new bool[] {false, false, false, false, false, false, false, false, false, false, false, false, false};
	public string moduleName;

	public StageInfo(int stageNo, int moduleId)
	{
		this.stageNo = stageNo;
		this.moduleId = moduleId;
		
		int LEDn = rnd.Range(1, 4);

		int[] colors = new int[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12};
		colors = colors.OrderBy(x => rnd.Range(1, 1000)).ToArray();
		for(int i = 0; i < LEDn; i++)
		{
			LED[colors[i]] = true;
		}

	}

	public string GetOnLED()
	{
		string ret = "[";
		for(int i = 0; i < LED.Count(); i++)
		{
			if(LED[i])
			{
				ret += GetColorName(i);
				if(i != LED.Count() - 1)
					ret += " ";
			}
		}

		return ret.Trim() + "]";
	}

	public void SetModuleName(string moduleName)
	{
		this.moduleName = moduleName;

		Debug.LogFormat("[Forget Them All #{0}] Stage {1} corresponding module is {2}.", moduleId, stageNo, moduleName);
		// Yellow LED Toggle
		if(moduleName.IndexOf("wire",  StringComparison.InvariantCultureIgnoreCase) != -1)
		{
			LED[0] = !LED[0];
			Debug.LogFormat("[Forget Them All #{0}] Stage {1} corresponding module name contains \"wire\". Toggling Yellow LED.", moduleId, stageNo);
		}
		// Grey LED Toggle
		if(moduleName.IndexOf("button",  StringComparison.InvariantCultureIgnoreCase) != -1)
		{
			LED[1] = !LED[1];
			Debug.LogFormat("[Forget Them All #{0}] Stage {1} corresponding module name contains \"button\". Toggling Grey LED.", moduleId, stageNo);
		}
		else if(moduleName.IndexOf("key",  StringComparison.InvariantCultureIgnoreCase) != -1)
		{
			LED[1] = !LED[1];
			Debug.LogFormat("[Forget Them All #{0}] Stage {1} corresponding module name contains \"key\". Toggling Grey LED.", moduleId, stageNo);
		}
		// Blue LED Toggle
		if(moduleName.IndexOf("maze",  StringComparison.InvariantCultureIgnoreCase) != -1)
		{
			LED[2] = !LED[2];
			Debug.LogFormat("[Forget Them All #{0}] Stage {1} corresponding module contains \"maze\". Toggling Blue LED.", moduleId, stageNo);
		}
		// Green LED Toggle
		if(moduleName.IndexOf("simon",  StringComparison.InvariantCultureIgnoreCase) != -1)
		{
			LED[3] = !LED[3];
			Debug.LogFormat("[Forget Them All #{0}] Stage {1} corresponding module contains \"simon\". Toggling Green LED.", moduleId, stageNo);
		}
		// Orange LED Toggle
		if(moduleName.IndexOf("morse",  StringComparison.InvariantCultureIgnoreCase) != -1)
		{
			LED[4] = !LED[4];
			Debug.LogFormat("[Forget Them All #{0}] Stage {1} corresponding module contains \"morse\". Toggling Orange LED.", moduleId, stageNo);
		}
		// Red LED Toggle
		if(moduleName.IndexOf("cruel",  StringComparison.InvariantCultureIgnoreCase) != -1)
		{
			LED[5] = !LED[5];
			Debug.LogFormat("[Forget Them All #{0}] Stage {1} corresponding module contains \"cruel\". Toggling Red LED.", moduleId, stageNo);
		}
		else if(moduleName.IndexOf("complicated",  StringComparison.InvariantCultureIgnoreCase) != -1)
		{
			LED[5] = !LED[5];
			Debug.LogFormat("[Forget Them All #{0}] Stage {1} corresponding module contains \"complicated\". Toggling Red LED.", moduleId, stageNo);
		}
		else if(moduleName.IndexOf("broken",  StringComparison.InvariantCultureIgnoreCase) != -1)
		{
			LED[5] = !LED[5];
			Debug.LogFormat("[Forget Them All #{0}] Stage {1} corresponding module contains \"broken\". Toggling Red LED.", moduleId, stageNo);
		}
		else if(moduleName.IndexOf("cursed",  StringComparison.InvariantCultureIgnoreCase) != -1)
		{
			LED[5] = !LED[5];
			Debug.LogFormat("[Forget Them All #{0}] Stage {1} corresponding module contains \"cursed\". Toggling Red LED.", moduleId, stageNo);
		}
		else if(moduleName.IndexOf("faulty",  StringComparison.InvariantCultureIgnoreCase) != -1)
		{
			LED[5] = !LED[5];
			Debug.LogFormat("[Forget Them All #{0}] Stage {1} corresponding module contains \"faulty\". Toggling Red LED.", moduleId, stageNo);
		}
		// Lime LED Toggle
		if(moduleName.IndexOf("math",  StringComparison.InvariantCultureIgnoreCase) != -1)
		{
			LED[6] = !LED[6];
			Debug.LogFormat("[Forget Them All #{0}] Stage {1} corresponding module contains \"math\". Toggling Lime LED.", moduleId, stageNo);
		}
		else if(moduleName.IndexOf("number",  StringComparison.InvariantCultureIgnoreCase) != -1)
		{
			LED[6] = !LED[6];
			Debug.LogFormat("[Forget Them All #{0}] Stage {1} corresponding module contains \"number\". Toggling Lime LED.", moduleId, stageNo);
		}
		else if(moduleName.IndexOf("digit",  StringComparison.InvariantCultureIgnoreCase) != -1)
		{
			LED[6] = !LED[6];
			Debug.LogFormat("[Forget Them All #{0}] Stage {1} corresponding module contains \"digit\". Toggling Lime LED.", moduleId, stageNo);
		}
		else if(moduleName.IndexOf("equation",  StringComparison.InvariantCultureIgnoreCase) != -1)
		{
			LED[6] = !LED[6];
			Debug.LogFormat("[Forget Them All #{0}] Stage {1} corresponding module contains \"equation\". Toggling Lime LED.", moduleId, stageNo);
		}
		else if(moduleName.IndexOf("logic",  StringComparison.InvariantCultureIgnoreCase) != -1)
		{
			LED[6] = !LED[6];
			Debug.LogFormat("[Forget Them All #{0}] Stage {1} corresponding module contains \"logic\". Toggling Lime LED.", moduleId, stageNo);
		}
		// Cyan LED Toggle
		if(moduleName.IndexOf("word",  StringComparison.InvariantCultureIgnoreCase) != -1)
		{
			LED[7] = !LED[7];
			Debug.LogFormat("[Forget Them All #{0}] Stage {1} corresponding module contains \"word\". Toggling Cyan LED.", moduleId, stageNo);
		}
		else if(moduleName.IndexOf("letter",  StringComparison.InvariantCultureIgnoreCase) != -1)
		{
			LED[7] = !LED[7];
			Debug.LogFormat("[Forget Them All #{0}] Stage {1} corresponding module contains \"letter\". Toggling Cyan LED.", moduleId, stageNo);
		}
		else if(moduleName.IndexOf("phrase",  StringComparison.InvariantCultureIgnoreCase) != -1)
		{
			LED[7] = !LED[7];
			Debug.LogFormat("[Forget Them All #{0}] Stage {1} corresponding module contains \"phrase\". Toggling Cyan LED.", moduleId, stageNo);
		}
		else if(moduleName.IndexOf("text",  StringComparison.InvariantCultureIgnoreCase) != -1)
		{
			LED[7] = !LED[7];
			Debug.LogFormat("[Forget Them All #{0}] Stage {1} corresponding module contains \"text\". Toggling Cyan LED.", moduleId, stageNo);
		}
		else if(moduleName.IndexOf("talk",  StringComparison.InvariantCultureIgnoreCase) != -1)
		{
			LED[7] = !LED[7];
			Debug.LogFormat("[Forget Them All #{0}] Stage {1} corresponding module contains \"talk\". Toggling Cyan LED.", moduleId, stageNo);
		}
		else if(moduleName.IndexOf("alphabet",  StringComparison.InvariantCultureIgnoreCase) != -1)
		{
			LED[7] = !LED[7];
			Debug.LogFormat("[Forget Them All #{0}] Stage {1} corresponding module contains \"alphabet\". Toggling Cyan LED.", moduleId, stageNo);
		}
		// Brown LED Toggle
		if(moduleName.IndexOf("code",  StringComparison.InvariantCultureIgnoreCase) != -1)
		{
			LED[8] = !LED[8];
			Debug.LogFormat("[Forget Them All #{0}] Stage {1} corresponding module contains \"code\". Toggling Brown LED.", moduleId, stageNo);
		}
		else if(moduleName.IndexOf("cipher",  StringComparison.InvariantCultureIgnoreCase) != -1)
		{
			LED[8] = !LED[8];
			Debug.LogFormat("[Forget Them All #{0}] Stage {1} corresponding module contains \"cipher\". Toggling Brown LED.", moduleId, stageNo);
		}
		// White LED Toggle
		if(moduleName.IndexOf("light",  StringComparison.InvariantCultureIgnoreCase) != -1)
		{
			LED[9] = !LED[9];
			Debug.LogFormat("[Forget Them All #{0}] Stage {1} corresponding module contains \"light\". Toggling White LED.", moduleId, stageNo);
		}
		else if(moduleName.IndexOf("LED",  StringComparison.InvariantCultureIgnoreCase) != -1)
		{
			LED[9] = !LED[9];
			Debug.LogFormat("[Forget Them All #{0}] Stage {1} corresponding module contains \"LED\". Toggling White LED.", moduleId, stageNo);
		}
		// Purple LED Toggle
		if(moduleName.IndexOf("square",  StringComparison.InvariantCultureIgnoreCase) != -1)
		{
			LED[10] = !LED[10];
			Debug.LogFormat("[Forget Them All #{0}] Stage {1} corresponding module contains \"square\". Toggling Purple LED.", moduleId, stageNo);
		}
		else if(moduleName.IndexOf("circle",  StringComparison.InvariantCultureIgnoreCase) != -1)
		{
			LED[10] = !LED[10];
			Debug.LogFormat("[Forget Them All #{0}] Stage {1} corresponding module contains \"circle\". Toggling Purple LED.", moduleId, stageNo);
		}
		else if(moduleName.IndexOf("triangle",  StringComparison.InvariantCultureIgnoreCase) != -1)
		{
			LED[10] = !LED[10];
			Debug.LogFormat("[Forget Them All #{0}] Stage {1} corresponding module contains \"triangle\". Toggling Purple LED.", moduleId, stageNo);
		}
		else if(moduleName.IndexOf("cube",  StringComparison.InvariantCultureIgnoreCase) != -1)
		{
			LED[10] = !LED[10];
			Debug.LogFormat("[Forget Them All #{0}] Stage {1} corresponding module contains \"cube\". Toggling Purple LED.", moduleId, stageNo);
		}
		else if(moduleName.IndexOf("sphere",  StringComparison.InvariantCultureIgnoreCase) != -1)
		{
			LED[10] = !LED[10];
			Debug.LogFormat("[Forget Them All #{0}] Stage {1} corresponding module contains \"sphere\". Toggling Purple LED.", moduleId, stageNo);
		}
		// Magenta LED Toggle
		if(moduleName.IndexOf("color",  StringComparison.InvariantCultureIgnoreCase) != -1)
		{
			LED[11] = !LED[11];
			Debug.LogFormat("[Forget Them All #{0}] Stage {1} corresponding module contains \"color\". Toggling Magenta LED.", moduleId, stageNo);
		}
		else if(moduleName.IndexOf("colour",  StringComparison.InvariantCultureIgnoreCase) != -1)
		{
			LED[11] = !LED[11];
			Debug.LogFormat("[Forget Them All #{0}] Stage {1} corresponding module contains \"colour\". Toggling Magenta LED.", moduleId, stageNo);
		}
		// Pink LED toggling
		if(moduleName.IndexOf("melody",  StringComparison.InvariantCultureIgnoreCase) != -1)
		{
			LED[12] = !LED[12];
			Debug.LogFormat("[Forget Them All #{0}] Stage {1} corresponding module contains \"melody\". Toggling Pink LED.", moduleId, stageNo);
		}
		else if(moduleName.IndexOf("harmony",  StringComparison.InvariantCultureIgnoreCase) != -1)
		{
			LED[12] = !LED[12];
			Debug.LogFormat("[Forget Them All #{0}] Stage {1} corresponding module contains \"harmony\". Toggling Pink LED.", moduleId, stageNo);
		}
		else if(moduleName.IndexOf("chord",  StringComparison.InvariantCultureIgnoreCase) != -1)
		{
			LED[12] = !LED[12];
			Debug.LogFormat("[Forget Them All #{0}] Stage {1} corresponding module contains \"chord\". Toggling Pink LED.", moduleId, stageNo);
		}
		else if(moduleName.IndexOf("piano",  StringComparison.InvariantCultureIgnoreCase) != -1)
		{
			LED[12] = !LED[12];
			Debug.LogFormat("[Forget Them All #{0}] Stage {1} corresponding module contains \"piano\". Toggling Pink LED.", moduleId, stageNo);
		}

		Debug.LogFormat("[Forget Them All #{0}] Stage {1} Actual LED (after evaluating broken): {2}", moduleId, stageNo, GetOnLED());

	}

	string GetColorName(int color)
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
}
