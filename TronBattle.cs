using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

class Player
{
	static void Main(string[] args)
	{
		string[] inputs;
		var game = new TronGame();
		

		// ゲームループ
		while (true)
		{
			inputs = Console.ReadLine().Split(' ');
			int N = int.Parse(inputs[0]); // 全プレイヤー数 (2〜4).
			int P = int.Parse(inputs[1]); // 自分のプレイヤー番号 (0〜3).
			for (int i = 0; i < N; i++)
			{
				inputs = Console.ReadLine().Split(' ');
				// 左上スタートの座標系
				int X0 = int.Parse(inputs[0]); // 初期X座標
				int Y0 = int.Parse(inputs[1]); // 初期Y座標
				int X1 = int.Parse(inputs[2]); // 現在のX座標
				int Y1 = int.Parse(inputs[3]); // 現在のY座標

				// すでに通過された座標を埋める
				if (game.board[Y0,X0] == -1)
				{
					game.board[Y0,X0] = i;
				}
				game.board[Y1,X1] = i;

				// 自分の座標を更新
				if (i == P)
				{
					game.nowX = X1;
					game.nowY = Y1;
				}
			}

			// 合法手を取得
			var actions = game.GetLegalActions();

			// 合法手から最大の空きマス数を持つ手を取得
			var maxEmptyCount = 0;
			var action = actions[0];
			foreach (var act in actions)
			{
				var emptyCount = 0;
				switch (act)
				{
					case TronAction.Left:
						emptyCount = game.GetEmptyCount(game.nowX - 1, game.nowY);
						break;
					case TronAction.Up:
						emptyCount = game.GetEmptyCount(game.nowX, game.nowY - 1);
						break;
					case TronAction.Right:
						emptyCount = game.GetEmptyCount(game.nowX + 1, game.nowY);
						break;
					case TronAction.Down:
						emptyCount = game.GetEmptyCount(game.nowX, game.nowY + 1);
						break;
				}
				if (emptyCount > maxEmptyCount)
				{
					maxEmptyCount = emptyCount;
					action = act;
				}
			}

			// 着手
			game.Advance(action);
		}
	}
}

public enum TronAction
{
	Left,
	Up,
	Right,
	Down,
}

public class TronGame
{
	public int[,] board = new int[20,30];
	public int nowX;
	public int nowY;

	public TronGame()
	{
		Init();
	}

	// 盤面の初期化
	public void Init()
	{
		// 盤面の初期化
		for (int y = 0; y < 20; y++)
		{
			for (int x = 0; x < 30; x++)
			{
				board[y,x] = -1;
			}
		}

		nowX = 0;
		nowY = 0;
	}

	// 合法手を取得する
	public List<TronAction> GetLegalActions()
	{
		var actions = new List<TronAction>();
		if (nowX > 0 && board[nowY,nowX - 1] == -1)
		{
			actions.Add(TronAction.Left);
		}
		if (nowY > 0 && board[nowY - 1,nowX] == -1)
		{
			actions.Add(TronAction.Up);
		}
		if (nowX < 29 && board[nowY,nowX + 1] == -1)
		{
			actions.Add(TronAction.Right);
		}
		if (nowY < 19 && board[nowY + 1,nowX] == -1)
		{
			actions.Add(TronAction.Down);
		}

		return actions;
	}

	// 指定した場所から移動可能な空きマスの数を深さ優先探索で取得する
	// FloodFillを使うが、再帰で実装するとスタックオーバーフローするので、スタックを使う
	public int GetEmptyCount(int x, int y)
	{
		var boardClone = (int[,])board.Clone();
		var stack = new Stack<ValueTuple<int,int>>();
		stack.Push(new ValueTuple<int,int>(x,y));
		var emptyCount = 0;
		while (stack.Count > 0)
		{
			var pos = stack.Pop();
			var posX = pos.Item1;
			var posY = pos.Item2;
			if (boardClone[posY,posX] == -1)
			{
				emptyCount++;
				boardClone[posY,posX] = 0;
				if (posX > 0 && boardClone[posY,posX - 1] == -1)
				{
					stack.Push(new ValueTuple<int,int>(posX - 1,posY));
				}
				if (posY > 0 && boardClone[posY - 1,posX] == -1)
				{
					stack.Push(new ValueTuple<int,int>(posX,posY - 1));
				}
				if (posX < 29 && boardClone[posY,posX + 1] == -1)
				{
					stack.Push(new ValueTuple<int,int>(posX + 1,posY));
				}
				if (posY < 19 && boardClone[posY + 1,posX] == -1)
				{
					stack.Push(new ValueTuple<int,int>(posX,posY + 1));
				}
			}
		}

		return emptyCount;
	}



	public void Advance(TronAction action)
	{
		switch (action)
		{
			case TronAction.Left:
				nowX--;
				Console.WriteLine("LEFT");
				break;
			case TronAction.Up:
				nowY--;
				Console.WriteLine("UP");
				break;
			case TronAction.Right:
				nowX++;
				Console.WriteLine("RIGHT");
				break;
			case TronAction.Down:
				nowY++;
				Console.WriteLine("DOWN");
				break;
		}
	}

	public TronGame Clone()
	{
		var clone = new TronGame();
		clone.board = (int[,])board.Clone();
		clone.nowX = nowX;
		clone.nowY = nowY;
		return clone;
	}

}