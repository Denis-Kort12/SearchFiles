using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FindFile
{
    public partial class Form1 : Form
    {  
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) //Выбор директории
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "")
            {
                MessageBox.Show("Поля с обязательной информацией не заполнены");
            }
            else
            {
                Criterion.startPath = textBox1.Text;
                Criterion.nameFormat = textBox2.Text;
                Criterion.text = textBox3.Text;

                using (StreamWriter sw = new StreamWriter("Remember.txt", false, System.Text.Encoding.Default))
                {
                    sw.WriteLine(Criterion.startPath);
                    sw.WriteLine(Criterion.nameFormat);
                    sw.WriteLine(Criterion.text);
                }

                FormSearch frm = new FormSearch();
                frm.Show();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            List<string> massivTextInFile = new List<string>();

            if (File.Exists("Remember.txt"))
            {
                using (StreamReader sr = new StreamReader("Remember.txt", System.Text.Encoding.Default))
                {
                    string line;

                    while ((line = sr.ReadLine()) != null)
                    {
                        massivTextInFile.Add(line);
                    }
                }
            }

            textBox1.Text = massivTextInFile[0];
            textBox2.Text = massivTextInFile[1];

            for (int i = 2; i < massivTextInFile.Count; i++)
            {
                if (massivTextInFile.Count - 1 == i) textBox3.Text += massivTextInFile[i];
                else textBox3.Text += massivTextInFile[i] + Environment.NewLine;
            }
             
        }
    }
}
