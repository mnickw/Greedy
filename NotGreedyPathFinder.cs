using System.Collections.Generic;
using System.Drawing;
using Greedy.Architecture;
using Greedy.Architecture.Drawing;
using System.Linq;
using System;

namespace Greedy
{
    public class NotGreedyPathFinder : IPathFinder
    {
        public List<Point> FindPathToCompleteGoal(State state)
        {
            DijkstraPathFinder finder = new DijkstraPathFinder();
            Stack<PathWithCost> pathsThroughAllChests = new Stack<PathWithCost>();
            HashSet<Point> notUsedChests = new HashSet<Point>(state.Chests);
            Dictionary<(Point start, Point end), PathWithCost> dictionaryOfPaths = new Dictionary<(Point start, Point end), PathWithCost>();
            List<PathWithCost> bestPath = null;

            foreach (var chest in state.Chests)
            {
                var result = FindPathByFirstChest(notUsedChests, state.Position, chest, 0,
                    state, pathsThroughAllChests, dictionaryOfPaths, finder, ref bestPath);
                if (result != null)
                    return result;
            }

            if (bestPath != null)
                return MakePath(bestPath);
            return new List<Point>();
        }

        List<Point> FindPathByFirstChest(HashSet<Point> notUsedChests, Point previousNode, Point currentChest,
            int cost, State state, Stack<PathWithCost> pathsThroughAllChests,
            Dictionary<(Point start, Point end), PathWithCost> dictionaryOfPaths,
            DijkstraPathFinder finder, ref List<PathWithCost> bestPath)
        {
            PathWithCost pathToCurrentChest = PathToNode(previousNode, currentChest, dictionaryOfPaths, state, finder);
            if (pathToCurrentChest == null)
                return null;
            cost += pathToCurrentChest.Cost;
            notUsedChests.Remove(currentChest);
            pathsThroughAllChests.Push(pathToCurrentChest);
            if (cost <= state.Energy && notUsedChests.Count > 0)
                foreach (var newCurrentChest in notUsedChests.ToList())
                {
                    var result = FindPathByFirstChest(notUsedChests, currentChest, newCurrentChest, cost,
                        state, pathsThroughAllChests, dictionaryOfPaths, finder, ref bestPath);
                    if (result != null)
                        return result;
                }
            else
            {
                if (cost <= state.Energy && notUsedChests.Count == 0)
                    return MakePath(pathsThroughAllChests.ToList());
                else
                {
                    notUsedChests.Add(currentChest);
                    pathsThroughAllChests.Pop();
                    CheckBestPath(ref bestPath, pathsThroughAllChests);
                    return null;
                }
            }
            notUsedChests.Add(currentChest);
            pathsThroughAllChests.Pop();
            return null;
        }

        private void CheckBestPath(ref List<PathWithCost> bestPath, Stack<PathWithCost> pathsThroughAllChests)
        {
            if (bestPath == null || bestPath.Count < pathsThroughAllChests.Count)
                bestPath = pathsThroughAllChests.ToList();
        }

        private List<Point> MakePath(List<PathWithCost> pathsThroughAllChests)
        {
            var result = new List<Point>();
            for (int i = pathsThroughAllChests.Count - 1; i >= 0; i--)
            {
                var path = pathsThroughAllChests[i];
                for (int j = 1; j < path.Path.Count; j++)
                    result.Add(path.Path[j]);
            }
            return result;
        }

        private PathWithCost PathToNode(Point previousNode, Point currentChest,
            Dictionary<(Point start, Point end), PathWithCost> dictionaryOfPaths, State state, DijkstraPathFinder finder)
        {
            if (dictionaryOfPaths.TryGetValue((previousNode, currentChest), out var path))
                return path;
            path = finder.GetPathsByDijkstra(state, previousNode, new[] { currentChest }).FirstOrDefault();
            dictionaryOfPaths[(previousNode, currentChest)] = path;
            return path;
        }
    }
}