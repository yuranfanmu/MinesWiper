using System;
using System.Drawing;
using System.Windows.Forms;

namespace MinesWiper
{
    public partial class Form1 : Form
    {
        Button[,] buttons;
        int[,] matrix; // матрица мин и мин вокруг (-1 - мина)
        int[,] matrixExp; // расширенная матрица мин (для упрощения подсчета мин вокруг выбранной ячейки)
        int buttonSize = 25; // размер кнопки
        int foundedMinesCounter = 0; // число отмеченных мин

        public Form1()
        {
            InitializeComponent();
        }

        private void generateMatrix(int width, int heigth, int mines)
        {
            /* Метод генерирует матрицу мин:
             * -1 - есть мина
             * 0 нет мины и нет мин вокруг клетки
             * 1-8 - нет мины, указано количество мин вокруг клетки
             */
            foundedMinesCounter = 0;
            matrix = new int[width, heigth];
            matrixExp = new int[width + 2, heigth + 2];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < heigth; j++)
                {
                    matrix[i, j] = 0;
                    matrixExp[i + 1, j + 1] = 0;
                }
            }
            
            // Расставляем мины, пока не будет расставлено требуемое количество
            Random r = new Random();
            int counter = 0;
            int random;
            while (counter < mines)
            {
                random = r.Next(width * heigth);
                if (matrix[random / heigth, random % heigth] != -1)
                {
                    matrix[random / heigth, random % heigth] = -1;
                    matrixExp[random / heigth + 1, random % heigth + 1] = -1;
                    counter++;
                }
            }
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < heigth; j++)
                    getMinesAround(i, j);
            }
        }

        private void getMinesAround(int i, int j)
        {
            // Метод подсчитывает число мин вокруг ячейки и устанавливает полученное значение в ячейку
            if (matrix[i, j] == -1)
                return;
            int counter = 0;
            for (int a = i; a < i + 3; a++)
                for (int b = j; b < j + 3; b++)
                {
                    if (matrixExp[a, b] == -1)
                        counter++;
                }
            matrix[i, j] = counter;
        }


        private void setFieldSize(int difficultMode)
        {
            if (buttons != null)
            for (int i = 0; i < buttons.GetLength(0); i++)
            {
                for (int j = 0; j < buttons.GetLength(1); j++)
                {
                    this.Controls.Remove(buttons[i, j]);
                }
            }
            buttons = null;
            switch (difficultMode)
            {
                case 1:
                    buttons = new Button[10, 10];
                    break;
                case 2:
                    buttons = new Button[16, 16];
                    break;
                case 3:
                    buttons = new Button[30, 16];
                    break;
                default:
                    Console.WriteLine("Error difficult mode");
                    return;
            }
            for (int i = 0; i < buttons.GetLength(0); i++)
            {
                for (int j = 0; j < buttons.GetLength(1); j++)
                {
                    buttons[i, j] = new Button();
                    buttons[i, j].Height = buttonSize;
                    buttons[i, j].Width = buttonSize;
                    buttons[i, j].Location = new Point(i * buttonSize, 30 + j * buttonSize);
                    buttons[i, j].BackColor = Color.LightBlue;
                    buttons[i, j].TabStop = false;
                    buttons[i, j].MouseDown += BtnClickEvent;
                    this.Controls.Add(buttons[i, j]);
                }
            }
            this.Width = 10;
            this.Height = 10;
            this.AutoSize = true;
        }

        private void BtnClickEvent(object sender, MouseEventArgs e)
        {
            Button b = (Button)sender;
            int i = b.Location.X / buttonSize;
            int j = (b.Location.Y - 30) / buttonSize;

            // Если нажата правая кнопка мыши, клетка отмечается как претендент на мину
            if (e.Button == MouseButtons.Right)
            {
                b.BackColor = Color.LightGreen;
                b.Enabled = false;
                foundedMinesCounter++;
            }

            // Если нажата левая кнопка мыши. клетка открывается
            if (e.Button == MouseButtons.Left)
            {
                b.BackColor = Color.White;
                b.Enabled = false;

                if (matrix[i, j] == 0)
                {
                    matrix[i, j] = -2;
                    b.Text = "";
                    openNearbyCells(i, j);
                    return;
                }
                if (matrix[i, j] == -1)
                {
                    gameOver();
                    return;
                }
                b.Text = matrix[i, j].ToString();
            }
        }

        private void openNearbyCells(int i, int j)
        {
            // Метод открывает соседние клетки, если вокруг выбранной клетки 0 мин (реализована рекурсия)
            for (int a = i - 1; a < i +2; a++)
                for (int b = j - 1; b < j + 2; b++)
                {
                    // Для крайних клеток в поле реализована обработка исключения выхода за границы массива
                    // Переработать метод с использованием расширенной матрицы
                    try
                    {
                        if (matrix[a, b] != -2)
                            buttons[a, b].Text = matrix[a, b].ToString();
                        buttons[a, b].Enabled = false;
                        buttons[a, b].BackColor = Color.White;
                        if (matrix[a, b] == 0)
                        {
                            matrix[a, b] = -2;
                            buttons[a, b].Text = "";
                            openNearbyCells(a, b);
                        }
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        //break;
                    }
                    
                }
        }

        private void gameOver()
        {
            // Метод делает недоступными все кнопки в поле и подсвечивает все мины
            for (int i = 0; i < buttons.GetLength(0); i++)
            {
                for (int j = 0; j < buttons.GetLength(1); j++)
                {
                    if (matrix[i, j] == -1)
                    {
                        buttons[i, j].Text = "*";
                        buttons[i, j].BackColor = Color.Red;
                    }
                    buttons[i, j].Enabled = false;
                }
            }
        }

        private void новичокToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setFieldSize(1);
            generateMatrix(10, 10, 10);
        }

        private void любительToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setFieldSize(2);
            generateMatrix(16, 16, 40);
        }

        private void профессионалToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setFieldSize(3);
            generateMatrix(30, 16, 99);
        }

        private void новаяИграToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (buttons == null)
            {
                setFieldSize(1);
                generateMatrix(10, 10, 10);
                return;
            }
            if (buttons.GetLength(0) == 10)
            {
                setFieldSize(1);
                generateMatrix(10, 10, 10);
                return;
            }
            if (buttons.GetLength(0) == 16)
            {
                setFieldSize(2);
                generateMatrix(16, 16, 40);
                return;
            }
            if (buttons.GetLength(0) == 30)
            {
                setFieldSize(3);
                generateMatrix(30, 16, 99);
                return;
            }
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }
    }
}
