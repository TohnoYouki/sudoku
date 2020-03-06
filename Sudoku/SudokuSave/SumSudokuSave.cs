using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SQLite;

namespace Sudoku
{
    class SumSudokuSave : SudokuSave
    {
        public override SudokuInstance LoadSudoku(string filePath, string tableName)
        {
            SumSudoku result = new SumSudoku();
            LoadNumber(filePath, tableName, result);
            LoadRuler(filePath, tableName, result);
            return result;
        }

        private void LoadRuler(string filePath, string tableName, SumSudoku instance)
        {
            var con = new SQLiteConnection("data source=" + filePath);
            con.Open();
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = con;
            cmd.CommandText = "SELECT * FROM " + tableName + "Ruler";
            SQLiteDataReader reader = cmd.ExecuteReader();
            instance.sumRulerList.Clear();
            while (reader.Read())
            {
                int sumIndex = reader.GetInt32(0);
                int sum = reader.GetInt32(1);
                IntVector2 Index = new IntVector2(reader.GetInt32(2), reader.GetInt32(3));
                if (sumIndex > instance.sumRulerList.Count)
                    instance.sumRulerList.Add(new SumRuler(new List<SolutionNumber>(), sum));
                instance.sumRulerList[sumIndex - 1].Scope.Add(instance.solutionNumbers[Index]);
            }
        }

        public string GenerateInsertRulerMessage(SudokuInstance instance, int index, int sum, SolutionNumber number)
        {
            IntVector2 NumberIndex = instance.IndexSet[number];
            return $"({index.ToString()},{sum.ToString()},{NumberIndex.X.ToString()},{NumberIndex.Y.ToString()})";
        }

        public override string SudokuRulerFormat()
        {
            return "(SumIndex INT,Sum INT,X INT,Y INT)";
        }

        protected override void SaveSudokuRuler(string filePath, string tableName, SudokuInstance instance)
        {
            if (instance is SumSudoku)
            {
                SumSudoku sumInstance = instance as SumSudoku;
                var con = new SQLiteConnection("data source=" + filePath);
                con.Open();
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.Connection = con;
                int index = 0;
                foreach (SumRuler ruler in sumInstance.sumRulerList)
                {
                    index++;
                    foreach (SolutionNumber number in ruler.Scope)
                    {
                        cmd.CommandText = "INSERT INTO " + tableName + "Ruler" + " VALUES" + GenerateInsertRulerMessage(sumInstance, index, ruler.Sum, number);
                        cmd.ExecuteNonQuery();
                    }
                }
                con.Close();
            }
        }
    }
}
