using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Threading;
using System.Text.RegularExpressions;
using System.IO;

namespace Binary
{
    /// <summary>
    /// Main-Class Static inititlizations and functions
    /// </summary>
    public partial class Form1 : Form
    {
        Button[,] buttons = new Button[8, 8];
        Label[] labels = new Label[8];
        TextBox[] textboxes = new TextBox[8];
        static Boolean gameInProgress = true;
        static String[] buttonCaseDefault = new String[8] { "0", "0", "0", "0", "0", "0", "0", "0" };
        static int rowsInUse = 0;
        static int round = 0;
        static int level = 1;
        static int points = 0;
        public string username { get; set; }
        public int score { get; set; }
        List<Form1> parts = new List<Form1>();

        /// <summary>
        /// main-function: generating window-content and stating background worker
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            createButtonArray();
            createLabelArray();
            createTextboxesArray();
            writeDefaultConfig();
            label13.Hide();
            textBox9.Hide();
            button77.Hide();
            readConfig();
            label11.Text = Convert.ToString("Level : " + level);
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker1.RunWorkerAsync();
        }

        /// <summary>
        /// switches enabled buttons from 0 to 1 and 1 to 0.
        /// </summary>
        /// <param name="sender">sending object as "sender"</param>
        /// <param name="e">EventArgs as "e"</param>
        private void binaryButtonClick(object sender, EventArgs e)
        {
            
            Button tempButton = sender as Button;
            switch(tempButton.Text){
                case "0":
                    tempButton.Text = "1";
                    break;
                case "1":
                    tempButton.Text = "0";
                    break;
            }
            int row = (tempButton.Top + 24)/36;
            double dual = getDecimalCode(row);
            if (Convert.ToString(dual) == textboxes[row - 1].Text)
            {
                disableRow(row);
                rowsInUse--;
                moveRowsDown(row);
            }

        }

        /// <summary>
        /// moves all rows down to fill free space
        /// </summary>
        /// <param name="rowWasDiasbled">row that has been solved by the player</param>
        public void moveRowsDown(int rowWasDiasbled)
        {
            for (int i = rowWasDiasbled - 2; i >= 0; i--)
            {
                for (int d = 0; d <= 7; d++)
                {
                    buttons[i + 1, d].Text = buttons[i, d].Text;
                    buttons[i + 1, d].Enabled = buttons[i, d].Enabled;
                }
                textboxes[i + 1].Text = textboxes[i].Text;
                textboxes[i + 1].Enabled = textboxes[i].Enabled;
                textboxes[i + 1].ReadOnly = textboxes[i].ReadOnly;
                labels[i + 1].Enabled = labels[i].Enabled;
            }
            points++;
            label12.Text = "Points : " + points;
            disableRow(1);
        }


        /// <summary>
        /// put all buttons to Array "buttons[]"
        /// </summary>
        public void createButtonArray()
        {
            buttons = new Button[8,8] {
                                 {button1,button2,button3,button4,button5,button6,button7,button8},
                                 {button9,button10,button11,button12,button13,button14,button15,button16},
                                 {button17,button18,button19,button20,button21,button22,button23,button24},
                                 {button25,button26,button27,button28,button29,button30,button31,button32},
                                 {button33,button34,button35,button36,button37,button38,button39,button40},
                                 {button41,button42,button43,button44,button45,button46,button47,button48},
                                 {button49,button50,button51,button52,button53,button54,button55,button56},
                                 {button57,button58,button59,button60,button61,button62,button63,button64}
                             };
            foreach(Button tempButton in buttons){
                tempButton.Enabled = false;
            }
        }

        /// <summary>
        /// calculates decimal code for the current row
        /// </summary>
        /// <param name="row">row u want to know the decimal code</param>
        /// <returns>double decimal code</returns>
        public double getDecimalCode(int row)
        {
            double dual = 0;
            for (int i = 0; i <= 7; i++)
            {
                if (buttons[row - 1,i].Text == "1")
                {
                    dual += Math.Pow(2,i);
                }
            }
            return dual;
        }

        /// <summary>
        /// put all labels to array "labels[]"
        /// </summary>
        public void createLabelArray()
        {
            labels = new Label[8] {
                label3,
                label4,
                label5,
                label6,
                label7,
                label8,
                label9,
                label10
            };
            foreach (Label l in labels)
            {
                l.Enabled = false;
            }
        }

