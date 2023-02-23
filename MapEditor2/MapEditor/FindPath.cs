using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MapEditor
{
    public class FindPath
    {

        public string[] GetSubFolderName(string folder)
        {
            return System.IO.Directory.GetDirectories(folder);
        }

        public string GetFolderPath(string folder, bool absolutePath)
        {
            // パスの取得
            string path = null;
            // カレントディレクトリの取得
            string[] abPath = System.IO.Directory.GetDirectories(System.IO.Directory.GetCurrentDirectory());

            foreach (var item in abPath)
            {
                if (item.Contains(folder))
                {
                    path = item;
                    break;
                }
            }
            if (path == null)
            {
                string dir = null;
                // pathがnullの限り
                do
                {
                    dir += "..\\";
                    abPath = System.IO.Directory.GetDirectories(dir);
                    foreach (var item in abPath)
                    {
                        if (item.Contains(folder))
                        {
                            path = item;
                            break;
                        }
                    }
                    // 右の値まで探索してもなければ抜ける
                    if (dir.Count() > 4 * 3)
                        return null;
                } while (path == null);
            }
            
            // フラグ有なら
            // 相対パスのままなので、絶対パスへ変更
            return absolutePath ? System.IO.Path.GetFullPath(path) : path;

        }

    }
}
