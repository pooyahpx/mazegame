using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace WindowsApplication22
{
	/// <summary>
	/// The GameMessage of the game
	/// </summary>
	public class GameMessage
	{
		public Point Position = new Point(0,0);
		public Font MyFont = new Font("Compact", 20.0f, GraphicsUnit.Pixel );

		public GameMessage(int x, int y)
		{
			// 
			// TODO: Add constructor logic here
			//
			Position.X = x;
			Position.Y = y;
		}

		public string Message = "";

		public void Draw(Graphics g)
		{
			g.DrawString(Message, MyFont, Brushes.RoyalBlue, Position.X, Position.Y, new StringFormat());
		}

		public Rectangle GetFrame()
		{
			Rectangle myRect = new Rectangle(Position.X, Position.Y, (int)MyFont.SizeInPoints*Message.Length, MyFont.Height);
			return myRect;
		}
	}
}
