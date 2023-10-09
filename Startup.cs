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

		while (true)
		{
			int id = int.Parse(Console.ReadLine()); // プレイヤーID
			int playerCount = int.Parse(Console.ReadLine()); // プレイヤー数
			int turn = int.Parse(Console.ReadLine()); // 経過ターン数
			int income = int.Parse(Console.ReadLine()); // 収入
			int cash = int.Parse(Console.ReadLine()); // 資産
			int devs = int.Parse(Console.ReadLine()); // 開発者数
			int sellers = int.Parse(Console.ReadLine()); // 営業数
			int managers = int.Parse(Console.ReadLine()); // マネージャー数
			int features = int.Parse(Console.ReadLine()); // 機能数：10以上必要
			int tests = int.Parse(Console.ReadLine()); // テスト数：機能数の4倍あるとバグが発生しなくなる
			int bugs = int.Parse(Console.ReadLine()); // ソフトウェアのバグ数
			int bestPlayerId = -1; // マーケットシェア1位のプレイヤーID(自分以外)
			int myMarketShare = 0; // 自分のマーケットシェア
			for (int i = 0; i < playerCount; i++)
			{
				string[] inputs = Console.ReadLine().Split(' ');
				int startUpId = int.Parse(inputs[0]); // ID
				int marketShare = int.Parse(inputs[1]); // マーケットシェア
				int reputation = int.Parse(inputs[2]); // 知名度
				int bestPlayerShare = 0;
				if (startUpId == id)
				{
					myMarketShare = marketShare;
					continue;
				}

				if (bestPlayerShare < marketShare)
				{
					bestPlayerShare = marketShare;
					bestPlayerId = startUpId;
				}
			}

			// Write an action using Console.WriteLine()
			// To debug: Console.Error.WriteLine("Debug messages...");
			Console.Error.WriteLine("シェア：" + myMarketShare);
			Console.Error.WriteLine("収入：" + income);
			Console.Error.WriteLine("資産：" + cash);
			Console.Error.WriteLine("開発者数：" + devs);
			Console.Error.WriteLine("営業数：" + sellers);
			Console.Error.WriteLine("マネージャー数：" + managers);
			Console.Error.WriteLine("機能数：" + features);
			Console.Error.WriteLine("テスト数：" + tests);
			Console.Error.WriteLine("バグ数：" + bugs);

			// 行動を決定
			// 開発者、営業、マネージャーを雇う数＋メンテナンス開発者＋シェアを奪うための営業数（＋ターゲットID）
			int devsToHire = 0;
			int sellerToHire = 0;
			int managersToHire = 0;
			int maintenanceDevs = 0;
			int competitiveSellers = 0;
			int targetId = -1;
			
			// 機能が10以上かどうかで戦略を変える
			if (features >= 10)
			{
				// 販売ができるようになった後はテスト数が機能数の4倍になるまでは開発者を雇う
				if (features * 4 > tests)
				{
					devsToHire = Math.Min(2, features * 4 - tests);
					// 販売が開始されたら開発者を全員メンテナンスに回す
					maintenanceDevs = devs;
				}
				else
				{
					devsToHire = 2 - devs;
					// 全員メンテナンスに回す
					maintenanceDevs = 2;
				}
				
				int sellerHireLimit = managers * 2 - devsToHire;
				// 販売ができるようになったら営業を雇う
				if (myMarketShare < 100)
				{
					sellerToHire = Math.Min(2, sellerHireLimit);
					competitiveSellers = sellers / 2;
				}
				else if (myMarketShare < 300)
				{
					sellerToHire = Math.Min(10, sellerHireLimit);
					competitiveSellers = sellers * 2 / 3;
				}
				else if (myMarketShare < 500)
				{
					sellerToHire = Math.Min(50, sellerHireLimit);
					competitiveSellers = sellers * 3 / 4;
				}
				else 
				{
					sellerToHire = Math.Min(100, sellerHireLimit);
					competitiveSellers = sellers * 4 / 5;
				}
				
				
				targetId = bestPlayerId;
			}
			else
			{
				devsToHire = 2;
				maintenanceDevs = devs * 4 / 5;
			}

			// マネージャーは従業員4人につき1人
			if (managers <= (devs + sellers + devsToHire + sellerToHire)/4 || managers == 0)
			{
				managersToHire = 1;
			}

			// 行動出力
			string action = GetAction(devsToHire, sellerToHire, managersToHire, maintenanceDevs, competitiveSellers, targetId);
			Console.WriteLine(action);
		}
	}

	private static string GetAction(int devsToHire, int sellerToHire, int managersToHire, int maintenanceDevs, int competitiveSellers, int targetId = -1)
	{
		if (targetId == -1)
		{
			return devsToHire + " " + sellerToHire + " " + managersToHire + " " + maintenanceDevs + " " + competitiveSellers;
		}
		else
		{
			return devsToHire + " " + sellerToHire + " " + managersToHire + " " + maintenanceDevs + " " + competitiveSellers + " " + targetId;
		}
	}
}