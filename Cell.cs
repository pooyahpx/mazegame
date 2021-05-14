using System;
using System.Drawing;

namespace DFSAlgorithmMaze
{
	/// <summary>
	/// Summary description for Cell.
	/// </summary>
	public class Cell
	{
		public static int kCellSize = 10;
		public static int kPadding = 5;
		public int[] Walls  = new int[4]{1, 1, 1, 1};
		public int Row;
		public int Column;
		private static long Seed = 	DateTime.Now.Ticks;
		static public Random TheRandom = new Random((int)Cell.Seed);
		
		
		public Cell()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public bool HasAllWalls()
		{
			for (int i = 0; i < 4; i++)
			{
				 if (Walls[i] == 0)
					 return false;
			}

			return true;
		}

		public void KnockDownWall(int theWall)
		{
			Walls[theWall] = 0;
		}

		public void KnockDownWall(Cell theCell)
		{
			// find adjacent wall
			int theWallToKnockDown = FindAdjacentWall(theCell);
			Walls[theWallToKnockDown] = 0;
			int oppositeWall = (theWallToKnockDown + 2) % 4;
			theCell.Walls[oppositeWall] = 0;
		}


		public  int FindAdjacentWall(Cell theCell)
		{
			if (theCell.Row == Row) 
			{
				if (theCell.Column < Column)
					return 0;
				else
					return 2;
			}
			else // columns are the same
			{
				if (theCell.Row < Row)
					return 1;
				else
					return 3;
			}
		}

		public int GetRandomWall()
		{
			int nextWall = TheRandom.Next(0, 3);
			while ( (Walls[nextWall] == 0)  
			||		((Row == 0) && (nextWall == 0)) ||
					((Row == Maze.kDimension - 1) && (nextWall == 2)) ||
					((Column == 0) && (nextWall == 1)) ||
					((Column == Maze.kDimension - 1) && (nextWall == 3)) 
				   )
			{
				nextWall = TheRandom.Next(0, 3);
			}

			return nextWall;
		}

		public Point CellCenter()
		{
			Point p1 = new Point(0,0);
			Point p2 = new Point(0,0);
			p1.X = Row*kCellSize + kPadding;
			p1.Y = Column*kCellSize + kPadding;
			p2.X = (Row+1)*kCellSize + kPadding;
			p2.Y = (Column+1)*kCellSize + kPadding;

			Point PMid = new Point( (p1.X + p2.X)/2, (p1.Y + p2.Y)/2);

			return PMid;
		}


		public Rectangle CellBounds()
		{
			Point p1 = new Point(0,0);
			Point p2 = new Point(0,0);
			p1.X = Row*kCellSize + kPadding;
			p1.Y = Column*kCellSize + kPadding;
			p2.X = (Row+1)*kCellSize + kPadding;
			p2.Y = (Column+1)*kCellSize + kPadding;

			Rectangle rtCell = new Rectangle(p1, new Size(Cell.kCellSize, Cell.kCellSize));
			return rtCell;
		}

		public Rectangle GetWallRect(int side)
		{
		  const int kLeeway = 7;
		  Rectangle r = CellBounds();
		  Rectangle rtResult = r;
		  switch(side)
			{
			  case 0: // top
				  rtResult.Y -= kLeeway; // put leeway at top
				  rtResult.Height = kLeeway*2;
				break;
			  case 1: // left
				  rtResult.X -= kLeeway; // put leeway at top
				  rtResult.Width = kLeeway*2;
				  break;
			  case 2: // bottorm
				  rtResult.Y += (r.Height - kLeeway); // put leeway at top
				  rtResult.Height = kLeeway*2;
				  break;
			  case 3: // right
				  rtResult.X += (r.Width - kLeeway); // put leeway at top
				  rtResult.Width = kLeeway*2;
				  break;
			}

			return rtResult;
		}



		bool BottomBlocked
		{
			get
			{
			  return (Walls[2] == 1);
			}
		}

		bool TopBlocked
		{
			get
			{
				return (Walls[0] == 1);
			}
		}

		bool LeftBlocked
		{
			get
			{
				return (Walls[1] == 1);
			}
		}

		bool RightBlocked
		{
			get
			{
				return (Walls[3] == 1);
			}
		}




		public void Draw(Graphics g)
		{
			if (Walls[0] == 1)
			{
				// top wall
				g.DrawLine(Pens.Blue, Row*kCellSize + kPadding, Column*kCellSize + kPadding, (Row+1) * kCellSize   + kPadding, Column*kCellSize +   kPadding);
			}
			if (Walls[1] == 1)
			{
				// left wall
				g.DrawLine(Pens.Blue, Row*kCellSize  + kPadding, Column*kCellSize + kPadding, Row*kCellSize + kPadding, (Column+1)*kCellSize + kPadding);
			}
			if (Walls[2] == 1)
			{
				// bottom wall
				g.DrawLine(Pens.Blue, Row*kCellSize + kPadding, (Column+1)*kCellSize + kPadding, (Row+1)*kCellSize + kPadding, (Column+1)*kCellSize + kPadding);
			}
			if (Walls[3] == 1)
			{
				// right wall
				g.DrawLine(Pens.Blue,(Row+1)*kCellSize + kPadding , Column*kCellSize + kPadding, (Row+1)*kCellSize + kPadding, (Column+1)*kCellSize + kPadding);
			}



		}
	}
}
