using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace DataEncryptionLab4_Kot_PC_21_MH
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            toolTip1.SetToolTip(button_fileKEYsave, "Зберегти");
            toolTip1.SetToolTip(button_fileKEYopen, "Відкрити");
        }

        private void button_fileIN_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog()
            {
                InitialDirectory = Directory.GetCurrentDirectory(),
                Filter = "text files (*.txt)|*.txt|All files (*.*)|*.*",
                FilterIndex = 2,
                RestoreDirectory = true
            })
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    textBox_fileIN.Text = openFileDialog.FileName;
                }
            }
        }

        private void button_fileOUT_Click(object sender, EventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                textBox_fileOUT.Text = saveFileDialog.FileName;
            }
        } 

        private void button_fileKEYopen_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog()
            {
                InitialDirectory = Directory.GetCurrentDirectory(),
                Filter = "text files (*.txt)|*.txt|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true
            })
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    textBox_fileKEY.Text = openFileDialog.FileName;
                }
            }
        }

        private void button_fileKEYsave_Click(object sender, EventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                textBox_fileKEY.Text = saveFileDialog.FileName;
            }
        }

        private void button_KEYgenerator_Click(object sender, EventArgs e)
        {
            var dialogResult = MessageBox.Show(
                "Стоворити новий ключ?", 
                "Увага",
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                Cursor.Current = Cursors.WaitCursor;
                string fileINpath = textBox_fileIN.Text;
                if (File.Exists(fileINpath))
                {
                    var fileKEYpath = textBox_fileKEY.Text;
                    var dirKEYpath = Path.GetDirectoryName(fileKEYpath);
                    if (Directory.Exists(dirKEYpath))
                    {
                        var lengthINfile = new FileInfo(fileINpath).Length;
                        var arrKEY = new byte[lengthINfile];
                        var rngCsp = new RNGCryptoServiceProvider();
                        rngCsp.GetBytes(arrKEY);
                        using (var fs = File.Create(fileKEYpath))
                        {
                            fs.Write(arrKEY, 0, arrKEY.Length);
                            fs.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Шлях до ключа не вказаний\nабо такий шлях не існує");
                        textBox_fileKEY.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("Вхідний файл не існує");
                    button_fileIN.Focus();
                }
                Cursor.Current = Cursors.Default;
            }
        }

        private void mySaveToFileOUT(byte[] arrCipher, string fileOutPath)
        {
            using(var fs = File.Create(fileOutPath))
            {
                fs.Write(arrCipher, 0, arrCipher.Length);
                fs.Close();
            }

            file_size.Text = $"Розмір ключа: \n{arrCipher.Length} байт";
        }

        private byte[] myCipherXOR(string fileINPath, string fileKEYPath)
        {
            var arrIN = File.ReadAllBytes(fileINPath);
            var arrKEY = File.ReadAllBytes(fileKEYPath);
            var lenIN = arrIN.Length;
            var arrCipher = new byte[lenIN];

            for (var i = 0; i < lenIN; i++)
            {
                var p = arrIN[i];
                var k = arrKEY[i % lenIN];
                
                arrCipher[i] = (byte)(p ^ k);
            }

            return arrCipher;
        }

        private void button_start_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            var fileINpath = textBox_fileIN.Text;
            var fileOUTPath = textBox_fileOUT.Text;
            var fileKEYpath = textBox_fileKEY.Text;

            if (File.Exists(fileINpath))
            {
                var dirOutPath = Path.GetDirectoryName(fileOUTPath);
                if (Directory.Exists(dirOutPath))
                {
                    if (File.Exists(fileKEYpath))
                    {
                        var lenINfile = new FileInfo(fileINpath).Length;
                        var lenKeyFIle = new FileInfo(fileKEYpath).Length;

                        if (lenINfile == lenKeyFIle || lenINfile >= lenKeyFIle)
                        {
                            var stopwatch = new Stopwatch();
                            stopwatch.Start();

                            var arrCipher = myCipherXOR(fileINpath, fileKEYpath);
                            mySaveToFileOUT(arrCipher, fileOUTPath);

                            stopwatch.Stop();
                            label_time.Text = stopwatch.Elapsed.ToString(@"mm\:ss\.fff");
                        }
                        else
                        {
                            MessageBox.Show("Розмір файлу ключа не співпадає\nз розміром або довжина файлу ключа більша");
                            button_fileKEYopen.Focus();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Шлях до файлу ключа не вказаний\n або такий файл не існує");
                        textBox_fileKEY.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("Шлях до вихідного файлу не вказаний\n або такий шлях не існує");
                    textBox_fileOUT.Focus();
                }
            }
            else
            {
                MessageBox.Show("Вхідний файл не існує");
                button_fileIN.Focus();
            }

            Cursor.Current = Cursors.Default;
        }
    }
}