        /// <summary>
        /// put all textboxes to array "textboxes[]"
        /// </summary>
        public void createTextboxesArray() 
        {
            textboxes = new TextBox[8] {
                textBox1,
                textBox2,
                textBox3,
                textBox4,
                textBox5,
                textBox6,
                textBox7,
                textBox8
            };
            foreach (TextBox t in textboxes)
            {
                t.Enabled = false;
            }
        }

        /// <summary>
        /// enables the given row with the given case
        /// </summary>
        /// <param name="row">row that has to be enabled</param>
        /// <param name="gameRowCase">case which decides weather the user has to read or write dual code</param>
        /// <param name="dezimalWrite">in case the user has to write dual code this is the decimal number</param>
        /// <param name="buttonCases">in case the user has to read dual code this array contains the dual code</param>
        public void enableRow(int row, String gameRowCase, String dezimalWrite, String[] buttonCases)
        {
            if (rowsInUse <= 7){

                if (round == 10)
                {
                    round = 1;
                    level++;
                    label11.Text = Convert.ToString("Level : " + level);
                }else{
                    round++;
                }
                row = row - 1;
                this.labels[row].Enabled = true;
                textboxes[row].Enabled = true;
                switch (gameRowCase)
                {
                    case "write":
                        for (int i = 0; i <= 7; i++)
                        {
                            buttons[row, i].Enabled = true;
                        }
                        textboxes[row].Text = dezimalWrite;
                        textboxes[row].ReadOnly = true;
                        break;
                    case "read":
                        textboxes[row].ReadOnly = false;
                        for (int i = 0; i <= 7; i++)
                        {
                            buttons[row, i].Text = buttonCases[i];
                        }
                        break;
                }
                rowsInUse++;
            }
        }

        /// <summary>
        /// removes a solved row
        /// </summary>
        /// <param name="row">row that has to be enabled</param>
        public void disableRow(int row)
        {
            row = row - 1;
            labels[row].Enabled = false;
            textboxes[row].Enabled = false;
            textboxes[row].Text = "";
            for (int i = 0; i <= 7; i++)
            {
                buttons[row, i].Enabled = false;
            }
            for (int i = 0; i <= 7; i++)
            {
                buttons[row, i].Text = "0";
            }
        }

        /// <summary>
        /// backgroundworker 
        /// </summary>
        /// <param name="sender">sending object as "sender"</param>
        /// <param name="e">DoWorkEventArgs as "e"</param>
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            MethodInvoker mi;
            MethodInvoker labelsetter;
            MethodInvoker scoreInvoker;
            while (rowsInUse < 8)
            {
                if (gameInProgress)
                {
                    Random rnd = new Random();
                    String gameCase;
                    int temp = rnd.Next(0, 2);
                    int row = 8;
                    for (int i = 7; i >= 0; i--)
                    {
                        if (!buttons[i, 0].Enabled && !textboxes[i].Enabled)
                        {
                            row = i + 1;
                            break;
                        }
                    }
                    switch (temp)
                    {
                        case 0:
                            gameCase = "write";
                            String dezimalWrites = Convert.ToString(rnd.Next(1, 256));
                            mi = delegate() { enableRow(row, gameCase, dezimalWrites, buttonCaseDefault); };
                            this.Invoke(mi);
                            break;
                        case 1:
                            gameCase = "read";
                            String[] buttonValuesGame = new String[8];
                            for (int i = 0; i <= 7; i++)
                            {
                                buttonValuesGame[i] = Convert.ToString(rnd.Next(0, 2));
                            }
                            mi = delegate() { enableRow(row, gameCase, "", buttonValuesGame); };
                            this.Invoke(mi);
                            break;
                    }

                    double value = (7 - (10 / Convert.ToDouble(level)) + 2);
                    labelsetter = delegate() { label14.Text = "Time next row : " + Convert.ToString(Math.Round(12 - value,2)); };
                    this.Invoke(labelsetter);
                    value = value * 1000;
                    int time = Convert.ToInt32(Math.Floor(value));
                    Thread.Sleep(12000 - time);
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
            gameInProgress = false;
            for (int i = 1; i <= 8; i++)
            {
                mi = delegate() { disableRow(i); };
                this.Invoke(mi);                                            // muss für den Highscoren entfernt werden, da der backgroundworker sonst vorzeitig startet.
            }
            backgroundWorker1.CancelAsync();
            if (MessageBox.Show("You got " + points + " points! (Press \"Restart\" to start a new Game.)", "Game Over!", MessageBoxButtons.OK, MessageBoxIcon.Information) == DialogResult.OK)
            {
            }
            //scoreInvoker = delegate() { startHighscore(); };
            //this.Invoke(scoreInvoker);                                    // FEHLER in den Folgemethoden. Index außerhalb des Array-Bereichs. Highscore kann leider nicht geschrieben werden. (daher unbenutzte methoden)
        }

        /// <summary>
        /// Hides the game and show highscore layout
        /// </summary>
        /// currently not in use
        public void startHighscore()
        {
            hideAll();
            label13.Show();
            textBox9.Show();
            button77.Show();
            backgroundWorker1.CancelAsync();
        }
        
        /// <summary>
        /// listens to right textbox content
        /// </summary>
        /// <param name="sender">sender object as "sender"</param>
        /// <param name="e">EventArgs as "e"</param>
        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            TextBox box = sender as TextBox;
            int row = (box.Top + 24) / 36;
            if (box.Text == Convert.ToString(getDecimalCode(row)))
            {
                disableRow(row);
                rowsInUse--;
                moveRowsDown(row);
            }
        }

