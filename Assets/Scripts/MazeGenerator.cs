using UnityEngine;
using System.Collections.Generic;

public class MazeGenerator : MonoBehaviour
{
    public GameObject wallPrefab;  // Wall prefab to be used for internal walls
    public int width = 5;          // Maze width (inside box)
    public int height = 5;         // Maze height (inside box)
    public float spacing = 1f;     // Distance between walls

    private int[,] maze;           // 2D array to store maze data
    private List<Vector2> stack = new List<Vector2>(); // Stack for backtracking
    private Vector2 currentPos;    // Current position in the maze

    void Start()
    {
        Debug.Log("Maze Generation Started");
        maze = new int[width, height]; // Initialize the maze array
        GenerateMaze();
        DrawMaze();
        // Debug.Log("Maze Generation Completed");
    }

   void GenerateMaze()
    {
        Debug.Log("Generating Maze...");

        // Step 1: Initialize the maze grid with walls (1 = wall, 0 = path)
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                maze[x, y] = 1; // Start with all cells as walls
            }
        }

        // Step 2: Set the starting point to a path (0)
        currentPos = new Vector2(1, 1); // Starting position
        maze[(int)currentPos.x, (int)currentPos.y] = 0; // Set the starting point as a path

        // Step 3: Use a stack for the backtracking algorithm
        stack.Add(currentPos);

        // Step 4: Generate the maze using backtracking
        while (stack.Count > 0)
        {
            currentPos = stack[stack.Count - 1]; // Get the last position from the stack
            stack.RemoveAt(stack.Count - 1); // Pop the position from the stack

            List<Vector2> neighbors = GetUnvisitedNeighbors(currentPos);

            // Step 5: If there are unvisited neighbors, continue exploring
            if (neighbors.Count > 0)
            {
                stack.Add(currentPos); // Push the current position back to the stack

                // Randomly choose a neighbor to carve a path
                Vector2 chosenNeighbor = neighbors[Random.Range(0, neighbors.Count)];

                // Remove the wall (mark it as a path)
                maze[(int)chosenNeighbor.x, (int)chosenNeighbor.y] = 0;

                // Add the chosen neighbor to the stack for further exploration
                stack.Add(chosenNeighbor);
            }
        }

        // Log the maze grid to verify maze layout
        for (int x = 0; x < width; x++)
        {
            string row = "";
            for (int y = 0; y < height; y++)
            {
                row += maze[x, y] + " "; // Log the row of the maze
            }
            Debug.Log(row); // Log the whole row for debugging
        }
    }

    List<Vector2> GetUnvisitedNeighbors(Vector2 position)
    {
        List<Vector2> neighbors = new List<Vector2>();

        // Check the 4 possible directions (up, down, left, right)
        Vector2[] directions = new Vector2[]
        {
            new Vector2(0, 1), // Up
            new Vector2(0, -1), // Down
            new Vector2(-1, 0), // Left
            new Vector2(1, 0),  // Right
        };

        foreach (Vector2 direction in directions)
        {
            Vector2 neighbor = position + direction;

            // Ensure the neighbor is within bounds and is a wall (1)
            if (neighbor.x >= 0 && neighbor.x < width && neighbor.y >= 0 && neighbor.y < height)
            {
                if (maze[(int)neighbor.x, (int)neighbor.y] == 1) // If the cell is still a wall
                {
                    neighbors.Add(neighbor);
                }
            }
        }

        return neighbors;
    }





    // Draw the maze by instantiating only internal wall prefabs
    void DrawMaze()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x * spacing, 0.5f, y * spacing); // Position for the tile

                // Only create internal walls (no outer walls)
                if (maze[x, y] == 1)
                {
                    Debug.Log("Placing wall at " + position);
                    Instantiate(wallPrefab, position, Quaternion.identity);
                }
            }
        }
    }
    
}
