using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;



namespace MapEditor
{

    /// <summary>
    /// ダイアログ選択
    /// </summary>
    public class DispDialog
    {

        /// <summary>
        /// ダイアログを開き、選択したファイルを返す
        /// </summary>
        /// <param name="extension">拡張子</param>
        /// <returns></returns>
        public string OpenDialog()
        {
            using (var cofd = new CommonOpenFileDialog()
            {
                Title = "フォルダを選択してください",
                InitialDirectory = @"D:\Users\threeshark",
                // フォルダ選択モードにする
                IsFolderPicker = false,
            })
            {
                cofd.Filters.Add(new CommonFileDialogFilter("CSV", "*.csv,*.tsv"));
                cofd.Filters.Add(new CommonFileDialogFilter("JPG", "*.jpg"));

                if (cofd.ShowDialog() != CommonFileDialogResult.Ok)
                {
                    return null;
                }

                // FileNameで選択されたフォルダを取得する
                //System.Windows.MessageBox.Show($"{cofd.FileName}を選択しました");
                
                return cofd.FileName;
            }

            //var ofd = new Microsoft.Win32.OpenFileDialog()
            //{
            //    Title = "フォルダを選択してください",
            //    InitialDirectory = @"D:\Users\threeshark",
            //    Filter = "Folder|.",
            //    CheckFileExists = false,
            //};

            //// OKが押されずに終わった場合はfalseかnullが返る
            //if (ofd.ShowDialog() != true)
            //{
            //    return null;
            //}

            //// FileNameで選択されたフォルダを取得したい…のだが、
            //// そもそもフォルダを選んでOKを押しても
            //// その中に移動するだけで確定されない
            //System.Windows.MessageBox.Show($"{ofd.FileName}を選択しました");

            //return ofd.FileName;
        }

    }
}