        /// <summary>
        /// pauses the game
        /// </summary>
        /// <param name="sender">sender object as "sender"</param>
        /// <param name="e">EventArgs as "e"</param>
        private void button73_Click(object sender, EventArgs e)
        {
            foreach (Button tempbutton in buttons)
            {
                tempbutton.Hide();
            }
            foreach (Label templabel in labels)
            {
                templabel.Hide();
            }
            foreach (TextBox tempbox in textboxes)
            {
                tempbox.Hide();
            }
            gameInProgress = false;
            if (MessageBox.Show("Would you like to continue your game?", "Program paused", MessageBoxButtons.OK, MessageBoxIcon.Question) == DialogResult.OK)
            {
                foreach (Button tempbutton in buttons)
                {
                    tempbutton.Show();
                }
                foreach (Label templabel in labels)
                {
                    templabel.Show();
                }
                foreach (TextBox tempbox in textboxes)
                {
                    tempbox.Show();
                }
                gameInProgress = true;
            }
        }

        /// <summary>
        /// Exit button. Closes the game
        /// </summary>
        /// <param name="sender">sender object as "sender"</param>
        /// <param name="e">EventArgs as "e"</param>
        private void button75_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Restart button. Restarts the game
        /// </summary>
        /// <param name="sender">sender object as "sender"</param>
        /// <param name="e">EventArgs as "e"</param>
        private void button74_Click(object sender, EventArgs e)
        {
            for (int i = 1; i < 9; i++)
            {
                disableRow(i);
            }
            rowsInUse = 0;
            round = 0;
            readConfig();
            label11.Text = Convert.ToString("Level : " + level);
            points = 0;
            label12.Text = "Points : " + points;
            if (gameInProgress == false)
            {
                gameInProgress = true;
                backgroundWorker1.RunWorkerAsync();
            }
        }

        /// <summary>
        /// generates the default config
        /// </summary>
        public void writeDefaultConfig()
        {
            string defaultsettings = "start at Level: 1";
            string serializationFile = Path.Combine(Environment.CurrentDirectory, "binary.conf");
            if (File.Exists(serializationFile))
            {
            }
            else
            {
                StreamWriter sw = new StreamWriter(serializationFile);
                sw.Write(defaultsettings);
                sw.Close();
            }
        }

