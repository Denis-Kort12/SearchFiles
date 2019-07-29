using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FindFile
{
    class FindFile
    {
        private string directoryStart; //Директория с которой начинается считывание
        private string formatFileName;
        private string text;

        private List<string> listDirectory = new List<string>(); //Коллекция директорий
        private List<string> listFile = new List<string>(); //Коллекция директорий

        public string DirectoryStart
        {
            set
            {
                if (Directory.Exists(value))
                {
                    directoryStart = value;
                }
            }
        }

        public string FormatFileName
        {
            set
            {
                formatFileName = value;
            }
        }

        public string Text
        {
            set
            {
                text = value;
            }
        }

        public FindFile(string startPath, string formatFileName, string text) //Конструктор
        {
            DirectoryStart = startPath;
            FormatFileName = formatFileName;
            Text = text;

            listDirectory.Add(directoryStart);
        }


        private void ScanDirectory() //Поиск всех директорий со стартовой
        {
            try
            {
                foreach (string s in Directory.GetDirectories(directoryStart))
                {
                    listDirectory.Add(s);
                    directoryStart = s;
                    ScanDirectory();
                }
            }
            catch (System.Exception excpt)
            {
                //MessageBox.Show(excpt.Message);
            }
        }

        private void ScanFile() //Поиск файлов указанного формата
        {
            try
            {
                foreach (string s in listDirectory)
                {
                    try
                    {
                        foreach (string f in Directory.GetFiles(s, formatFileName))
                        {
                            try
                            {
                                listFile.Add(f);
                            }
                            catch
                            {
                            }

                        }
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        public List<string> WriteFile()
        {
            ScanDirectory();
            ScanFile();

            return listFile;
        }


        public List<string> FindTextInFile()
        {
            List<string> readyFiles = new List<string>();

            for (int i = 0; i < listFile.Count; i++)
            {
                try
                {
                    string text = File.ReadAllText(listFile[i], Encoding.GetEncoding(1251));

                    if (text.Contains(this.text))
                    {
                        readyFiles.Add(listFile[i]);
                    }
                }
                catch
                {
                }
            }

            return readyFiles;
        }

    }
}

