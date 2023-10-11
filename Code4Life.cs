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
			}
			inputs = Console.ReadLine().Split(' ');
			int availableA = int.Parse(inputs[0]);
			int availableB = int.Parse(inputs[1]);
			int availableC = int.Parse(inputs[2]);
			int availableD = int.Parse(inputs[3]);
			int availableE = int.Parse(inputs[4]);
			int sampleCount = int.Parse(Console.ReadLine()); // ゲーム中のサンプルの数
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
			}

			// Write an action using Console.WriteLine()
			// To debug: Console.Error.WriteLine("Debug messages...");

			// 行動として可能なのは以下
			// GOTO <module> : <module>に移動する
			// CONNECT <id/type> : <id/type>のサンプルをつなげる
			Console.WriteLine("GOTO DIAGNOSIS");
		}
	}
}

public enum Module
{
	Diagnosis,
	Molecules,
	Laboratory
}

public class GameState
{
	public Module module;
	public int currentSampleId = -1;

	public void DoAction()
	{
		// サンプルを持っていない場合は取得しに行く
		if (currentSampleId == -1)
		{
			if (module != Module.Diagnosis)
			{
				Console.WriteLine("GOTO Diagnosis");
				module = Module.Diagnosis;
				return;
			}
			else
			{
				Console.WriteLine("CONNECT 1");
				currentSampleId = 1;
				return;
			}
		}
		else
		{
			// サンプルを持っている場合は分子を取りに行く
		}
	}
}