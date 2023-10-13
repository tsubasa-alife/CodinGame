using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * Code4Life(StateMachine)
 **/
class Player
{
	static void Main(string[] args)
	{
		Agent agent = new Agent();
		agent.Init();
		string[] inputs;
		int projectCount = int.Parse(Console.ReadLine());
		for (int i = 0; i < projectCount; i++)
		{
			inputs = Console.ReadLine().Split(' ');
			int a = int.Parse(inputs[0]);
			int b = int.Parse(inputs[1]);
			int c = int.Parse(inputs[2]);
			int d = int.Parse(inputs[3]);
			int e = int.Parse(inputs[4]);
		}

		// game loop
		while (true)
		{
			for (int i = 0; i < 2; i++)
			{
				inputs = Console.ReadLine().Split(' ');
				string target = inputs[0]; // どこのモジュールにいるか
				int eta = int.Parse(inputs[1]);
				int score = int.Parse(inputs[2]); // 得点
				int storageA = int.Parse(inputs[3]); // プレイヤーが持っている各分子の数
				int storageB = int.Parse(inputs[4]);
				int storageC = int.Parse(inputs[5]);
				int storageD = int.Parse(inputs[6]);
				int storageE = int.Parse(inputs[7]);
				int expertiseA = int.Parse(inputs[8]);
				int expertiseB = int.Parse(inputs[9]);
				int expertiseC = int.Parse(inputs[10]);
				int expertiseD = int.Parse(inputs[11]);
				int expertiseE = int.Parse(inputs[12]);
				if (i == 0)
				{
					// 初期化
					if (agent.module == Module.None)
					{
						if (target == "DIAGNOSIS")
						{
							agent.module = Module.Diagnosis;
						}
						else if (target == "MOLECULES")
						{
							agent.module = Module.Molecules;
						}
						else if (target == "LABORATORY")
						{
							agent.module = Module.Laboratory;
						}
					}
					agent.score = score;
					agent.storage[0] = storageA;
					agent.storage[1] = storageB;
					agent.storage[2] = storageC;
					agent.storage[3] = storageD;
					agent.storage[4] = storageE;
					agent.expertise[0] = expertiseA;
					agent.expertise[1] = expertiseB;
					agent.expertise[2] = expertiseC;
					agent.expertise[3] = expertiseD;
					agent.expertise[4] = expertiseE;
				}
			}
			inputs = Console.ReadLine().Split(' ');
			int availableA = int.Parse(inputs[0]);
			int availableB = int.Parse(inputs[1]);
			int availableC = int.Parse(inputs[2]);
			int availableD = int.Parse(inputs[3]);
			int availableE = int.Parse(inputs[4]);
			int sampleCount = int.Parse(Console.ReadLine()); // ゲーム中のサンプルの数
			var sampleList = new List<Dictionary<string, object>>(); // サンプルのリスト
			for (int i = 0; i < sampleCount; i++)
			{
				inputs = Console.ReadLine().Split(' ');
				int sampleId = int.Parse(inputs[0]); // サンプルのID
				int carriedBy = int.Parse(inputs[1]); // どのプレイヤーが持っているか 0:自分, 1:相手, -1:クラウド上
				int rank = int.Parse(inputs[2]);
				string expertiseGain = inputs[3];
				int health = int.Parse(inputs[4]); // サンプルの健康スコア
				int costA = int.Parse(inputs[5]); // サンプルを作るのに必要な各分子の数
				int costB = int.Parse(inputs[6]);
				int costC = int.Parse(inputs[7]);
				int costD = int.Parse(inputs[8]);
				int costE = int.Parse(inputs[9]);
				var sample = new Dictionary<string, object>();
				sample.Add("sampleId", sampleId);
				sample.Add("carriedBy", carriedBy);
				sample.Add("rank", rank);
				sample.Add("expertiseGain", expertiseGain);
				sample.Add("health", health);
				sample.Add("costA", costA);
				sample.Add("costB", costB);
				sample.Add("costC", costC);
				sample.Add("costD", costD);
				sample.Add("costE", costE);
				sampleList.Add(sample);
			}
			agent.GetSampleList(sampleList);
			// Write an action using Console.WriteLine()
			// To debug: Console.Error.WriteLine("Debug messages...");

			// 行動決定
			agent.DoAction();
		}
	}
}

public enum State
{
	GetSample,
	GetMolecule,
	GetMedicine,
}

public enum Module
{
	None,
	Diagnosis,
	Molecules,
	Laboratory,
}

