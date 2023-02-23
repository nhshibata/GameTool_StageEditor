using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MapEditor
{
    public class TextureLoad
    {

        public void Init(TabControl tabControl, ItemCollection itemCollection)
        {
            // パスの取得
            string path = null;
            string[] headerName;
            {
                FindPath fp = new FindPath();
                path = fp.GetFolderPath("texture", false);
                // textureフォルダ内のサブフォルダを格納
                headerName = fp.GetSubFolderName(path);
            }

            // 1:パス 2: 拡張子*(全て) 2:サブフォルダを含めるか
            string[] aTexName = Directory.GetFiles(path, "*.png", SearchOption.AllDirectories);
            // 相対パスのままなので、絶対パスへ変更
            path = System.IO.Path.GetFullPath(path);

            // 追加用
            Dictionary<string, ListView> keyValuePairs = new Dictionary<string, ListView>();
            foreach (var name in headerName)
            {
                keyValuePairs.Add(name, new ListView());
            }


            foreach (var item in aTexName)
            {
                // 画像読み込み
                var source = new BitmapImage();
                source.BeginInit();
                source.UriSource = new Uri(path + item);
                source.EndInit();

                // コントロールとしてのImageを生成
                var addImage = new Image();
                addImage.Width = 50;
                addImage.Height = 50;
                addImage.Source = source;

                // mainのImageとして扱うため、もう一つ作成
                var mainImage = new Image();
                mainImage.Width = 50;
                mainImage.Height = 50;
                mainImage.Source = source;

                // 一致するフォルダ名があれば格納
                foreach (var header in keyValuePairs)
                {
                    if(item.Contains(header.Key))
                    {
                        header.Value.Items.Add(addImage);
                        addImage = null;
                        break;
                    }
                }
                itemCollection.Add(mainImage);

            }

            // タブ生成
            // 生成したタブにListViewを追加
            foreach (var header in keyValuePairs)
            {
                // TabItemを継承した
                DynamicTabItem tabItem = new DynamicTabItem();
                // texture(7) + /(1)
                int idx = header.Key.IndexOf("texture");
                string name = header.Key.Substring(idx + 8);
                tabItem.Header = name;
                // ListView
                tabItem.Add(header.Value);
                // TabControlに追加
                tabControl.Items.Add(tabItem);
            }
        }


    }
}
