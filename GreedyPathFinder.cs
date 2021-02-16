using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Greedy.Architecture;
using Greedy.Architecture.Drawing;

namespace Greedy
{
	public class GreedyPathFinder : IPathFinder
	{
		public List<Point> FindPathToCompleteGoal(State state)
		{
			if (state.Goal == 0)
				return new List<Point>();

			HashSet<Point> chests = new HashSet<Point>(state.Chests);
			DijkstraPathFinder finder = new DijkstraPathFinder();
			List<Point> result = new List<Point>();

			var currentCost = 0;
			var position = state.Position;

			for (int i = 0; i < state.Goal; i++)
			{
				if (!chests.Any())
					return new List<Point>();
				PathWithCost pathToNewChest = finder.GetPathsByDijkstra(state, position, chests).FirstOrDefault();
				if (pathToNewChest == null)
					return new List<Point>();
				position = pathToNewChest.End;
				currentCost += pathToNewChest.Cost;
				if (currentCost > state.Energy)
					return new List<Point>();
				chests.Remove(pathToNewChest.End);

				for (int j = 1; j < pathToNewChest.Path.Count; j++)
					result.Add(pathToNewChest.Path[j]);
			}

			return result;
		}
	}
}