using System;

namespace SnakeASCII
{
    internal class Logic
    {
		public struct coordinates // Mora biti public za druge dijelove programa
		{
			public int x;
			public int y;
		}
		// Konstante
		const int SCREEN_WIDTH = 40; // 0 -> 39
		const int SCREEN_HEIGHT = 20; // 0 -> 19
		const int GAME_AREA_WIDTH = SCREEN_WIDTH - 2; // 1 -> 38
		const int GAME_AREA_HEIGHT = SCREEN_HEIGHT - 2; // 1 -> 18
		const int DEFAULT_SNAKE_X_COORDINATE = 9; // Ovo MORA biti >= 3 i < SCREEN_WIDTH
		const int DEFAULT_SNAKE_Y_COORDINATE = 9; // Ovo MORA biti >= 1 i < SCREEN_HEIGHT
		const double MOVEMENT_UPDATE_INTERVAL = 0.25; // u sekundama po pomaku
		// Globalne varijable
		double DeltaTime;
		double MovementTimer;
		coordinates AppleCoordinates; // default x = 0, y = 0
		int SnakeLength = 3;
		coordinates[] SnakeArray = new coordinates[GAME_AREA_WIDTH * GAME_AREA_HEIGHT];
		bool DoReset = false;
		bool IsFirstTick = true;
		// Vraća koordinate jabuke
		Random rnd = new Random();
		private coordinates generateApple(coordinates[] snakeArray)
		{
			coordinates appleCoordinates;
			bool matchFound;
			do
			{
				matchFound = false;
				appleCoordinates.x = rnd.Next(1, SCREEN_WIDTH - 1);
				appleCoordinates.y = rnd.Next(1, SCREEN_HEIGHT - 1);
				for (int i = 0; i < SnakeLength; i++)
				{
					if (appleCoordinates.x == snakeArray[i].x && appleCoordinates.y == snakeArray[i].y)
					{
						matchFound = true;
						break;
					}
				}
			}
			while (matchFound);
			return appleCoordinates;
		}
		// Vraća zadnju pritisnutu tipku kao string
		// Ovo je loša implementacija, funkcija radi ispravno samo na prvom pozivu
		private string getKeyPressed()
		{
			string keyPressed = string.Empty;
			while (Console.KeyAvailable)
				keyPressed = Console.ReadKey(intercept: true).Key.ToString();
			return keyPressed;
		}
		private bool isAppleEaten(coordinates appleCoordinates, coordinates snakeHead)
		{
			return appleCoordinates.x == snakeHead.x && appleCoordinates.y == snakeHead.y;
		}
		// Pomoćna funkcija za createSnakeMovement()
		private coordinates[] moveSnake(coordinates[] snakeArray, coordinates velocity)
		{
			coordinates oldTailCoordinates = new coordinates { x = 0, y = 0 };
			for (int i = SnakeLength - 1; i >= 0; i--)
			{
				if (i > 0)
				{
					if (i == SnakeLength - 1)
						oldTailCoordinates = snakeArray[i];
					snakeArray[i].x = snakeArray[i - 1].x;
					snakeArray[i].y = snakeArray[i - 1].y;
				}
				else
				{
					snakeArray[0].x += velocity.x;
					snakeArray[0].y += velocity.y;
					if (isAppleEaten(AppleCoordinates, snakeArray[0]))
					{
						SnakeLength++;
						if (SnakeLength >= GAME_AREA_WIDTH * GAME_AREA_HEIGHT)
							DoReset = true;
						else
						{
							snakeArray[SnakeLength - 1].x = oldTailCoordinates.x;
							snakeArray[SnakeLength - 1].y = oldTailCoordinates.y;
							AppleCoordinates = generateApple(snakeArray);
						}
					}
				}
			}
			return snakeArray;
		}
		// Ova funkcija koristi lambda izraz da mogu inicijalizirati stvari
		// Ovdje se računa smjer kretanja i veličina zmije, tj. uređuje niz koordinata koje zauzima
		private Func <coordinates[]> createSnakeMovement()
		{
			coordinates velocity = new coordinates {x = 1, y = 0};
			coordinates oldVelocity = velocity;
			for (int i = 0; i < SnakeLength; i++)
			{
				SnakeArray[i].x = DEFAULT_SNAKE_X_COORDINATE - i;
				SnakeArray[i].y = DEFAULT_SNAKE_Y_COORDINATE;
			}
			AppleCoordinates = generateApple(SnakeArray);
			bool directionChanged = false; // sprječavanje promjene smjera do izvršavanja moveSnake()
			return () =>
			{
				string keyPressed = getKeyPressed();
				if (keyPressed == "Backspace")
					Environment.Exit(0);
				if (!directionChanged)
				{
					if (keyPressed == "UpArrow" && oldVelocity.y != 1)
					{
						velocity.x = 0;
						velocity.y = -1;
						directionChanged = true;
					}
					else if (keyPressed == "DownArrow" && oldVelocity.y != -1)
					{
						velocity.x = 0;
						velocity.y = 1;
						directionChanged = true;
					}
					else if (keyPressed == "LeftArrow" && oldVelocity.x != 1)
					{
						velocity.x = -1;
						velocity.y = 0;
						directionChanged = true;
					}
					else if (keyPressed == "RightArrow" && oldVelocity.x != -1)
					{
						velocity.x = 1;
						velocity.y = 0;
						directionChanged = true;
					}
				}
				MovementTimer += DeltaTime;
				if (MovementTimer >= MOVEMENT_UPDATE_INTERVAL)
				{
					MovementTimer -= MOVEMENT_UPDATE_INTERVAL;
					SnakeArray = moveSnake(SnakeArray, velocity);
					oldVelocity = velocity;
					directionChanged = false;
				}
				return SnakeArray;
			};
		}
		private Func<coordinates[]> snakeMovement;
		// Provjeravamo je li glava zmije izvan određenog prostora, je li se sudarila sa samim sobom
		private void snakeFailConditions(coordinates[] snakeArray)
		{
			coordinates snakeHead = snakeArray[0];
			if (snakeHead.x < 1 || snakeHead.x >= SCREEN_WIDTH - 1)
				DoReset = true;
			else if (snakeHead.y < 1 || snakeHead.y >= SCREEN_HEIGHT - 1)
				DoReset = true;
			for (int i = 1; i < SnakeLength; i++)
			{
				if (snakeHead.x == snakeArray[i].x && snakeHead.y == snakeArray[i].y)
					DoReset = true;
			}
		}
		public void ExecuteLogic()
		{
			if (IsFirstTick)
			{
				IsFirstTick = false;
				snakeMovement = createSnakeMovement();
			}
			snakeMovement();
			snakeFailConditions(SnakeArray);
		}
		// Dohvaćujemo deltu vremena iz glavnog programa
		public void SetDeltaTime(double deltaTime)
		{
			DeltaTime = deltaTime;
		}
		public bool GetResetBoolean()
		{
			return DoReset;
		}
		public coordinates GetAppleCoordinates()
		{
			return AppleCoordinates;
		}
		public coordinates[] GetSnakeArray()
		{
			return SnakeArray; // Ovo je IZVORNI niz, može ga promijeniti bilo tko
		}
		public int GetScore()
		{
			return SnakeLength - 3;
		}
	}
}
