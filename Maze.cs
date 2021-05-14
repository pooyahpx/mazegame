using System;
using System.Collections;
using System.Drawing;

/*
 * create a CellStack (LIFO) to hold a list of cell locations  
set TotalCells = number of cells in grid  
choose a cell at random and call it CurrentCell  
set VisitedCells = 1  
   
while VisitedCells < TotalCells  
find all neighbors of CurrentCell with all walls intact   
if one or more found  
choose one at random  
knock down the wall between it and CurrentCell  
push CurrentCell location on the CellStack  
make the new cell CurrentCell  
add 1 to VisitedCells 
else  
pop the most recent cell entry off the CellStack  
make it CurrentCell 
endIf 
endWhile  
*/

namespace DFSAlgorithmMaze
{
	/// <summary>
	/// Summary description for Maze.
	/// </summary>
	public class Maze
	{
		public static int kDimension = 50;
		Cell[, ] Cells = null;
		Stack CellStack = new Stack();
		int VisitedCells = 1;
		int TotalCells = kDimension * kDimension;
		Cell CurrentCell = null;

		public Maze()
		{
			//
			// TODO: Add constructor logic here
			//
			Initialize();
		}

		public Point GetCellCenter(int cellNumberX, int cellNumberY)
		{
			return Cells[cellNumberX, cellNumberY].CellCenter();
		}

		public Cell GetCellFromPoint(int x, int y)
		{
			int cellX = (x - Cell.kPadding)/Cell.kCellSize;
			int cellY = (y - Cell.kPadding)/Cell.kCellSize;
			return Cells[cellX, cellY];
		}

		private ArrayList GetNeighborsWithWalls(Cell aCell)
		{
			ArrayList Neighbors = new ArrayList();
			int count = 0;
			for (int countRow = -1; countRow <= 1; countRow++)
				for (int countCol = -1; countCol <= 1; countCol++)
				{
					if ( (aCell.Row + countRow < kDimension) &&  
						 (aCell.Column+countCol < kDimension) &&
						 (aCell.Row+countRow >= 0) &&
						 (aCell.Column+countCol >= 0) &&
						 ((countCol == 0) || (countRow == 0)) &&
						 (countRow != countCol)
						)
					{
						if (Cells[aCell.Row+countRow, aCell.Column+countCol].HasAllWalls())
						{
							Neighbors.Add( Cells[aCell.Row+countRow, aCell.Column+countCol]);
						}
					}
				}

			return Neighbors;
		}

		public void Initialize()
		{
			Cells = new Cell[kDimension, kDimension];
			TotalCells = kDimension * kDimension;
			for (int i = 0; i < kDimension; i++)
				for (int j = 0; j < kDimension; j++)
				{
					Cells[i,j] =  new Cell();
					Cells[i, j].Row = i;
					Cells[i, j].Column = j;
				}

			CurrentCell = Cells[0,0];
			VisitedCells = 1;
			CellStack.Clear();
		}

		public void  Generate()
		{
			while (VisitedCells < TotalCells)
			{
				// get a list of the neighboring cells with all walls intact
				ArrayList AdjacentCells = GetNeighborsWithWalls(CurrentCell);
				// test if a cell like this exists
				if (AdjacentCells.Count > 0)
				{
					// yes, choose one of them, and knock down the wall between it and the current cell
					int randomCell = Cell.TheRandom.Next(0, AdjacentCells.Count);
					Cell theCell = ((Cell)AdjacentCells[randomCell]);
					CurrentCell.KnockDownWall(theCell);
					CellStack.Push(CurrentCell); // push the current cell onto the stack
					CurrentCell = theCell; // make the random neighbor the new current cell
					VisitedCells++;
				}
				else
				{
					// No cells with walls intact, pop current cell from stack
					CurrentCell = (Cell)CellStack.Pop();
				}

			}
		}


		public void Draw(Graphics g)
		{
			for (int i = 0; i < kDimension; i++)
				for (int j = 0; j < kDimension; j++)
				{
					Cells[i,j].Draw(g);
				}
		}
	}
}