public class Agent
{
	public State state = State.GetSample;
	public Module module = Module.None;
	public bool needMolecule = false;
	public int score = 0;
	public int currentSampleId = -1;
	public Dictionary<string, int> storage;
	public Dictionary<string, int> expertise;
	public List<Dictionary<string, object>> sampleList = new List<Dictionary<string, object>>();

	// 初期化
	public void Init()
	{
		state = State.GetSample;
		module = Module.None;
		needMolecule = false;
		score = 0;
		currentSampleId = -1;
		storage = new Dictionary<string, int> { { "A", 0 }, { "B", 0 }, { "C", 0 }, { "D", 0 }, { "E", 0 } };
		expertise = new Dictionary<string, int> { { "A", 0 }, { "B", 0 }, { "C", 0 }, { "D", 0 }, { "E", 0 } };
	}

	public void GetSampleList(List<Dictionary<string, object>> sampleList)
	{
		this.sampleList = sampleList;
	}

	public int SelectSample()
	{
		var bestSampleIndex = 0;
		var bestSampleHealth = 0;
		for (var i = 0; i < sampleList.Count; i++)
		{
			var sample = sampleList[i];
			var health = (int)sample["health"];
			if (health > bestSampleHealth)
			{
				bestSampleIndex = i;
				bestSampleHealth = health;
			}
		}

		return bestSampleIndex;
	}

	// 単純な分子選択
	public void BasicMolculeSelection()
	{
		var sample = sampleList[currentSampleId];
		var needA = (int)sample["costA"] - storage["A"] - expertise["A"];
		var needB = (int)sample["costB"] - storage["B"] - expertise["B"];
		var needC = (int)sample["costC"] - storage["C"] - expertise["C"];
		var needD = (int)sample["costD"] - storage["D"] - expertise["D"];
		var needE = (int)sample["costE"] - storage["E"] - expertise["E"];
		if (needA > 0)
		{
			Console.WriteLine("CONNECT A");
			return;
		}
		else if (needB > 0)
		{
			Console.WriteLine("CONNECT B");
			return;
		}
		else if (needC > 0)
		{
			Console.WriteLine("CONNECT C");
			return;
		}
		else if (needD > 0)
		{
			Console.WriteLine("CONNECT D");
			return;
		}
		else if (needE > 0)
		{
			Console.WriteLine("CONNECT E");
			return;
		}
	}

	public bool CanGetMedicine()
	{
		var sample = sampleList[currentSampleId];
		var needA = (int)sample["costA"] - storage["A"] - expertise["A"];
		var needB = (int)sample["costB"] - storage["B"] - expertise["B"];
		var needC = (int)sample["costC"] - storage["C"] - expertise["C"];
		var needD = (int)sample["costD"] - storage["D"] - expertise["D"];
		var needE = (int)sample["costE"] - storage["E"] - expertise["E"];
		if (needA <= 0 && needB <= 0 && needC <= 0 && needD <= 0 && needE <= 0)
		{
			return true;
		}
		else
		{
			return false;
		}
	}


	public void DoAction()
	{
		switch (state)
		{
			case State.GetSample:
				GetSample();
				break;
			case State.GetMolecule:
				GetMolecule();
				break;
			case State.GetMedicine:
				GetMedicine();
				break;
		}
	}

	// サンプル取得状態
	private void GetSample()
	{
		if (module != Module.Diagnosis)
		{
			Console.WriteLine("GOTO DIAGNOSIS");
			module = Module.Diagnosis;
			return;
		}
		else
		{
			var sampleIndex = SelectSample();
			Console.WriteLine("CONNECT " + sampleIndex);
			currentSampleId = sampleIndex;
			needMolecule = true;
			state = State.GetMolecule;
			return;
		}
	}

	private void GetMolecule()
	{
		if (module != Module.Molecules)
		{
			Console.WriteLine("GOTO MOLECULES");
			module = Module.Molecules;
			return;
		}
		else
		{
			if (CanGetMedicine())
			{
				needMolecule = false;
				state = State.GetMedicine;
			}
			else
			{
				BasicMolculeSelection();
			}
			return;
		}
	}

	private void GetMedicine()
	{
		if (module != Module.Laboratory)
		{
			Console.WriteLine("GOTO LABORATORY");
			module = Module.Laboratory;
			return;
		}
		else
		{
			if (CanGetMedicine())
			{
				// スコアを獲得する
				Console.WriteLine("CONNECT " + currentSampleId);
				currentSampleId = -1;
			}
			else
			{
				Console.WriteLine("GOTO DIAGNOSIS");
				state = State.GetSample;
				module = Module.Diagnosis;
			}
			return;
		}
	}


}