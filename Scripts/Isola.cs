using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 【Isolaのルール】
/// 駒を動かして、どこかのマスを取り除く。
/// どちらかの駒が動けなくなったら負け。
/// 【駒の動き】
/// 斜めにも動ける。
/// パスはできない。
/// </summary>
class Player
{
	static void Main(string[] args)
	{
		// ゲーム状態の初期化
		GameState gameState = new GameState();
		gameState.Init();
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
			}

			// 自分の手を決定する
			var move = MonteCarloAI.GetMonteCarloBestMove(gameState, 100);

			// 自分の手を反映して局面を進める
			gameState.Advance(move);
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
	private int[,] board = new int[8, 8];
	// 8方向の移動（左上から反時計回り）
	public static int[,] dir = new int[8, 2] { { -1, -1 }, { -1, 0 }, { -1, 1 }, { 0, -1 }, { 0, 1 }, { 1, -1 }, { 1, 0 }, { 1, 1 } };

	// ゲーム状態の初期化
	public void Init()
	{
		// プレイヤー位置の初期化
		agents[0] = new Agent(0, 4);
		agents[1] = new Agent(7, 4);

		// 盤面の初期化
		for (int i = 0; i < 8; i++)
		{
			for (int j = 0; j < 8; j++)
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
			if (nextX < 0 || nextX >= 8 || nextY < 0 || nextY >= 8)
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
			for (int y = 0; y < 8; y++)
			{
				for (int x = 0; x < 8; x++)
				{
					removeX = x;
					removeY = y;
					// 相手の駒の四方2マス以内かチェック
					if (Math.Abs(x - agents[1].posX) > 2 || Math.Abs(y - agents[1].posY) > 2)
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

	// ディープコピー用メソッド
	public GameState Clone()
	{
		GameState clone = new GameState();
		clone.agents = new Agent[2] { new Agent(this.agents[0].posX, this.agents[0].posY), new Agent(this.agents[1].posX, this.agents[1].posY) };
		clone.board = (int[,])this.board.Clone();
		return clone;
	}
}

/// <summary>
/// モンテカルロ木探索AI
/// </summary>
public class MonteCarloAI
{
	// ランダムに手を選択
	public static ValueTuple<int, int, int, int> GetRandomMove(GameState gameState)
	{
		var moves = gameState.GetLegalMoves();
		var random = new Random();
		var index = random.Next(moves.Count);
		var randomMove = moves[index];
		return randomMove;
	}
	// モンテカルロ木探索で手を選択
	public static ValueTuple<int, int, int, int> GetMonteCarloBestMove(GameState gameState, int playOutNumber)
	{
		Node rootNode = new Node(gameState);
		rootNode.Expand();
		for (int i = 0; i < playOutNumber; i++)
		{
			rootNode.Evaluate();
		}
		var legalMoves = gameState.GetLegalMoves();
		var bestMoveSearchCount = -1;
		var bestMoveIndex = -1;

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

	// プレイアウト用メソッド
	public static float PlayOut(GameState gameState)
	{
		switch (gameState.GetStatus())
		{
			case -1:
				// 負け
				return 0.0f;
			default:
				var nextState = gameState.Clone();
				var legalMoves = nextState.GetLegalMoves();
				var move = GetRandomMove(nextState);
				nextState.Advance(move);
				// 再帰的にプレイアウト（相手ターンでの評価が負けの場合は1.0fが返る）
				return 1.0f - PlayOut(nextState);
		}
	}
}

/// <summary>
/// 探索時のノードを表すクラス
/// </summary>
public class Node
{
	private GameState gameState;
	private float w; // 累積価値
	public List<Node> children = new List<Node>();
	public int visitCount = 0;
	private int expandThreshold = 10;

	public Node(GameState gameState)
	{
		this.gameState = gameState;
		this.w = 0.0f;
		this.visitCount = 0;
	}

	// ノードの評価
	public float Evaluate()
	{
		// ゲームが終了している場合
		if (this.gameState.isDone())
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
			float value = MonteCarloAI.PlayOut(newGameState);
			this.w += value;
			this.visitCount += 1;

			// 一定回数以上の訪問回数の場合は子ノードを展開
			if (this.visitCount > this.expandThreshold)
			{
				this.Expand();
			}

			return value;
		}
		else
		{
			// 子ノードがある場合
			float value = 1.0f - this.Select().Evaluate();
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
			var ucb = 1.0f - child.w / child.visitCount + 2.0f * (float)Math.Sqrt(Math.Log(t) / child.visitCount);
			if (ucb > bestScore)
			{
				bestScore = ucb;
				bestIndex = i;
			}
		}

		return this.children[bestIndex];

	}
}