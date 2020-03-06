using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SQLite;
namespace Sudoku
{
    class FerrisWheelSudokuSave : SudokuSave
    {
        public override SudokuInstance LoadSudoku(string filePath, string tableName)
        {
            FerrisWheelSudoku result = new FerrisWheelSudoku();
            LoadNumber(filePath, tableName, result);
            LoadRuler(filePath, tableName, result);
            return result;
        }

        private void LoadRuler(string filePath, string tableName, FerrisWheelSudoku instance)
        {
            var con = new SQLiteConnection("data source=" + filePath);
            con.Open();
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = con;
            cmd.CommandText = "SELECT * FROM " + tableName + "Ruler";
            SQLiteDataReader reader = cmd.ExecuteReader();
            instance.ferrisWheels.Clear();
            while (reader.Read())
            {
                int j = reader.GetInt32(1);
                int column = reader.GetInt32(0);
                int ferris = reader.GetInt32(2);
                List<SolutionNumber> scope = new List<SolutionNumber>();
                for (int i = 1; i < 10; i++)
                    if (column == 1) scope.Add(instance.solutionNumbers[new IntVector2(i, j)]); else scope.Add(instance.solutionNumbers[new IntVector2(j, i)]);
                instance.ferrisWheels.Add(new FerrisWheelRuler(scope, ferris));
            }
        }

        public string GenerateInsertRulerMessage(FerrisWheelSudoku instance, FerrisWheelRuler ruler)
        {
            IntVector2 FirstIndex = instance.IndexSet[ruler.Scope[0]];
            IntVector2 SecondIndex = instance.IndexSet[ruler.Scope[1]];
            if (FirstIndex.X == SecondIndex.X)
                return $"(0,{FirstIndex.X.ToString()},{ruler.FerrisWheelNumber.ToString()})";
            else
                return $"(1,{FirstIndex.Y.ToString()},{ruler.FerrisWheelNumber.ToString()})";
        }

        public override string SudokuRulerFormat()
        {
            return "(Column INT,LineIndex INT,Number INT)";
        }

        protected override void SaveSudokuRuler(string filePath, string tableName, SudokuInstance instance)
        {
            if (instance is FerrisWheelSudoku)
            {
                FerrisWheelSudoku FerrisInstance = instance as FerrisWheelSudoku;
                var con = new SQLiteConnection("data source=" + filePath);
                con.Open();
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.Connection = con;
                foreach (FerrisWheelRuler ruler in FerrisInstance.ferrisWheels)
                {
                    cmd.CommandText = "INSERT INTO " + tableName + "Ruler" + " VALUES" + GenerateInsertRulerMessage(FerrisInstance, ruler);
                    cmd.ExecuteNonQuery();
                }
                con.Close();
            }
        }
    }
}
