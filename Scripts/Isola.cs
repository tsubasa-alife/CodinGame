using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
class Player
{
	static void Main(string[] args)
	{
		int playerPositionX = int.Parse(Console.ReadLine());
		int playerPositionY = int.Parse(Console.ReadLine()); // player's coordinates.

		// game loop
		while (true)
		{
			int opponentPositionX = int.Parse(Console.ReadLine());
			int opponentPositionY = int.Parse(Console.ReadLine()); // opponent's coordinates.
			int opponentLastRemovedTileX = int.Parse(Console.ReadLine());
			int opponentLastRemovedTileY = int.Parse(Console.ReadLine()); // coordinates of the last removed tile. (-1 -1) if no tile has been removed.

			// Write an action using Console.WriteLine()
			// To debug: Console.Error.WriteLine("Debug messages...");

			Console.WriteLine("RANDOM;MESSAGE");
		}
	}
}

/// <summary>
/// 各プレイヤーの情報
/// </summary>
public class Agent
{
	public int posX;
	public int posY;
}

public class GameState
{
	private Agent[] agents = new Agent[2];
	private int[,] board = new int[8, 8];

	public void Init()
	{

	}
}