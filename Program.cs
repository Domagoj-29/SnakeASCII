using System;
using System.Diagnostics; // također bitno
using System.Runtime.InteropServices; // bitno za isWindowFocused()
using System.Threading;

// "Snake" - napravio Domagoj Radulović

namespace SnakeASCII
{
    // Rezolucija je 40x20 charactera

    internal class Program
    {
		// IntPtr ovisi o arhitekturi računala (32 bita ili 64 bita) daje 4 ili 8 bajtova
		[DllImport("user32.dll")] private static extern IntPtr GetForegroundWindow();
		[DllImport("kernel32.dll")] private static extern IntPtr GetConsoleWindow();
		static bool IsWindowFocused()
		{
			return GetForegroundWindow() == GetConsoleWindow();
		}
		static void Main(string[] args)
        {
			do
			{
				Stopwatch sw = new Stopwatch();
				sw.Start();
				double previousTime = 0;

				Logic gameLogic = new Logic(); // static variable pamte stanja između stvaranja novih objekata
				// renderer - ispisivanje na ekran
				Renderer renderer = new Renderer();
				do
				{
					//bool windowFocus = IsWindowFocused();
					double currentTime = sw.Elapsed.TotalSeconds;
					double deltaTime = currentTime - previousTime;
					previousTime = currentTime;
					if (IsWindowFocused())
					{
						sw.Start(); // ponovno pokrećemo stopwatch ako smo dobili natrag windowFocus
						// igra
						gameLogic.SetDeltaTime(deltaTime); // ovo će isto trebati i rendereru
						gameLogic.ExecuteLogic();
						renderer.Draw(gameLogic.GetAppleCoordinates(), gameLogic.GetSnakeArray(), gameLogic.GetScore());
					}
					else
						sw.Stop(); // ne računamo vrijeme kada nismo u programu
					Thread.Sleep(10);
				}
				while (gameLogic.GetResetBoolean() != true);
			}
			while (true);
        }
    }
}
