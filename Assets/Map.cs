using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Xeltica.Kurutto
{
	[ExecuteInEditMode]
	public class Map : MapBase
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

		public override Vector2 BallLocation => ballLocation;

		readonly List<GameObject> blocks = new List<GameObject>();

		BlockInfo[][] blockMap;

		Vector2 Offset => blockMap == null ? default(Vector2) : new Vector2(blockMap.Max(s => s.Length) / 2 - .5f, blockMap.Length / 2 + .5f);

		private void Update()
		{
			if (mapDefinition == prevMapDefinition)
				return;
			PopulateMapInfo();
			prevMapDefinition = mapDefinition;
		}

		private void OnDrawGizmos()
		{
			if (blockMap == null) return;

			var offset = Offset;
			for (int y = 0; y < blockMap.Length; y++)
			{
				var line = blockMap[y];
				for (int x = 0; x < line.Length; x++)
				{
					switch (line[x].Kind)
					{
						case BlockKind.Air:
							continue;
						case BlockKind.Static:
							Gizmos.color = Color.magenta;
							break;
						case BlockKind.Force:
							Gizmos.color = Color.yellow;
							break;
						case BlockKind.Warp:
							Gizmos.color = Color.blue;
							break;
						case BlockKind.Acceleration:
							Gizmos.color = Color.gray;
							break;
					}

					Gizmos.DrawCube(new Vector2(x, blockMap.Length - y) - offset, Vector3.one);
				}
			}

			Gizmos.color = Color.white;
			Gizmos.DrawSphere(BallLocation, 0.4f);
		}

		public void PopulateMapInfo()
		{
			var def = mapDefinition.ToUpper().Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');

			if (def.Length > 0 && def.Any(s => s.Length % 2 != 0))
				return;
			var mapLines = new List<BlockInfo[]>();
			for (int y = 0; y < def.Length; y++)
			{
				var mapLine = new List<BlockInfo>();
				var line = def[y];
				for (int x = 0; x < line.Length; x += 2)
				{
					var info = new BlockInfo();
					switch (line[x])
					{
						default:
						case '0':
							info.Kind = BlockKind.Air;
							break;
						case 'S':
							info.Kind = BlockKind.Static;
							break;
						case 'F':
							info.Kind = BlockKind.Force;
							break;
						case 'W':
							info.Kind = BlockKind.Warp;
							break;
						case 'A':
							info.Kind = BlockKind.Acceleration;
							break;
					}
					
					switch (line[x + 1])
					{
						default:
						case 'U':
							info.Facing = BlockFacing.Up;
							break;
						case 'L':
							info.Facing = BlockFacing.Left;
							break;
						case 'D':
							info.Facing = BlockFacing.Down;
							break;
						case 'R':
							info.Facing = BlockFacing.Right;
							break;
					}
					mapLine.Add(info);
				}
				mapLines.Add(mapLine.ToArray());
			}
			blockMap = mapLines.ToArray();
		}


		public override void PopulateMapObject()
		{
			PopulateMapInfo();

			var offset = Offset;

			blocks.ForEach(Destroy);
			blocks.Clear();

			for (int y = 0; y < blockMap.Length; y++)
			{
				var line = blockMap[y];
				for (int x = 0; x < line.Length; x++)
				{
					Block theBlock;
					switch (line[x].Kind)
					{
						default:
						case BlockKind.Air:
							continue;
						case BlockKind.Static:
							theBlock = GameMaster.Instance.StaticBlock;
							break;
						case BlockKind.Force:
							theBlock = GameMaster.Instance.ForceBlock;
							break;
						case BlockKind.Warp:
							theBlock = GameMaster.Instance.WarpBlock;
							break;
						case BlockKind.Acceleration:
							theBlock = GameMaster.Instance.AccelerationBlock;
							break;
					}

					float angle;
					switch (line[x].Facing)
					{
						default:
						case BlockFacing.Up:
							angle = 0;
							break;
						case BlockFacing.Left:
							angle = 90;
							break;
						case BlockFacing.Down:
							angle = 180;
							break;
						case BlockFacing.Right:
							angle = 270;
							break;
					}
					var instance = Instantiate(theBlock.gameObject, transform);
					instance.transform.localPosition = new Vector2(x, blockMap.Length - y) - offset;
					instance.transform.eulerAngles = new Vector3(0, 0, angle);
					blocks.Add(instance);
				}

			}
		}
	}

	public struct BlockInfo
	{
		public BlockKind Kind { get; set; }
		public BlockFacing Facing { get; set; }
	}

	public enum BlockKind
	{
		Air, Static, Force, Warp, Acceleration
	}

	public enum BlockFacing
	{
		Up, Left, Down, Right
	}

	public abstract class MapBase : MonoBehaviour
	{
		public abstract void PopulateMapObject();

		public abstract Vector2 BallLocation { get; }

	}

}