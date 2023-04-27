using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * Get the market!
 **/
class Player
{
	static void Main(string[] args)
	{

		// game loop
		while (true)
		{
			int id = int.Parse(Console.ReadLine()); // Your player id
			int playerCount = int.Parse(Console.ReadLine()); // Number of players
			int turn = int.Parse(Console.ReadLine()); // Number of turns since the beginning
			int income = int.Parse(Console.ReadLine()); // Your income for this turn
			int cash = int.Parse(Console.ReadLine()); // Cash of your start-up
			int devs = int.Parse(Console.ReadLine());
			int sellers = int.Parse(Console.ReadLine());
			int managers = int.Parse(Console.ReadLine());
			int features = int.Parse(Console.ReadLine());
			int tests = int.Parse(Console.ReadLine()); // Tests developed in your software
			int bugs = int.Parse(Console.ReadLine()); // Bugs in your software
			for (int i = 0; i < playerCount; i++)
			{
				string[] inputs = Console.ReadLine().Split(' ');
				int startUpId = int.Parse(inputs[0]); // Start-up id
				int marketShare = int.Parse(inputs[1]); // Market share of the start-up in thousands
				int reputation = int.Parse(inputs[2]); // Reputation of the start-up
			}

			// Write an action using Console.WriteLine()
			// To debug: Console.Error.WriteLine("Debug messages...");
			int devsToHire = 0; // Number of developers to hire
			int sellerToHire = 0; // Number of sellers to hire
			int managersToHire = 0; // Number of managers to hire
			int maintenanceDevs = 0; // Number of developers to put in maintenance
			int competitiveSellers = 0; // Number of sellers to put in competitive mode
			
			// 五つの変数をスペース区切りで文字列として結合する
			string output = string.Join(" ", devsToHire, sellerToHire, managersToHire, maintenanceDevs, competitiveSellers);

			// <devsToHire> <sellerToHire> <managersToHire> <maintenanceDevs> <competitiveSellers> <targetId>
			Console.WriteLine(output);
		}
	}
}