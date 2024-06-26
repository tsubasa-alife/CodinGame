using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 【ルール】
/// デッキ:ドラフト型(PICK 0, 1 or 2 | PASSは最初のカード | 相手と重複して選べる)
/// システム:バトルで相手HPを30→0にしたら勝ち
/// </summary>

class Player
{
	static void Main(string[] args)
	{
		string[] inputs;

		// game loop
		while (true)
		{
			for (int i = 0; i < 2; i++)
			{
				inputs = Console.ReadLine().Split(' ');
				int playerHealth = int.Parse(inputs[0]);
				int playerMana = int.Parse(inputs[1]);
				int playerDeck = int.Parse(inputs[2]);
				int playerRune = int.Parse(inputs[3]);
				int playerDraw = int.Parse(inputs[4]);
			}
			inputs = Console.ReadLine().Split(' ');
			int opponentHand = int.Parse(inputs[0]);
			int opponentActions = int.Parse(inputs[1]);
			for (int i = 0; i < opponentActions; i++)
			{
				string cardNumberAndAction = Console.ReadLine();
			}
			int cardCount = int.Parse(Console.ReadLine());
			for (int i = 0; i < cardCount; i++)
			{
				inputs = Console.ReadLine().Split(' ');
				int cardNumber = int.Parse(inputs[0]);
				int instanceId = int.Parse(inputs[1]);
				int location = int.Parse(inputs[2]);
				int cardType = int.Parse(inputs[3]);
				int cost = int.Parse(inputs[4]);
				int attack = int.Parse(inputs[5]);
				int defense = int.Parse(inputs[6]);
				string abilities = inputs[7];
				int myHealthChange = int.Parse(inputs[8]);
				int opponentHealthChange = int.Parse(inputs[9]);
				int cardDraw = int.Parse(inputs[10]);
			}

			// Write an action using Console.WriteLine()
			// To debug: Console.Error.WriteLine("Debug messages...");

			Console.WriteLine("PASS");
		}
	}
}