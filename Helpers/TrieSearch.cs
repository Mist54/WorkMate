using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkMate.Models;

namespace WorkMate.Helpers
{
    public class TrieSearch
    {
        private readonly TrieNode root = new TrieNode();

        /// <summary>
        /// Build the trie and reuse it 
        /// </summary>
        /// <param name="tasks"></param>
        public void BuildIndex(List<TaskModel> tasks)
        {
            foreach (var task in tasks)
            {
                if (!string.IsNullOrWhiteSpace(task.TaskName))
                {
                    Insert(task.TaskName.ToLowerInvariant().Trim(), task);
                }
            }
        }

        private void Insert(string word, TaskModel task)
        {
            TrieNode current = root;
            foreach (char c in word)
            {
                if (!current.Children.ContainsKey(c))
                {
                    current.Children[c] = new TrieNode();
                }
                current = current.Children[c];
            }
            current.IsEndWord = true;
            current.TaskReference = task;
        }

        public List<TaskModel> SearchFuzzy(string searchTerm, int maxDistance = 2)
        {
            List<TaskModel> results = new List<TaskModel>();
            HashSet<TaskModel> visited = new HashSet<TaskModel>(); 

            // Use BFS approach for better performance
            var queue = new Queue<(TrieNode node, string currentWord, int distance)>();
            queue.Enqueue((root, "", 0));

            while (queue.Count > 0)
            {
                var (currentNode, currentWord, currentDistance) = queue.Dequeue();

                // If we found a complete word and it's within distance
                if (currentNode.IsEndWord &&
                    currentNode.TaskReference != null &&
                    !visited.Contains(currentNode.TaskReference))
                {
                    int distance = CalculateLevenshteinDistance(searchTerm, currentWord);
                    if (distance <= maxDistance)
                    {
                        results.Add(currentNode.TaskReference);
                        visited.Add(currentNode.TaskReference);
                    }
                }

                // Early termination - if current distance is already too high
                if (currentDistance > maxDistance + searchTerm.Length)
                    continue;

                // Add children to queue
                foreach (var child in currentNode.Children)
                {
                    queue.Enqueue((child.Value, currentWord + child.Key, currentDistance + 1));
                }
            }

            return results.OrderBy(t => CalculateLevenshteinDistance(searchTerm, t.TaskName.ToLowerInvariant()))
                         .ToList();
        }

        private static int CalculateLevenshteinDistance(string source, string target)
        {
            if (string.IsNullOrEmpty(source))
                return string.IsNullOrEmpty(target) ? 0 : target.Length;

            if (string.IsNullOrEmpty(target))
                return source.Length;

            int sourceLength = source.Length;
            int targetLength = target.Length;

            // Use only two rows instead of full matrix to save memory
            int[] previousRow = new int[targetLength + 1];
            int[] currentRow = new int[targetLength + 1];

            // Initialize first row
            for (int i = 0; i <= targetLength; i++)
                previousRow[i] = i;

            for (int i = 1; i <= sourceLength; i++)
            {
                currentRow[0] = i;

                for (int j = 1; j <= targetLength; j++)
                {
                    int cost = (source[i - 1] == target[j - 1]) ? 0 : 1;

                    currentRow[j] = Math.Min(
                        Math.Min(currentRow[j - 1] + 1, previousRow[j] + 1),
                        previousRow[j - 1] + cost);
                }

                // Swap rows
                (previousRow, currentRow) = (currentRow, previousRow);
            }

            return previousRow[targetLength];
        }

        public TaskModel SearchExact(string word)
        {
            TrieNode current = root;
            foreach (char c in word.ToLowerInvariant())
            {
                if (!current.Children.ContainsKey(c))
                    return null;
                current = current.Children[c];
            }
            return current.IsEndWord ? current.TaskReference : null;
        }

    }

    public class TrieNode
    {
        public Dictionary<char, TrieNode> Children = new Dictionary<char, TrieNode>();
        public bool IsEndWord = false;
        public TaskModel TaskReference = null;
    }

    public class SimpleFuzzySearch
    {
        private List<TaskModel> tasks;

        public void SetTasks(List<TaskModel> taskList)
        {
            tasks = taskList ?? new List<TaskModel>();
        }

        public List<TaskModel> SearchFuzzy(string searchTerm, int maxDistance = 2)
        {
            if (string.IsNullOrWhiteSpace(searchTerm) || tasks == null)
                return new List<TaskModel>();

            var results = new List<(TaskModel task, int distance)>();
            string normalizedSearch = searchTerm.ToLowerInvariant().Trim();

            foreach (var task in tasks)
            {
                if (string.IsNullOrWhiteSpace(task.TaskName))
                    continue;

                string normalizedTaskName = task.TaskName.ToLowerInvariant().Trim();
                int distance = CalculateLevenshteinDistance(normalizedSearch, normalizedTaskName);

                if (distance <= maxDistance)
                {
                    results.Add((task, distance));
                }
            }

            return results.OrderBy(r => r.distance)
                         .ThenBy(r => r.task.TaskName)
                         .Select(r => r.task)
                         .ToList();
        }

        private static int CalculateLevenshteinDistance(string source, string target)
        {
            if (source == target) return 0;
            if (source.Length == 0) return target.Length;
            if (target.Length == 0) return source.Length;

            int[] previousRow = new int[target.Length + 1];
            for (int i = 0; i <= target.Length; i++)
                previousRow[i] = i;

            for (int i = 0; i < source.Length; i++)
            {
                int[] currentRow = new int[target.Length + 1];
                currentRow[0] = i + 1;

                for (int j = 0; j < target.Length; j++)
                {
                    int cost = source[i] == target[j] ? 0 : 1;
                    currentRow[j + 1] = Math.Min(
                        Math.Min(currentRow[j] + 1, previousRow[j + 1] + 1),
                        previousRow[j] + cost);
                }

                previousRow = currentRow;
            }

            return previousRow[target.Length];
        }
    }


   
}