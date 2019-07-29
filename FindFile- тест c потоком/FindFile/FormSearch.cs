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
    public partial class FormSearch : Form
    {
        public FormSearch()
        {
            InitializeComponent();
        }

        CancellationTokenSource cancelTokenSource;
        CancellationToken token;

        private int iFile;

        List<string> listFile = new List<string>();
        List<string> readyListFile = new List<string>();

        int saveListFileI;
        int saveiFile;

        string startPath = Criterion.startPath;
        string nameFormat = Criterion.nameFormat;
        string text = Criterion.text;

        private void button1_Click(object sender, EventArgs e)
        {
            button3.Enabled = false;
            button1.Enabled = false;

            saveListFileI = 0;
            saveiFile = 0;
            dateTimeI = 0;
            treeView1.Nodes.Clear();

            label4.Text = "Файл обрабатывается: ";
            label5.Text = "Идет поиск файлов с данным форматом. Подождите, пока все файлы будут готовы для последующей обработки...";
            label6.Text = "Время(в секундах): ";

            timer1.Enabled = true;
            timer1.Start();

            cancelTokenSource = new CancellationTokenSource();
            token = cancelTokenSource.Token;

            Task task1 = new Task(() => ThredPoisk(token));
            task1.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            button1.Enabled = true;
            button3.Enabled = true;

            cancelTokenSource.Cancel();
            timer1.Stop();
        }

        List<string> listTree = new List<string>();

        public void DrawTree()
        {
            label4.Invoke((MethodInvoker)(() => label4.Text = "Идет построение дерева..."));

            treeView1.PathSeparator = @"\";
            PopulateTreeView(treeView1, readyListFile, '\\');
        }

        private static void PopulateTreeView(TreeView treeView, IEnumerable<string> paths, char pathSeparator)
        {
            TreeNode lastNode = null;
            string subPathAgg;
            foreach (string path in paths)
            {
                subPathAgg = string.Empty;
                foreach (string subPath in path.Split(pathSeparator))
                {
                    subPathAgg += subPath + pathSeparator;
                    TreeNode[] nodes = treeView.Nodes.Find(subPathAgg, true);
                    if (nodes.Length == 0)
                        if (lastNode == null)
                            treeView.Invoke((MethodInvoker)(() => lastNode = treeView.Nodes.Add(subPathAgg, subPath)));
                        else
                            treeView.Invoke((MethodInvoker)(() => lastNode = lastNode.Nodes.Add(subPathAgg, subPath)));
                    else
                        treeView.Invoke((MethodInvoker)(() => lastNode = nodes[0]));
                }
            }
        }

        public void ThredPoisk(CancellationToken token)
        {
            iFile = saveiFile;

            FindFile findFile = new FindFile(startPath, nameFormat, text);

            listFile = findFile.WriteFile();

            button2.Invoke((MethodInvoker)(() => button2.Enabled = true));  

            for (int i = saveListFileI; i < listFile.Count; i++)
            {
                string s = listFile[i];

                if (token.IsCancellationRequested)
                {
                    saveiFile = iFile;
                    saveListFileI = i;

                    MessageBox.Show("Операция остановлена");
                    return;
                }

                iFile++;

                label4.Invoke((MethodInvoker)(() => label4.Text = "Файл обрабатывается: \n" + s));
               // listBox1.Invoke((MethodInvoker)(() => listBox1.Items.Add(s)));
                label5.Invoke((MethodInvoker)(() => label5.Text = "Количество обработанных файлов: \n" + iFile));

               // Thread.Sleep(50);

               // listBox1.Invoke((MethodInvoker)(() => listBox1.Refresh()));
                label4.Invoke((MethodInvoker)(() => label4.Refresh()));
                label5.Invoke((MethodInvoker)(() => label5.Refresh()));
            }

            readyListFile = findFile.FindTextInFile();

            if(readyListFile.Count == 0)
            {
                timer1.Stop();

                MessageBox.Show("Не было найдено ни одного файла");
                button1.Invoke((MethodInvoker)(() => button1.Enabled = true));
                button2.Invoke((MethodInvoker)(() => button2.Enabled = false));
                button3.Invoke((MethodInvoker)(() => button3.Enabled = false));
                label5.Invoke((MethodInvoker)(() => label5.Text = "Количество обработанных файлов: "));
                label4.Invoke((MethodInvoker)(() => label4.Text = "Файл обрабатывается: "));

                return;
            }

            DrawTree();

            timer1.Stop();

            label4.Invoke((MethodInvoker)(() => label4.Text = "Дерево построено!"));

            MessageBox.Show("Поиск завершён!");

            button1.Invoke((MethodInvoker)(() => button1.Enabled = true));
            button2.Invoke((MethodInvoker)(() => button2.Enabled = false));
            button3.Invoke((MethodInvoker)(() => button3.Enabled = false));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button2.Enabled = true;
            button1.Enabled = false;
            button3.Enabled = false;

            cancelTokenSource = new CancellationTokenSource();
            token = cancelTokenSource.Token;

            label5.Text = "Идет поиск файлов с данным форматом. Подождите, пока все файлы будут готовы для последующей обработки...";

            Task task1 = new Task(() => ThredPoisk(token));
            task1.Start();

            timer1.Start();
        }

        private int dateTimeI;

        private void timer1_Tick(object sender, EventArgs e)
        {
            dateTimeI++;
            label6.Text = "Время(в секундах): " + dateTimeI.ToString();
            label6.Refresh();
        }

        private void FormSearch_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