        /// <summary>
        /// saves Highscores ti file
        /// </summary>
        /// <param name="points">integer points the player has reached</param>
        /// <param name="name">string name the player has entered</param>
        /// currently not in use
        public void saveHighScore(int points, string name)
        {
            string serializationFile = Path.Combine(Environment.CurrentDirectory, "highscoresBinary.csv");
            if (File.Exists(serializationFile))
            {
                StreamReader rw = new StreamReader(serializationFile);
                int filelength = 0;
                string line;
                while ((line = rw.ReadLine()) != null)
                {
                    filelength++;
                }
                rw.Close();
                StreamReader rw1 = new StreamReader(serializationFile);
                //char newline = Convert.ToChar(Environment.NewLine);
                string[] array1 = new string[filelength];
                for (int i = 0; i < filelength; i++)
                {
                    array1[i] = rw1.ReadLine();
                }
                if (array1.Length > 21)
                {
                    for (int i = 1; i<= 20; i++)
                    {
                        parts.Add(new Form1() { username = array1[i].Split('|')[0], score = Convert.ToInt32(array1[i].Split('|')[1]) });
                    }
                }
                else
                {
                    for (int i = 1; i < array1.Length; i++)
                    {
                        parts.Add(new Form1() { username = array1[i].Split('|')[0], score = Convert.ToInt32(array1[i].Split('|')[1]) });
                    }
                }
                int d = 0;
                int partslength = 0;
                foreach(Form1 element in parts)
                {
                    if (points > element.score)
                    {
                        parts.Insert(d, new Form1() { username = name, score = points });
                        d++;
                    }else if (array1.Length < 21)
                    {
                        parts.Add(new Form1() { username = name, score = points });
                    }
                    partslength++;
                }

                StreamWriter sw = new StreamWriter(serializationFile);
                sw.Write("");
                sw.WriteLine("username;score");
                if (partslength < 20)
                {
                    for (int i = 0; i <= partslength; i++)
                    {
                        sw.WriteLine(parts[i].username + ";" + parts[i].score);
                    }
                }
                else
                {
                    for (int i = 0; i <= 20; i++)
                    {
                        sw.WriteLine(parts[i].username + ";" + parts[i].score);
                    }
                }
                sw.Close();
            }
            else
            {
                StreamWriter sw = new StreamWriter(serializationFile);
                sw.WriteLine("username;score");
                sw.WriteLine(name + ";" + points);
                sw.Close();
            }
        }

        /// <summary>
        /// reads the config to set the startinglevel
        /// </summary>
        public void readConfig()
        {
            string file = Path.Combine(Environment.CurrentDirectory, "binary.conf");
            if (File.Exists(file))
            {
                StreamReader rw = new StreamReader(file);
                
                level = Convert.ToInt32(Convert.ToString(rw.ReadLine()).Split(':')[1]);
                rw.Close();

            }
            else
            {
                MessageBox.Show("Could not find the config-file. Please restart the program!", "File Not Found!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// hides all game-elements
        /// </summary>
        public void hideAll()
        {
            foreach (Button tempbutton in buttons)
            {
                tempbutton.Hide();
            }
            foreach (Label templabel in labels)
            {
                templabel.Hide();
            }
            foreach (TextBox tempbox in textboxes)
            {
                tempbox.Hide();
            }
            button72.Hide();
            button71.Hide();
            button70.Hide();
            button69.Hide();
            button68.Hide();
            button67.Hide();
            button66.Hide();
            button65.Hide();
            label2.Hide();
            label1.Hide();
            groupBox1.Hide();
            groupBox2.Hide();
            label11.Hide();
            label12.Hide();
            button73.Hide();
            button74.Hide();
            button75.Hide();
        }

        /// <summary>
        /// shows all game-elements
        /// </summary>
        public void showAll()
        {
            foreach (Button tempbutton in buttons)
            {
                tempbutton.Show();
            }
            foreach (Label templabel in labels)
            {
                templabel.Show();
            }
            foreach (TextBox tempbox in textboxes)
            {
                tempbox.Show();
            }

            button72.Show();
            button71.Show();
            button70.Show();
            button69.Show();
            button68.Show();
            button67.Show();
            button66.Show();
            button65.Show();
            label2.Show();
            label1.Show();
            groupBox1.Show();
            groupBox2.Show();
            label11.Show();
            label12.Show();
            button73.Show();
            button74.Show();
            button75.Show();
        }

        /// <summary>
        /// Button set name for highscore
        /// </summary>
        /// <param name="sender">sender object as "sender"</param>
        /// <param name="e">EventArgs as "e"</param>
        /// currently not in use
        private void button77_Click(object sender, EventArgs e)
        {
            if (textBox9.Text.Equals(""))
            {
                MessageBox.Show("Please enter a Name!", "No name given!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (textBox9.Text.Contains(" "))
            {
                MessageBox.Show("There are no space-characters allowed in the unsername!", "Invalid name!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {

                saveHighScore(points, textBox9.Text);
                label13.Hide();
                textBox9.Hide();
                button77.Hide();

                for (int i = 1; i < 9; i++)
                {
                    disableRow(i);
                }
                rowsInUse = 0;
                round = 0;
                readConfig();
                label11.Text = Convert.ToString("Level : " + level);
                points = 0;
                label12.Text = "Points : " + points;
                showAll();

                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void Form1_Activated(object sender, EventArgs e)
        {

        }

    }
}
