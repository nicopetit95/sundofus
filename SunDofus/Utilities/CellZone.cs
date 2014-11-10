using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SunDofus.World.Maps;

namespace SunDofus.Utilities
{
    class CellZone
    {
        public static List<int> GetAdjacentCells(Map Map, int Cell)
        {
            var Cells = new List<int>();
            for (int i = 1; i < 8; i += 2)
                Cells.Add(Pathfinding.NextCell(Map, Cell, i));

            return Cells;
        }

        public static List<int> GetLineCells(Map Map, int Cell, int Direction, int Length)
        {
            var Cells = new List<int>();
            var LastCell = Cell;
            for (int i = 0; i < Length; i++)
            {
                Cells.Add(Pathfinding.NextCell(Map, LastCell, Direction));
                LastCell = Cells[i];
            }

            return Cells;
        }

        public static List<int> GetCircleCells(Map Map, int CurrentCell, int Radius)
        {
            var Cells = new List<int>() { CurrentCell };
            for (int i = 0; i < Radius; i++)
            {
                var Copy = Cells.ToArray();
                foreach (var Cell in Copy)
                    Cells.AddRange(from Item in GetAdjacentCells(Map, Cell) where !Cells.Contains(Item) select Item);
            }

            return Cells;
        }

        public static List<int> GetCrossCells(Map Map, int CurrentCell, int Radius)
        {
            var Cells = new List<int>();
            foreach (var Cell in GetCircleCells(Map, CurrentCell, Radius))
                if (Pathfinding.InLine(Map, CurrentCell, Cell))
                    Cells.Add(Cell);

            return Cells;
        }

        public static List<int> GetTLineCells(Map Map, int Cell, int Direction, int Length)
        {
            var LineDirection = Direction <= 5 ? Direction + 2 : Direction - 6;
            var Cells = new List<int>();

            Cells.AddRange(GetLineCells(Map, Cell, LineDirection, Length));
            Cells.AddRange(GetLineCells(Map, Cell, Pathfinding.OppositeDirection(LineDirection), Length));

            return Cells;
        }

        public static List<int> GetCells(Map Map, int Cell, int CurrentCell, string Range)
        {
            switch (Range[0])
            {
                case 'C':
                    return GetCircleCells(Map, Cell, Basic.HASH.IndexOf(Range[1]));

                case 'X':
                    return GetCrossCells(Map, Cell, Basic.HASH.IndexOf(Range[1]));

                case 'T':
                    var Cells1 = new List<int> { Cell };
                    Cells1.AddRange(GetTLineCells(Map, Cell, Pathfinding.GetDirection(Map, CurrentCell, Cell), Basic.HASH.IndexOf(Range[1])));
                    return Cells1;

                case 'L':
                    var Cells2 = new List<int> { Cell };
                    Cells2.AddRange(GetLineCells(Map, Cell, Pathfinding.GetDirection(Map, CurrentCell, Cell), Basic.HASH.IndexOf(Range[1])));
                    return Cells2;
            }

            return new List<int>() { Cell };
        }
    }
}