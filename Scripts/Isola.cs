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

			// 自分の手を決定して局面を進める
			var move = GetRandomMove(gameState);
			gameState.Advance(move);
			var moveMessage = move.Item1.ToString() + " " + move.Item2.ToString() + " " + move.Item3.ToString() + " " + move.Item4.ToString();
			Console.Error.WriteLine(moveMessage);
			Console.WriteLine(moveMessage + ";MESSAGE");
		}
	}

	// ランダムに手を選択
	private static ValueTuple<int, int, int, int> GetRandomMove(GameState gameState)
	{
		var moves = gameState.GetLegalMoves();
		var random = new Random();
		var index = random.Next(moves.Count);
		var randomMove = moves[index];
		return randomMove;
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
	public List<ValueTuple<int, int, int, int>> GetLegalMoves()
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

	public void ChangePerspective()
	{
		// 視点を入れ替える（Agentsをスワップする）
		Agent tmp = agents[0];
		agents[0] = agents[1];
		agents[1] = tmp;
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