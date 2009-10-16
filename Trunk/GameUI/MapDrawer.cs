﻿using System.Collections.Generic;
using GameEngine.Interfaces;
using libtcodWrapper;
using Utilities;

namespace GameUI
{
    public static class MapDrawer
    {
        public const int MapDrawnWidth = 51;
        public const int MapDrawnHeight = 43;
        
        public static Point ScreenCenter = new Point((MapDrawnWidth - 1) / 2, (MapDrawnHeight - 2) / 2);

        public static void DrawMap(IPlayer player, IMap map, Console screen)
        {
            Point mapUpCorner = CalculateMapCorner(player);

            for (int i = 0; i < map.Width; ++i)
            {
                for (int j = 0; j < map.Height; ++j)
                {
                    Point screenPlacement = new Point(mapUpCorner.X + i, mapUpCorner.Y + j);

                    if (IsDrawableTile(screenPlacement))
                    {
                        screen.PutChar(screenPlacement.X, screenPlacement.Y, ConvertTerrianToChar(map[i, j].Terrain));
                    }
                }
            }

            screen.PutChar(ScreenCenter.X, ScreenCenter.Y, '@');
        }

        private static char ConvertTerrianToChar(TerrainType t)
        {
            switch (t)
            {
                case TerrainType.Floor:
                    return ' ';
                case TerrainType.Wall:
                    return '#';
                default:
                    return ' ';
            }
        }
        
        private static Point CalculateMapCorner(IPlayer player)
        {
            return new Point(ScreenCenter.X - player.Position.X, ScreenCenter.Y - player.Position.Y);
        }

        private static bool IsDrawableTile(Point p)
        {
            bool xOk = p.X >= 0 && p.X < MapDrawnWidth;
            bool yOk = p.Y >= 0 && p.Y < MapDrawnHeight;
            return xOk && yOk;
        }
    }
}