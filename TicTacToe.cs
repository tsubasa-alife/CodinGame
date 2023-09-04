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
		GameBoard gameboard = new GameBoard();

		// ゲームループ
		while (true)
		{
			inputs = Console.ReadLine().Split(' ');
			// 相手の手を取得
			int opponentRow = int.Parse(inputs[0]);
			int opponentCol = int.Parse(inputs[1]);
			// 先攻時は相手の手がないので-1が入る
			if (opponentRow != -1 && opponentCol != -1)
			{
				gameboard.Advance(opponentRow, opponentCol);
			}
			int validActionCount = int.Parse(Console.ReadLine());
			string move = "";
			// 各合法手について処理
			for (int i = 0; i < validActionCount; i++)
			{
				inputs = Console.ReadLine().Split(' ');
				int row = int.Parse(inputs[0]);
				int col = int.Parse(inputs[1]);
			}

			var moves = gameboard.GetLegalMoves();
			var moveRow = moves[0].Item1;
			var moveCol = moves[0].Item2;
			gameboard.Advance(moveRow, moveCol);
			move = moveRow.ToString() + " " + moveCol.ToString();
			Console.WriteLine(move);
		}
	}
}

// 盤面情報管理用クラス
class GameBoard
{
	// ゲームの状態管理用enum
	enum GameStatus
	{
		BlackTurn,
		WhiteTurn,
		BlackWin,
		WhiteWin,
		Draw
	}

	private int[,] _board = new int[3,3];
	private GameStatus gameStatus = GameStatus.BlackTurn;

	public GameBoard()
	{
		// 初期化
		for (int row = 0; row < 3; row++)
		{
			for (int col = 0; col < 3; col++)
			{
				_board[row,col] = 0;
			}
		}
	}

	public void Advance(int row, int col)
	{
		if (gameStatus == GameStatus.BlackTurn)
		{
			_board[row,col] = 1;
		}
		else if (gameStatus == GameStatus.WhiteTurn)
		{
			_board[row,col] = 2;
			
		}
		UpdateGameStatus();
	}

	public List<ValueTuple<int,int>> GetLegalMoves()
	{
		List<ValueTuple<int,int>> legalMoves = new List<ValueTuple<int,int>>();
		for (int row = 0; row < 3; row++)
		{
			for (int col = 0; col < 3; col++)
			{
				if (_board[row,col] == 0)
				{
					legalMoves.Add(new ValueTuple<int,int>(row,col));
				}
			}
		}
		return legalMoves;
	}

	public void UpdateGameStatus()
	{
		// 横のチェック
		for (int row = 0; row < 3; row++)
		{
			if (_board[row,0] == _board[row,1] && _board[row,1] == _board[row,2])
			{
				if (_board[row,0] == 1)
				{
					gameStatus = GameStatus.BlackWin;
					return;
				}
				else if (_board[row,0] == 2)
				{
					gameStatus = GameStatus.WhiteWin;
					return;
				}
			}
		}
		// 縦のチェック
		for (int col = 0; col < 3; col++)
		{
			if (_board[0,col] == _board[1,col] && _board[1,col] == _board[2,col])
			{
				if (_board[0,col] == 1)
				{
					gameStatus = GameStatus.BlackWin;
					return;
				}
				else if (_board[0,col] == 2)
				{
					gameStatus = GameStatus.WhiteWin;
					return;
				}
			}
		}
		// 斜めのチェック
		if (_board[0,0] == _board[1,1] && _board[1,1] == _board[2,2])
		{
			if (_board[0,0] == 1)
			{
				gameStatus = GameStatus.BlackWin;
				return;
			}
			else if (_board[0,0] == 2)
			{
				gameStatus = GameStatus.WhiteWin;
				return;
			}
		}
		if (_board[0,2] == _board[1,1] && _board[1,1] == _board[2,0])
		{
			if (_board[0,2] == 1)
			{
				gameStatus = GameStatus.BlackWin;
				return;
			}
			else if (_board[0,2] == 2)
			{
				gameStatus = GameStatus.WhiteWin;
				return;
			}
		}

		// 空のマスがあるかチェック
		bool empty = false;
		foreach (int cell in _board)
		{
			if (cell == 0)
			{
				empty = true;
				if (gameStatus == GameStatus.BlackTurn)
				{
					gameStatus = GameStatus.WhiteTurn;
				}
				else if (gameStatus == GameStatus.WhiteTurn)
				{
					gameStatus = GameStatus.BlackTurn;
				}
				return;
			}
		}

		// 空のマスがない場合は引き分け
		if (!empty)
		{
			gameStatus = GameStatus.Draw;
		}
	}

	public int Evaluate()
	{
		int score = 0;
		switch (gameStatus)
		{
			case GameStatus.BlackWin:
				if(gameStatus == GameStatus.BlackTurn)
				{
					score = 1;
				}
				else
				{
					score = -1;
				}
				break;
			case GameStatus.WhiteWin:
				if(gameStatus == GameStatus.WhiteTurn)
				{
					score = 1;
				}
				else
				{
					score = -1;
				}
				break;
			default:
				score = 0;
				break;
		}
		return score;
	}

	public GameBoard Clone()
	{
		GameBoard clone = new GameBoard();
		clone._board = (int[,])_board.Clone();
		clone.gameStatus = gameStatus;
		return clone;
	}
}