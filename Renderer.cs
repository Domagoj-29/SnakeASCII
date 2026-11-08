using System;

namespace SnakeASCII
{
    internal class Renderer
    {
		// Konstante
		const int SCREEN_WIDTH = 40; // 0 -> 39
		const int SCREEN_HEIGHT = 20; // 0 -> 19
		//const int GAME_AREA_WIDTH = SCREEN_WIDTH - 2; // 1 -> 38
		//const int GAME_AREA_HEIGHT = SCREEN_HEIGHT - 2; // 1 -> 18
		const char APPLE_CHAR = 'A';
		const char SNAKE_HEAD_CHAR = 'O';
		const char SNAKE_BODY_CHAR = '0';

		public void Draw(Logic.coordinates AppleCoordinates, Logic.coordinates[] SnakeArray, int Score)
		{
			Console.CursorVisible = false;
			// popunimo kartu simbola razmacima
			char[,] characterMap = new char[SCREEN_WIDTH, SCREEN_HEIGHT];
			for (int i = 0; i < characterMap.GetLength(0); i++)
			{
				for (int j = 0; j < characterMap.GetLength(1); j++)
				{
					if (i == 0 || i == SCREEN_WIDTH - 1 || j == 0 || j == SCREEN_HEIGHT - 1) // ako smo uz rub ekrana
						characterMap[i, j] = '#';
					else
						characterMap[i, j] = ' ';
				}
			}
			// unesemo jabuku i zmiju u kartu simbola
			characterMap[AppleCoordinates.x, AppleCoordinates.y] = APPLE_CHAR;
			for (int i = 0; i < Score + 3; i++)
			{
				if (i == 0)
					characterMap[SnakeArray[0].x, SnakeArray[0].y] = SNAKE_HEAD_CHAR;
				else
					characterMap[SnakeArray[i].x, SnakeArray[i].y] = SNAKE_BODY_CHAR;
			}

			Console.SetCursorPosition(0, 0);
			for (int i = 0; i < SCREEN_HEIGHT; i++)
			{
				for (int j = 0; j < SCREEN_WIDTH; j++)
					Console.Write(characterMap[j, i]);
				Console.WriteLine(); // prelazak u novi red
			}
		}
	}
}
