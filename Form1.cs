using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;
using System.Runtime.InteropServices;
using DFSAlgorithmMaze;


namespace WindowsApplication22
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{

		[DllImport("winmm.dll")]
		public static extern long PlaySound(String lpszName, long hModule, long dwFlags);

		private ArrayList Stones = new ArrayList(30);
		private Eater TheEater = new Eater(100, 100);
		private Random RandomGen = new Random();
		private const int NumberOfStones = 25;
		private Score TheScore  = new Score(400, 290);
		private int TheSeconds = 90;
		private TimerDisplay TheTime = new TimerDisplay(20, 290);
		private System.Windows.Forms.Timer timer1;
		private System.ComponentModel.IContainer components;
		private Thread oThread = null;
		private Maze TheMaze  = new Maze();
		private bool m_bGameDone = false;
		private GameMessage TheStatusMessage = new GameMessage(150, 10);
		private GameMessage TheStoneMessage = new GameMessage(430, 10);
		public enum Side  {top = 0, left = 1, bottom = 2, right = 3};

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			Maze.kDimension = 20;
			Cell.kCellSize = 30;
			TheMaze.Initialize();
			TheMaze.Generate();

			InitializeStones();
			InitializeEater();
			InitializeMessage();
            InitializeTimer();
			InitializeScore();


			SetBounds(10, 10, (Maze.kDimension + 1) * Cell.kCellSize + Cell.kPadding, (Maze.kDimension + 3) * Cell.kCellSize + Cell.kPadding);
			// reduce flicker

			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		private string m_strCurrentSoundFile = "miss.wav";
		public void PlayASound()
		{
			if (m_strCurrentSoundFile.Length > 0)
			{
				PlaySound(Application.StartupPath + "\\" + m_strCurrentSoundFile, 0, 0);
			}
			m_strCurrentSoundFile = "";
			oThread.Abort();
		}

		public void PlaySoundInThread(string wavefile)
		{
			m_strCurrentSoundFile = wavefile;
			oThread = new Thread(new ThreadStart(PlayASound));
			oThread.Start();
		}


		public void InitializeTimer()
		{
			timer1.Start();
			TheTime.Direction = TimeDirection.Down;
			TheTime.Position.Y = (Maze.kDimension) * Cell.kCellSize + Cell.kPadding;
		}

		public void InitializeMessage()
		{
			TheStatusMessage.Position.Y = (Maze.kDimension) * Cell.kCellSize + Cell.kPadding;
			TheStoneMessage.Position.Y = (Maze.kDimension) * Cell.kCellSize + Cell.kPadding;
			TheStoneMessage.Message = "Stones";
		}


		public void InitializeScore()
		{
			TheScore.Reset();
			TheScore.Position.Y = (Maze.kDimension) * Cell.kCellSize + Cell.kPadding;
		}


		public void InitializeStones()
		{
			for (int i = 0; i < NumberOfStones; i++)
			{
				Point cellCenter = GetRandomCellPosition();
				Stones.Add(new Stone(cellCenter.X - 6, cellCenter.Y - 6)); // 12 is stone image width and height, 6 is half of this
			}
		} 

		public Point GetRandomCellPosition()
		{
			int xCell = RandomGen.Next(0, Maze.kDimension);
			int yCell = RandomGen.Next(0, Maze.kDimension);
			Point cellCenter = TheMaze.GetCellCenter(xCell, yCell);
			return cellCenter;
		}

		public void InitializeEater()
		{
			Point cellCenter = GetRandomCellPosition();
			TheEater.Position.X = cellCenter.X - 10;
			TheEater.Position.Y = cellCenter.Y - 10;
		} 



		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			// 
			// timer1
			// 
			this.timer1.Interval = 20;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(592, 573);
			this.KeyPreview = true;
			this.Name = "Form1";
			this.Text = "Eater Game";
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}


		private void Form1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			g.FillRectangle(Brushes.White, 0, 0, this.ClientRectangle.Width, ClientRectangle.Height);

			TheMaze.Draw(g);

			// draw the score

			TheScore.Draw(g);		
	
			// draw the time

			TheTime.Draw(g, TheSeconds);

			// Draw a message indicating the status of the game
			TheStatusMessage.Draw(g);

			// Draw a message indicating a Stone
			if (TheScore.Value == 1)
				TheStoneMessage.Message = "Stone";
			else
				TheStoneMessage.Message = "Stones";

			TheStoneMessage.Draw(g);


			// draw the stones

			for (int i = 0; i < Stones.Count; i++)
			{
				((Stone)Stones[i]).Draw(g);
			}

			// also draw the eater
			TheEater.Draw(g);

		}

		private int CheckIntersection()
		{
			for (int i = 0; i < Stones.Count; i++)
			{
				Rectangle stoneRect = ((Stone)Stones[i]).GetFrame();
				if (TheEater.GetFrame().IntersectsWith(stoneRect))
				{
					return i;
				}
			}

			return -1;
		}

		private bool CanEaterMove(Side aSide)
		{
		  int theSide = (int)aSide;
		  Cell EaterCell = TheMaze.GetCellFromPoint(TheEater.Position.X + 10, TheEater.Position.Y + 10);
			if (EaterCell.Walls[theSide] == 1)
			{
				if (EaterCell.GetWallRect((int)aSide).IntersectsWith(TheEater.GetFrame()))
				{				
					return false;  // blocked
				}


			}

			return true;  // not blocked
		}

		string LatestKey = "none";

		private void HandleLatestKey()
		{
			if (m_bGameDone)
				return;  // precondition

//			string result = e.KeyData.ToString();
			string result = LatestKey;
			Invalidate(TheEater.GetFrame());
			switch (result)
			{
				case "Left":
					if (CanEaterMove(Side.left))
					{
						TheEater.MoveLeft(ClientRectangle);
					}
					Invalidate(TheEater.GetFrame());
					break;
				case "Right":
					if (CanEaterMove(Side.right))
					{
						TheEater.MoveRight(ClientRectangle);
					}
					Invalidate(TheEater.GetFrame());
					break;
				case "Up":
					if (CanEaterMove(Side.top))
					{
						TheEater.MoveUp(ClientRectangle);
					}
					Invalidate(TheEater.GetFrame());
					break;
				case "Down":
					if (CanEaterMove(Side.bottom))
					{
						TheEater.MoveDown(ClientRectangle);
					}
					Invalidate(TheEater.GetFrame());
					break;
				default:
					break;

			}

			int hit = CheckIntersection();
			if (hit != -1)
			{
				TheScore.Increment();

				PlaySoundInThread("hit.wav");
				Invalidate(TheScore.GetFrame());
				Invalidate(((Stone)Stones[hit]).GetFrame()); 
				Stones.RemoveAt(hit);
				if (Stones.Count == 0)
				{
					MessageBox.Show("You Win!\nYour time is " + TheTime.TheString + " seconds.");
					Application.Exit();
				}
			}

		}

		private void Form1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			string result = e.KeyData.ToString();
			LatestKey = result;
		}

		static long TimerTickCount = 0;
		private void timer1_Tick(object sender, System.EventArgs e)
		{
			TimerTickCount++;

			if (TimerTickCount % 2 == 0) // do the key handling here
			{
				HandleLatestKey();
			}
			
			if (TimerTickCount % 50 == 0) // every 50 is one second
			{
				if (TheTime.Direction == TimeDirection.Up)
					TheSeconds++;
				else
					TheSeconds--;

				Invalidate(TheTime.GetFrame());

				if (TheSeconds == 0)
				{
					TheStatusMessage.Message = "Game Over";
					m_bGameDone = true;
					timer1.Stop();
					Invalidate(TheStatusMessage.GetFrame());
				}
			}

		}
	}
}
