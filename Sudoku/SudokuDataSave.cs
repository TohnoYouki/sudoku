using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SQLite;
using System.Threading;

namespace Sudoku
{
    class SudokuDataSave
    {
        private List<bool> compareSudokuRead;
        private List<bool> sumSudokuRead;
        private List<bool> ferrisWheelRead;

        private CompareSudokuSave compareSudokuSave = new CompareSudokuSave();
        private SumSudokuSave sumSudokuSave = new SumSudokuSave();
        private FerrisWheelSudokuSave FerrisWheelSudokuSave = new FerrisWheelSudokuSave();

        private Thread UpdateThread;
        private int tableNumber = 40;
        public bool update;
        private string filePath = System.Environment.CurrentDirectory + "\\SudokuDataBase.db";

        private void LoadBoolRead()
        {
            compareSudokuRead = new List<bool>();
            sumSudokuRead = new List<bool>();
            ferrisWheelRead = new List<bool>();
            var con = new SQLiteConnection("data source=" + filePath);
            con.Open();
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = con;
            cmd.CommandText = "SELECT * FROM ReadValue";
            SQLiteDataReader reader = cmd.ExecuteReader();
            for (int i = 0; i < tableNumber; i++){ compareSudokuRead.Add(true); sumSudokuRead.Add(true); ferrisWheelRead.Add(true); }
            while (reader.Read())
            {
                int index = reader.GetInt32(0);
                if (reader.GetInt32(1) == 0) compareSudokuRead[index] = false; else compareSudokuRead[index] = true;
                if (reader.GetInt32(2) == 0) sumSudokuRead[index] = false; else sumSudokuRead[index] = true;
                if (reader.GetInt32(3) == 0) ferrisWheelRead[index] = false; else ferrisWheelRead[index] = true;
            }
            con.Close();
        }

        private void SaveBoolRead()
        {
            var con = new SQLiteConnection("data source=" + filePath);
            con.Open();
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = con;
            cmd.CommandText = "DROP TABLE ReadValue";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "CREATE TABLE ReadValue(ReadIndex INT,Compare INT,Sum INT,Ferris INT)";
            cmd.ExecuteNonQuery();
            for (int i = 0; i < tableNumber; i++)
            {
                int compare, sum, ferris;
                if (compareSudokuRead[i]) compare = 1; else compare = 0;
                if (sumSudokuRead[i]) sum = 1; else sum = 0;
                if (ferrisWheelRead[i]) ferris = 1; else ferris = 0;
                cmd.CommandText = "INSERT INTO ReadValue" + " VALUES" + $"({i.ToString()},{compare.ToString()},{sum.ToString()},{ferris.ToString()})";
                cmd.ExecuteNonQuery();
            }
            con.Close();
        }

        public SudokuDataSave()
        {
            update = true;
            LoadBoolRead();
            UpdateThread = new Thread(new ThreadStart(Update));
            UpdateThread.Start();
        }

        public SudokuInstance GetSingleCompareSudoku()
        {
            int result = -1;
            ContinusRandomSelect x = new ContinusRandomSelect();
            for (int i = 0; i < tableNumber; i++)
                if (compareSudokuRead[i])
                    if (x.SelectThis()) result = i;
            if (result != -1)
            {
                SudokuInstance returnValue = compareSudokuSave.LoadSudoku(filePath, "CompareSudoku" + result.ToString());
                compareSudokuRead[result] = false;
                return returnValue;
            }
            else
            {
                CompareSudoku sudoku = new CompareSudoku();
                sudoku.GenerateSudoku();
                return sudoku;
            }
        }

        public SudokuInstance GetSingleSumSudoku()
        {
            int result = -1;
            ContinusRandomSelect x = new ContinusRandomSelect();
            for (int i = 0; i < tableNumber; i++)
                if (sumSudokuRead[i])
                    if (x.SelectThis()) result = i;
            if (result != -1)
            {
                SudokuInstance returnValue = sumSudokuSave.LoadSudoku(filePath, "SumSudoku" + result.ToString());
                sumSudokuRead[result] = false;
                return returnValue;
            }
            else
            {
                SumSudoku sudoku = new SumSudoku();
                sudoku.GenerateSudoku();
                return sudoku;
            }
        }

        public SudokuInstance GetSingleFerrisWheelSudoku()
        {
            int result = -1;
            ContinusRandomSelect x = new ContinusRandomSelect();
            for (int i = 0; i < tableNumber; i++)
                if (ferrisWheelRead[i])
                    if (x.SelectThis()) result = i;
            if (result != -1)
            {
                SudokuInstance returnValue = FerrisWheelSudokuSave.LoadSudoku(filePath, "FerrisWheelSudoku" + result.ToString());
                ferrisWheelRead[result] = false;
                return returnValue;
            }
            else
            {
                FerrisWheelSudoku sudoku = new FerrisWheelSudoku();
                sudoku.GenerateSudoku();
                return sudoku;
            }
        }

        private void GenerateCompareSudoku()
        {
            for (int i = 0; i < tableNumber; i++)
                if (!compareSudokuRead[i])
                {
                    CompareSudoku sudoku = new CompareSudoku();
                    if (sudoku.GenerateSudoku())
                        FerrisWheelSudokuSave.SaveSudoku(filePath, "CompareSudoku" + i.ToString(), sudoku);
                    compareSudokuRead[i] = true;
                    break;
                }
        }

        private void GenerateSumSudoku()
        {
            for (int i = 0; i < tableNumber; i++)
                if (!sumSudokuRead[i])
                {
                    SumSudoku sudoku = new SumSudoku();
                    if (sudoku.GenerateSudoku())
                        sumSudokuSave.SaveSudoku(filePath, "SumSudoku" + i.ToString(), sudoku);
                    sumSudokuRead[i] = true;
                    break;
                }
        }

        private void GenerateFerrisWheelSudoku()
        {
            for (int i = 0; i < tableNumber; i++)
                if (!ferrisWheelRead[i])
                {
                    FerrisWheelSudoku sudoku = new FerrisWheelSudoku();
                    if (sudoku.GenerateSudoku())
                        FerrisWheelSudokuSave.SaveSudoku(filePath, "FerrisWheelSudoku" + i.ToString(), sudoku);
                    ferrisWheelRead[i] = true;
                    break;
                }
        }
        private bool over;

        public void Close()
        {
            update = false;
            SudokuInstance.Close();
        }

        public void Update()
        {
            over = false;
            int index = new Random().Next(3);
            while (update)
            {
                if (index == 0) GenerateCompareSudoku();
                if (index == 1) GenerateSumSudoku();
                if (index == 2) GenerateFerrisWheelSudoku();
                index = (index + 1) % 3;
            }
            over = true;
        }

        ~SudokuDataSave()
        {
            update = false;
            while (!over) ;
            SaveBoolRead();
        }
    }
}
