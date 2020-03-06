using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SQLite;
namespace Sudoku
{
    abstract class SudokuSave
    {
        public abstract SudokuInstance LoadSudoku(string filePath, string tableName);

        public void LoadNumber(string filePath, string tableName, SudokuInstance instance)
        {
            var con = new SQLiteConnection("data source=" + filePath);
            con.Open();
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = con;
            cmd.CommandText = "SELECT * FROM " + tableName + "Number";
            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                IntVector2 Index = new IntVector2(reader.GetInt32(0), reader.GetInt32(1));
                instance.PrimaryDisk[Index] = reader.GetInt32(3);
                instance.Result[Index] = reader.GetInt32(2);
            }
            con.Close();
        }

        public string SudokuNumberFormat()
        {
            return "(X INT,Y INT,Number INT,PrimaryNumber INT)";
        }

        public abstract string SudokuRulerFormat();
        protected abstract void SaveSudokuRuler(string filePath, string tableName, SudokuInstance instance);

        public void SaveSudoku(string filePath, string tableName, SudokuInstance instance)
        {
            SaveSudokuNumber(filePath, tableName, instance);
            SaveSudokuRuler(filePath, tableName, instance);
        }

        public string GenerateInsertNumberMessage(SudokuInstance instance, IntVector2 Index)
        {
            return $"({Index.X.ToString()},{Index.Y.ToString()},{instance.Result[Index].ToString()},{instance.PrimaryDisk[Index].ToString()})";
        }

        public void CreateNewTable(string filePath, string tableName)
        {
            var con = new SQLiteConnection("data source=" + filePath);
            con.Open();
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = con;
            cmd.CommandText = "CREATE TABLE " + tableName + "Number" + SudokuNumberFormat();
            cmd.ExecuteNonQuery();
            cmd.CommandText = "CREATE TABLE " + tableName + "Ruler" + SudokuRulerFormat();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void DropExistTable(string filePath, string tableName)
        {
            var con = new SQLiteConnection("data source=" + filePath);
            con.Open();
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = con;
            cmd.CommandText = "DROP TABLE " + tableName + "Number";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "DROP TABLE " + tableName + "Ruler";
            cmd.ExecuteNonQuery();
            con.Close();
        }

        private void SaveSudokuNumber(string filePath, string tableName, SudokuInstance instance)
        {
            var con = new SQLiteConnection("data source=" + filePath);
            con.Open();
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = con;
            foreach (IntVector2 Index in instance.PrimaryDisk.Keys)
            {
                cmd.CommandText = "INSERT INTO " + tableName + "Number" + " VALUES" + GenerateInsertNumberMessage(instance, Index);
                cmd.ExecuteNonQuery();
            }
            con.Close();
        }
    }
}
