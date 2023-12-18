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
		GameBoard _gameBoard = new GameBoard();

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
				_gameBoard.Advance(opponentRow, opponentCol);
			}
			int validActionCount = int.Parse(Console.ReadLine());
			for (int i = 0; i < validActionCount; i++)
			{
				inputs = Console.ReadLine().Split(' ');
				int row = int.Parse(inputs[0]);
				int col = int.Parse(inputs[1]);
			}

			// 最善手を取得
			_gameBoard.PrintBoard();
			var bestMove = GetAlphaBetaBestMove(_gameBoard, 10);
			var moveRow = bestMove.Item1;
			var moveCol = bestMove.Item2;

			// 最善手を着手
			_gameBoard.Advance(moveRow, moveCol);
			string move = moveRow.ToString() + " " + moveCol.ToString();
			Console.WriteLine(move);
		}
	}

	// AlphaBeta法で最善手を探索
	private static ValueTuple<int,int> GetAlphaBetaBestMove(GameBoard gameBoard, int depth)
	{
		var moves = gameBoard.GetLegalMoves();
		var bestMove = moves[0];
		var alpha = -100;
		var beta = 100;

		foreach (var move in moves)
		{
			var newGameBoard = gameBoard.Clone();
			newGameBoard.Advance(move.Item1, move.Item2);
			var score = -GetAlphaBetaScore(newGameBoard, -beta, -alpha, depth - 1);
			if (score > alpha)
			{
				alpha = score;
				bestMove = move;
			}
		}

		return bestMove;
	}

	// AlphaBeta法で盤面評価値を計算
	private static int GetAlphaBetaScore(GameBoard gameBoard, int alpha, int beta, int depth)
	{
		if (gameBoard.IsGameOver() || depth == 0)
		{
			return gameBoard.Evaluate();
		}

		var moves = gameBoard.GetLegalMoves();

		if (moves.Count == 0)
		{
			return gameBoard.Evaluate();
		}

		foreach (var move in moves)
		{
			var newGameBoard = gameBoard.Clone();
			newGameBoard.Advance(move.Item1, move.Item2);
			var score = -GetAlphaBetaScore(newGameBoard, -beta, -alpha, depth - 1);
			if (score >= beta)
			{
				return beta;
			}
			if (score > alpha)
			{
				alpha = score;
			}
		}

		return alpha;
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
	private bool isBlackPerspective = true;

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

	// 局面を1手進める
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
		// 視点を入れ替える
		isBlackPerspective = !isBlackPerspective;
	}

	// 合法手のリストを返す
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

	// ゲームの状態を更新する
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

	// ゲームが終了しているかどうかを返す
	public bool IsGameOver()
	{
		if (gameStatus == GameStatus.BlackWin || gameStatus == GameStatus.WhiteWin || gameStatus == GameStatus.Draw)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	// 盤面評価（先攻視点ならtrue、後攻視点ならfalseが引数）
	public int Evaluate()
	{
		int score = 0;
		switch (gameStatus)
		{
			case GameStatus.BlackWin:
				if(isBlackPerspective)
				{
					score = 1;
				}
				else
				{
					score = -1;
				}
				break;
			case GameStatus.WhiteWin:
				if(isBlackPerspective)
				{
					score = -1;
				}
				else
				{
					score = 1;
				}
				break;
			default:
				score = 0;
				break;
		}
		return score;
	}

	// 盤面を表示する
	public void PrintBoard()
	{
		Console.Error.WriteLine("GameStatus: " + gameStatus.ToString());
		Console.Error.WriteLine("isBlackPerspective: " + isBlackPerspective.ToString());
		Console.Error.WriteLine("Board:");
		for (int row = 0; row < 3; row++)
		{
			string line = "";
			for (int col = 0; col < 3; col++)
			{
				line += _board[row,col].ToString();
			}
			Console.Error.WriteLine(line);
		}
	}

	// 盤面を複製する（ディープコピー用）
	public GameBoard Clone()
	{
		GameBoard clone = new GameBoard();
		clone._board = (int[,])_board.Clone();
		clone.gameStatus = gameStatus;
		clone.isBlackPerspective = isBlackPerspective;
		return clone;
	}
}