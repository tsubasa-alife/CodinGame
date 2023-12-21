using System;
using System.Threading;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// ThunderサーチAIによるIsolaのプレイ
/// </summary>
class Player
{
	static void Main(string[] args)
	{
		// ゲーム状態の初期化
		GameState gameState = new GameState();
		gameState.Init();
		int turn = 0;
		// プレイヤーの座標
		int playerPositionX = int.Parse(Console.ReadLine());
		int playerPositionY = int.Parse(Console.ReadLine());

		// ゲームループ
		while (true)
		{
			// 相手の座標
			int opponentPositionX = int.Parse(Console.ReadLine());
			int opponentPositionY = int.Parse(Console.ReadLine());
			// 相手が取り除いたタイルの座標
			int opponentLastRemovedTileX = int.Parse(Console.ReadLine());
			int opponentLastRemovedTileY = int.Parse(Console.ReadLine());
			// 相手が先攻の場合
			if (opponentLastRemovedTileX != -1 && opponentLastRemovedTileY != -1)
			{
				// 相手の手を反映して局面を進める
				gameState.Advance(new ValueTuple<int, int, int, int>(opponentPositionX, opponentPositionY, opponentLastRemovedTileX, opponentLastRemovedTileY));
				turn++;
			}

			ValueTuple<int, int, int, int> move = new ValueTuple<int, int, int, int>(0, 0, 0, 0);
			// 自分の手を決定する
			// 10ターン目まではルールベースAIで選択
			if (turn < 20)
			{
				move = RuleBaseAI.GetRuleBaseBestMove(gameState);
			}
			else
			{
				// 10ターン目以降はThunderサーチAIで選択
				move = ThunderSearchAI.GetThunderSearchBestMove(gameState, 300);
			}

			// 現在の局面を出力
			Console.Error.WriteLine(gameState.ToString());

			// 自分の手を反映して局面を進める
			gameState.Advance(move);
			turn++;
			// 自分の手を送信する
			var moveMessage = move.Item1.ToString() + " " + move.Item2.ToString() + " " + move.Item3.ToString() + " " + move.Item4.ToString();
			Console.Error.WriteLine(moveMessage);
			Console.WriteLine(moveMessage + ";MESSAGE");
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

	public Agent(int x, int y)
	{
		posX = x;
		posY = y;
	}

}

/// <summary>
/// ゲーム盤面状態
/// </summary>
public class GameState
{
	private Agent[] agents = new Agent[2];
	public int[,] board = new int[9, 9];
	// 8方向の移動（左上から反時計回り）
	public static int[,] dir = new int[8, 2] { { -1, -1 }, { -1, 0 }, { -1, 1 }, { 0, -1 }, { 0, 1 }, { 1, -1 }, { 1, 0 }, { 1, 1 } };

	// ゲーム状態の初期化
	public void Init()
	{
		// プレイヤー位置の初期化
		agents[0] = new Agent(0, 4);
		agents[1] = new Agent(8, 4);

		// 盤面の初期化
		for (int i = 0; i < 9; i++)
		{
			for (int j = 0; j < 9; j++)
			{
				// 1はマスがあることを示す
				board[i, j] = 1;
			}
		}
	}

	// 合法手の取得
	public List<ValueTuple<int, int, int, int>> GetLegalMoves(int limRemoveX = -1, int limRemoveY = -1)
	{
		var legalMoves = new List<ValueTuple<int, int, int, int>>();
		var agent = agents[0];
		// 8方向に移動できるかチェック
		for (int i = 0; i < 8; i++)
		{
			int nextX = agent.posX + dir[i, 0];
			int nextY = agent.posY + dir[i, 1];
			// 盤面内チェック
			if (nextX < 0 || nextX > 8 || nextY < 0 || nextY > 8)
			{
				continue;
			}
			// 存在しているマスかチェック
			if (board[nextX, nextY] != 1)
			{
				continue;
			}
			// 相手がいるかチェック
			if (nextX == agents[1].posX && nextY == agents[1].posY)
			{
				continue;
			}

			// 取り除くマスの座標を追加
			int removeX = 0;
			int removeY = 0;
			for (int y = 0; y < 9; y++)
			{
				for (int x = 0; x < 9; x++)
				{
					removeX = x;
					removeY = y;
					// 相手の周囲2マス以内かチェック
					if (Math.Abs(removeX - agents[1].posX) > 2 || Math.Abs(removeY - agents[1].posY) > 2)
					{
						continue;
					}
					// 存在しているマスかチェック
					if (board[x, y] != 1)
					{
						continue;
					}
					// 相手がいるかチェック
					if (x == agents[1].posX && y == agents[1].posY)
					{
						continue;
					}
					// 移動先のマスかチェック
					if (x == nextX && y == nextY)
					{
						continue;
					}

					// 合法手として追加
					legalMoves.Add(new ValueTuple<int, int, int, int>(nextX, nextY, removeX, removeY));
				}
			}
		}
		return legalMoves;
	}

	// 着手によって盤面を進める
	public void Advance(ValueTuple<int, int, int, int> move)
	{
		// プレイヤーの移動
		agents[0].posX = move.Item1;
		agents[0].posY = move.Item2;
		// 移動先のマスを取り除く
		board[move.Item3, move.Item4] = 0;

		// 視点を入れ替える
		ChangePerspective();
	}

	// 視点の入れ替え用メソッド
	public void ChangePerspective()
	{
		// 視点を入れ替える（Agentsをスワップする）
		Agent tmp = agents[0];
		agents[0] = agents[1];
		agents[1] = tmp;
	}

	// ゲームの状態を取得
	public int GetStatus()
	{
		// 合法手があるかチェック
		if (GetLegalMoves().Count > 0)
		{
			// 合法手がある場合はゲーム続行
			return 0;
		}
		// 合法手がない場合は負け
		return -1;
	}

	// ゲームが終了しているかチェック
	public bool isDone()
	{
		return GetStatus() != 0;
	}

	// 指定されたプレイヤーの現在位置を取得
	public ValueTuple<int, int> GetPlayerPosition(int playerIndex)
	{
		return new ValueTuple<int, int>(agents[playerIndex].posX, agents[playerIndex].posY);
	}

	// 現在のプレイヤーから見た予想勝率を取得
	public float GetScoreRate()
	{
		// 自分の移動可能マスの数
		int myMoveCount = 0;
		var agent = agents[0];
		for (int i = 0; i < 8; i++)
		{
			int nextX = agent.posX + dir[i, 0];
			int nextY = agent.posY + dir[i, 1];
			// 盤面内チェック
			if (nextX < 0 || nextX > 8 || nextY < 0 || nextY > 8)
			{
				continue;
			}
			// 存在しているマスかチェック
			if (board[nextX, nextY] != 1)
			{
				continue;
			}
			// 相手がいるかチェック
			if (nextX == agents[1].posX && nextY == agents[1].posY)
			{
				continue;
			}
			myMoveCount++;
		}

		// 相手の移動可能マスの数
		int opponentMoveCount = 0;
		agent = agents[1];
		for (int i = 0; i < 8; i++)
		{
			int nextX = agent.posX + dir[i, 0];
			int nextY = agent.posY + dir[i, 1];
			// 盤面内チェック
			if (nextX < 0 || nextX > 8 || nextY < 0 || nextY > 8)
			{
				continue;
			}
			// 存在しているマスかチェック
			if (board[nextX, nextY] != 1)
			{
				continue;
			}
			// 相手がいるかチェック
			if (nextX == agents[0].posX && nextY == agents[0].posY)
			{
				continue;
			}
			opponentMoveCount++;
		}

		// 勝率 = 自分の移動可能マスの数/（自分の移動可能マスの数+相手の移動可能マスの数）
		if (opponentMoveCount == 0)
		{
			return 1.0f;
		}

		var rate = (float)myMoveCount / (myMoveCount + opponentMoveCount);

		return rate;
	}

	// ディープコピー用メソッド
	public GameState Clone()
	{
		GameState clone = new GameState();
		clone.agents = new Agent[2] { new Agent(this.agents[0].posX, this.agents[0].posY), new Agent(this.agents[1].posX, this.agents[1].posY) };
		clone.board = (int[,])this.board.Clone();
		return clone;
	}

	// 現在の盤面を文字列で取得
	public string ToString()
	{
		var sb = new StringBuilder();
		for (int y = 0; y < 9; y++)
		{
			for (int x = 0; x < 9; x++)
			{
				if (x == agents[0].posX && y == agents[0].posY)
				{
					sb.Append("A");
				}
				else if (x == agents[1].posX && y == agents[1].posY)
				{
					sb.Append("B");
				}
				else
				{
					sb.Append(board[x, y]);
				}
			}
			sb.Append("\n");
		}
		return sb.ToString();
	}
}

/// <summary>
/// ルールベースAI
/// </summary>
public class RuleBaseAI
{
	// ルールベースで手を選択
	public static ValueTuple<int, int, int, int> GetRuleBaseBestMove(GameState gameState)
	{
		// 合法手の中から相手に近づき、かつ相手の周囲2マス以内のマスを取り除く手を選択
		var legalMoves = gameState.GetLegalMoves();
		var bestMoveIndex = -1;
		var bestMoveDistance = 100;
		for (int i = 0; i < legalMoves.Count; i++)
		{
			var move = legalMoves[i];
			// 取り除くマスが相手の周囲1マス以内かチェック
			var removeX = move.Item3;
			var removeY = move.Item4;
			var opponentPosX = gameState.GetPlayerPosition(1).Item1;
			var opponentPosY = gameState.GetPlayerPosition(1).Item2;
			if (Math.Abs(removeX - opponentPosX) > 1 || Math.Abs(removeY - opponentPosY) > 1)
			{
				continue;
			}

			var distance = Math.Abs(move.Item1 - gameState.GetPlayerPosition(1).Item1) + Math.Abs(move.Item2 - gameState.GetPlayerPosition(1).Item2);
			if (distance < bestMoveDistance)
			{
				bestMoveDistance = distance;
				bestMoveIndex = i;
			}
		}

		return legalMoves[bestMoveIndex];

	}
}

/// <summary>
/// ThunderサーチAI
/// </summary>
public class ThunderSearchAI
{
	// Thunderサーチで手を選択
	public static ValueTuple<int, int, int, int> GetThunderSearchBestMove(GameState gameState, int playOutNumber)
	{
		// 探索開始時刻
		DateTime startTime = DateTime.Now;
		Node rootNode = new Node(gameState);
		rootNode.Expand();
		for (int i = 0; i < playOutNumber; i++)
		{
			// 経過時間
			var elapsedTime = DateTime.Now - startTime;
			// 探索時間が90msを超えたら終了
			if (elapsedTime.TotalMilliseconds > 80)
			{
				Console.Error.WriteLine("TimeOut");
				break;
			}
			rootNode.Evaluate(startTime);
		}

		// 探索後の最善手を取得
		var legalMoves = gameState.GetLegalMoves();
		Console.Error.WriteLine("合法手数: " + legalMoves.Count);
		var bestMoveSearchCount = -1;
		var bestMoveIndex = -1;

		// 最も訪問回数が多い手を最善手として選択する
		for (int i = 0; i < legalMoves.Count; i++)
		{
			int searchCount = rootNode.children[i].visitCount;
			if (searchCount > bestMoveSearchCount)
			{
				bestMoveSearchCount = searchCount;
				bestMoveIndex = i;
			}
		}

		return legalMoves[bestMoveIndex];
	}

}

/// <summary>
/// 探索時のノードを表すクラス
/// </summary>
public class Node
{
	private GameState gameState;
	public float w; // 累積価値
	public List<Node> children = new List<Node>();
	public int visitCount = 0; // 訪問回数

	public Node(GameState gameState)
	{
		this.gameState = gameState;
		this.w = 0.0f;
		this.visitCount = 0;
	}

	// ノードの評価
	public float Evaluate(DateTime startTime)
	{
		// ゲームが終了している or 探索の時間制限が来ている場合
		if (this.gameState.isDone() || (DateTime.Now - startTime).TotalMilliseconds > 80)
		{
			float value = 0.5f;
			switch (this.gameState.GetStatus())
			{
				case -1:
					value = 0.0f;
					break;
				default:
					value = 1.0f;
					break;
			}

			this.w += value;
			this.visitCount += 1;
			return value;
		}

		// 子ノードがない場合
		if (this.children.Count == 0)
		{
			// 合法手を取得
			var newGameState = this.gameState.Clone();
			float value = newGameState.GetScoreRate();
			this.w += value;
			this.visitCount += 1;

			// 必ず子ノードを展開
			this.Expand();

			return value;
		}
		else
		{
			// 子ノードがある場合
			float value = 1.0f - this.Select().Evaluate(startTime);
			this.w += value;
			this.visitCount += 1;
			return value;
		}

	}

	// 子ノードの展開
	public void Expand()
	{
		var legalMoves = this.gameState.GetLegalMoves();
		this.children = new List<Node>();
		foreach (var move in legalMoves)
		{
			var newGameState = this.gameState.Clone();
			newGameState.Advance(move);
			this.children.Add(new Node(newGameState));
		}
	}

	// 子ノードの選択
	public Node Select()
	{
		foreach (var child in this.children)
		{
			if (child.visitCount == 0)
			{
				return child;
			}
		}

		float t = 0.0f;

		foreach (var child in this.children)
		{
			t += child.visitCount;
		}

		float bestScore = -1000.0f;
		int bestIndex = -1;
		for (int i = 0; i < this.children.Count; i++)
		{
			var child = this.children[i];
			var winRate = 1.0f - child.w / child.visitCount;
			if (winRate > bestScore)
			{
				bestScore = winRate;
				bestIndex = i;
			}
		}

		return this.children[bestIndex];

	}
}