using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Xeltica.Kurutto
{
	[ExecuteInEditMode]
	public class Map : MonoBehaviour, IMap
	{
		const string help = 
@"マップ定義文字列を記述します。
マップ定義は、2文字で1つのブロックを定義します。
1文字目はブロックの種類を表します...
0: 空
S: スタティックブロック
F: 力ブロック
W: ワープブロック
A: 一方通行ゲート

2文字目はブロックの初期方向を表します...
L: 左
U: 上
R: 右
D: 下
";

		[SerializeField]
		Vector2 ballLocation;

		[SerializeField]
		[TextArea(1, 20)]
		[Tooltip(help)]
		string mapDefinition;


		string prevMapDefinition;

		public Vector2 BallLocation => ballLocation;

		readonly List<GameObject> blocks = new List<GameObject>();


		private void OnValidate()
		{
			if (mapDefinition == prevMapDefinition)
				return;

			PopulateMapObject();

			prevMapDefinition = mapDefinition;
		}

		public void PopulateMapObject()
		{
			var def = mapDefinition.ToUpper().Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');

			if (def.Length > 0 && def.Any(s => s.Length % 2 != 0))
				return;

			var offset = new Vector2(def.Max(s => s.Length) / 2, def.Length / 2);

			blocks.ForEach(Destroy);
			blocks.Clear();

			for (int y = 0; y < def.Length; y++)
			{
				var line = def[y];
				for (int x = 0; x < line.Length; x += 2)
				{
					Block theBlock;
					switch (line[x])
					{
						default:
						case '0':
							continue;
						case 'S':
							theBlock = GameMaster.Instance.StaticBlock;
							break;
						case 'F':
							theBlock = GameMaster.Instance.ForceBlock;
							break;
						case 'W':
							theBlock = GameMaster.Instance.WarpBlock;
							break;
						case 'A':
							theBlock = GameMaster.Instance.AccelerationBlock;
							break;
					}

					float angle;
					switch (line[x + 1])
					{
						default:
						case 'U':
							angle = 0;
							break;
						case 'L':
							angle = 90;
							break;
						case 'D':
							angle = 180;
							break;
						case 'R':
							angle = 270;
							break;
					}
					var instance = Instantiate(theBlock.gameObject, transform);
					instance.transform.localPosition = offset + new Vector2(x, def.Length - y);
					instance.transform.eulerAngles = new Vector3(0, 0, angle);
					blocks.Add(instance);
				}

			}
		}


	}

	public interface IMap
	{
		void PopulateMapObject();

		Vector2 BallLocation { get; }

	}

}