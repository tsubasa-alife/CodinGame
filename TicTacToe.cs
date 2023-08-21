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
		string[] inputs;
		GameBoard gameboard = new GameBoard();

		// game loop
		while (true)
		{
			inputs = Console.ReadLine().Split(' ');
			int opponentRow = int.Parse(inputs[0]);
			int opponentCol = int.Parse(inputs[1]);
			int validActionCount = int.Parse(Console.ReadLine());
			string move = "";
			for (int i = 0; i < validActionCount; i++)
			{
				inputs = Console.ReadLine().Split(' ');
				int row = int.Parse(inputs[0]);
				int col = int.Parse(inputs[1]);
				gameboard.SetMove(row,col);
			}

			// Write an action using Console.WriteLine()
			// To debug: Console.Error.WriteLine("Debug messages...");
			move = gameboard.GetMove();
			Console.WriteLine(move);
		}
	}
}

// 盤面情報管理用クラス
class GameBoard
{
	private int _row = 0;
	private int _col = 0;

	public void SetMove(int row, int col)
	{
		_row = row;
		_col = col;
	}

	public string GetMove()
	{
		string move = _row + " " + _col;
		return move;
	}
}