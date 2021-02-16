using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Greedy.Architecture;
using System.Drawing;

namespace Greedy
{
	public class DijkstraData
	{
		public Point? Previous { get; set; }
		public int Price { get; set; }
	}
	public class DijkstraPathFinder
	{
		public IEnumerable<PathWithCost> GetPathsByDijkstra(State state, Point start, IEnumerable<Point> targets)
		{
			HashSet<Point> chests = new HashSet<Point>(targets);
			HashSet<Point> candidatesToOpen = new HashSet<Point>();
			HashSet<Point> visitedNodes = new HashSet<Point>();

			var track = new Dictionary<Point, DijkstraData>();
			track[start] = new DijkstraData { Price = 0, Previous = null };

			candidatesToOpen.Add(start);

			while (true)
			{
				Point? toOpen = null;
				var bestPrice = int.MaxValue;
				foreach (var candidate in candidatesToOpen)
					if (track[candidate].Price < bestPrice)
					{
						bestPrice = track[candidate].Price;
						toOpen = candidate;
					}

				if (toOpen == null) yield break;
				if (chests.Contains(toOpen.Value)) yield return MakePath(track, toOpen.Value);

				var incidentNodes = GetIncidentNodes(toOpen.Value, state);

				foreach (var incidentNode in incidentNodes)
				{
					var currentPrice = track[toOpen.Value].Price + state.CellCost[incidentNode.X, incidentNode.Y];
					if (!track.ContainsKey(incidentNode) || track[incidentNode].Price > currentPrice)
					{
						track[incidentNode] = new DijkstraData { Previous = toOpen, Price = currentPrice };
					}
					if (!visitedNodes.Contains(incidentNode))
						candidatesToOpen.Add(incidentNode);
				}

				candidatesToOpen.Remove(toOpen.Value);
				visitedNodes.Add(toOpen.Value);
			}
		}

		public PathWithCost MakePath(Dictionary<Point, DijkstraData> track, Point end)
		{
			var result = new List<Point>();
			Point? currentPoint = end;
			while (currentPoint != null)
			{
				result.Add(currentPoint.Value);
				currentPoint = track[currentPoint.Value].Previous;
			}
			result.Reverse();
			PathWithCost pathResult = new PathWithCost(track[end].Price, result.ToArray());
			return pathResult;
		}

		public IEnumerable<Point> GetIncidentNodes(Point node, State state)
		{
			return new Point[]
			{
				new Point(node.X, node.Y+1),
				new Point(node.X, node.Y-1),
				new Point(node.X+1, node.Y),
				new Point(node.X-1, node.Y)
			}.Where(point => state.InsideMap(point) && !state.IsWallAt(point));
		}
	}
}
