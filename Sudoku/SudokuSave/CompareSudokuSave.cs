using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SQLite;

namespace Sudoku
{
    class CompareSudokuSave : SudokuSave
    {
        public override SudokuInstance LoadSudoku(string filePath, string tableName)
        {
            CompareSudoku result = new CompareSudoku();
            LoadNumber(filePath, tableName, result);
            LoadRuler(filePath, tableName, result);
            return result;
        }

        private void LoadRuler(string filePath, string tableName, CompareSudoku instance)
        {
            var con = new SQLiteConnection("data source=" + filePath);
            con.Open();
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = con;
            cmd.CommandText = "SELECT * FROM " + tableName + "Ruler";
            SQLiteDataReader reader = cmd.ExecuteReader();
            instance.compareRulers.Clear();
            while (reader.Read())
            {
                IntVector2 Index1 = new IntVector2(reader.GetInt32(0), reader.GetInt32(1));
                IntVector2 Index2 = new IntVector2(reader.GetInt32(2), reader.GetInt32(3));
                instance.compareRulers.Add(new SingleCompareRuler(instance.solutionNumbers[Index1], instance.solutionNumbers[Index2]));
            }
        }

        public string GenerateInsertRulerMessage(SudokuInstance instance, Ruler ruler)
        {
            SingleCompareRuler compareRuler = ruler as SingleCompareRuler;
            IntVector2 larger = instance.IndexSet[compareRuler.larger];
            IntVector2 smaller = instance.IndexSet[compareRuler.less];
            return $"({larger.X.ToString()},{larger.Y.ToString()},{smaller.X.ToString()},{smaller.Y.ToString()})";
        }

        public override string SudokuRulerFormat()
        {
            return "(LargerX INT,Larger Y INT,SmallerX INT,SmallerY INT)";
        }

        protected override void SaveSudokuRuler(string filePath, string tableName, SudokuInstance instance)
        {
            if (instance is CompareSudoku)
            {
                CompareSudoku compareInstance = instance as CompareSudoku;
                var con = new SQLiteConnection("data source=" + filePath);
                con.Open();
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.Connection = con;
                foreach (SingleCompareRuler ruler in compareInstance.compareRulers)
                {
                    cmd.CommandText = "INSERT INTO " + tableName + "Ruler" + " VALUES" + GenerateInsertRulerMessage(compareInstance, ruler);
                    cmd.ExecuteNonQuery();
                }
                con.Close();
            }
        }
    }
}
