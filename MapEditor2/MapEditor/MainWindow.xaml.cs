using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace MapEditor
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }

        /// <summary>
        /// ウィンドウコントロール描画後呼び出し
        /// コントロール読み込み終了したい時に
        /// </summary>
        /// <param name="sender">no</param>
        /// <param name="e">no</param>
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            // 初期サイズ保存
            ViewOperation.m_instance.m_fStartMapWidth = gridOrder.RenderSize.Width;
            ViewOperation.m_instance.m_fStartMapHeight = gridOrder.RenderSize.Height;

            // jsonが読みこまれているのでサイズ調整
            GridRowResize(0);
            // jsonが読みこまれているのでサイズ調整
            GridRowResize(ToolSetting.m_instance.m_nPanelH);
            GridColumnResize(ToolSetting.m_instance.m_nPanelW);

            GridUnitType type = ViewOperation.m_instance.m_bViewMode ? GridUnitType.Star : GridUnitType.Pixel;
            int gridSize = ViewOperation.m_instance.m_bViewMode ? 1 : ToolSetting.m_instance.m_nGridSize;
            foreach (RowDefinition item in myGrid.RowDefinitions)
            {
                item.Height = new GridLength(gridSize, type);
            }

            foreach (ColumnDefinition item in myGrid.ColumnDefinitions)
            {
                item.Width = new GridLength(gridSize, type);
            }

            TextureLoad textureLoad = new TextureLoad();
            textureLoad.Init(myTab, ImageList.Items);

            foreach (var item in ImageList.Items)
            {
                Image image = item as Image;
                image.PreviewMouseLeftButtonUp += Image_PreviewMouseLeftButtonUp;
            }

            foreach (var item in myTab.Items)
            {
                var tabItem = item as DynamicTabItem;
                if (tabItem == null)
                    continue;

                ListView listView = tabItem.GetChild() as ListView;
                listView.PreviewMouseMove += ListView_PreviewMouseMove;

                foreach (var i in listView.Items)
                {
                    Image image = i as Image;
                    if(image != null)
                        image.PreviewMouseLeftButtonUp += Image_PreviewMouseLeftButtonUp;
                }
            } 

        }

        // object sender ...このイベントを呼び出したコントロールの情報
        // 今回の場合はListViewコントロール
        private void ListView_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            // 念のため、senderがListViewコントロールか確認
            ListView listView = sender as ListView; // as ... C#の型変換キーワード、失敗したらnull
            if (listView == null)
                return;
            // まず、ドラッグアンドドロップのためにボタンが押されているか判定
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            // さらに、ドラッグアンドドロップで動かすアイテムが選択されているか判定
            // SelectedItem...現在選択中
            if (listView.SelectedItem == null)
                return;

            // 問題なさそうなので、ドラッグアンドドロップ実行
            DragDrop.DoDragDrop(
                listView,               // ドラッグアンドドロップを実行し始めるcontrol
                listView.SelectedItem,  // ドラッグアンドドロップで渡す情報
                DragDropEffects.Move    // ドラッグアンドドロップ中のマウスのアイコン
                );

        }

        // DragEventArgs ドラッグアンドドロップに関する色んな情報
        private void Grid_Drop(object sender, DragEventArgs e)
        {
            // senderがドロップされるGridか確認
            Grid grid = sender as Grid;
            if (grid == null)
                return;

            int colum = grid.ColumnDefinitions.Count;   // 方向の区切り線
            int row = grid.RowDefinitions.Count;        // 方向の区切り線
            double width = grid.RenderSize.Width;       // 横方向のサイズ
            double height = grid.RenderSize.Height;     // 縦方向のサイズ

            // ドロップされた位置を計算
            Point pos = e.GetPosition(grid); // gridコントロール内の位置を取得
            if (!ViewOperation.m_instance.m_bViewMode)
            {
                width = gridScroll.ViewportWidth;       // 横方向のサイズ
                height = gridScroll.ViewportHeight;     // 縦方向のサイズ
                colum = (int)gridScroll.ViewportWidth / ToolSetting.m_instance.m_nGridSize;   // 方向の区切り線
                row = (int)gridScroll.ViewportHeight / ToolSetting.m_instance.m_nGridSize;        // 方向の区切り線
            }

            // 1マス辺りのサイズを計算
            double cellWidth  = width  / colum;
            double cellHeight = height / row;
            
            // 1マス当たりのサイズから、マウスの位置がマスのどのインデックスになるか計算
            int cellX = (int)(pos.X / cellWidth);
            int cellY = (int)(pos.Y / cellHeight);

            Image image = null;
            // グリッドの該当箇所のcontrolを取得
            foreach (UIElement element in grid.Children)
            {
                int x = (int)element.GetValue(Grid.ColumnProperty);
                int y = (int)element.GetValue(Grid.RowProperty);
                if(cellX == x && cellY == y)
                {
                    image = element as Image;
                    break;
                }
            }

            //Image image = grid.Children[cellX * row + cellY] as Image;    // 2次元のインデックスを1次元に変換
            if (image == null)
                return;

            // ドロップされてきたアイテムを該当箇所に入れる
            Image dropImage = e.Data.GetData(typeof(Image)) as Image;
            if (dropImage == null)  // 確認
                return;

            // ドロップイメージをグリッドのイメージに適用
            image.Source = dropImage.Source;
        }

        private void TextBox_IsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if((bool)e.OldValue == true && (bool)e.NewValue == false)
            {
                TextBox textBox = sender as TextBox;
                if (textBox == null) return;

                int newColumn = int.Parse(textBox.Text);
                GridColumnResize(newColumn);
                MyGrid_SizeChanged(null, null);
            }
        }

        private void TextBox_IsKeyboardFocusedChanged_1(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.OldValue == true && (bool)e.NewValue == false)
            {
                TextBox textBox = sender as TextBox;
                if (textBox == null) return;

                int newRow = int.Parse(textBox.Text);
                GridRowResize(newRow);
                MyGrid_SizeChanged(null, null);
            }
        }

        /// <summary>
        /// 全グリッドPixelサイズ調整
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_IsKeyboardFocusedChanged_2(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (myGrid == null)
                return;
            if (ViewOperation.m_instance.m_bViewMode)
                return;

            foreach (RowDefinition item in myGrid.RowDefinitions)
            {
                item.Height = new GridLength(ToolSetting.m_instance.m_nGridSize, GridUnitType.Pixel);
            }

            foreach (ColumnDefinition item in myGrid.ColumnDefinitions)
            {
                item.Width = new GridLength(ToolSetting.m_instance.m_nGridSize, GridUnitType.Pixel);
            }
        }

        /// <summary>
        /// サイズ変更時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            float cellWidth = (float)gridOrder.RenderSize.Width /ToolSetting.m_instance.m_nPanelW;
            float cellHeight = (float)gridOrder.RenderSize.Height /ToolSetting.m_instance.m_nPanelH;

            //--- 横幅が長いパターン
            //if(e.NewSize.Width > e.NewSize.Height)
            if (cellWidth > cellHeight)
            {
                // 横幅に合わせて、横幅を短くする
                cellWidth = cellHeight;
            }

            //--- 縦幅が長いパターン
            //if(e.NewSize.Height > e.NewSize.Width)
            if(cellHeight > cellWidth)
            {
                // 横幅に合わせて、横幅を短くする
                cellHeight = cellWidth;
            }

            if (ViewOperation.m_instance.m_bViewMode)
            {// 補正した後の長さで、全体のサイズを計算
                myGrid.Width = cellWidth * ToolSetting.m_instance.m_nPanelW;
                myGrid.Height = cellHeight * ToolSetting.m_instance.m_nPanelH;
            }
            else
            {
                float width = ToolSetting.m_instance.m_nGridSize * ToolSetting.m_instance.m_nPanelW;
                float height = ToolSetting.m_instance.m_nGridSize * ToolSetting.m_instance.m_nPanelH;
                // 規定値よりサイズを小さくしない
                myGrid.Width = width < ViewOperation.m_instance.m_fStartMapWidth ? ViewOperation.m_instance.m_fStartMapWidth : width;
                myGrid.Height = height < ViewOperation.m_instance.m_fStartMapHeight ? ViewOperation.m_instance.m_fStartMapHeight : height;
            }

        }

        private void WriteJson(object sender, RoutedEventArgs e)
        {
            MapData data = new MapData();
            // パネルの大きさ
            data.width = ToolSetting.m_instance.m_nPanelW;
            data.height = ToolSetting.m_instance.m_nPanelH;

            // 必要な分のリストを確保
            data.noMap = new List<int>(data.width * data.height);

            for (int i = 0; i < data.width * data.height; i++)
            {
                data.noMap.Insert(i, -1);
            }

            for (int i = 0; i < data.noMap.Count; i++)
            {
                // グリッド上の位置
                int x = i % data.width;
                int y = i / data.width;

                // グリッドの中から該当のデータを探索
                foreach (Image item in myGrid.Children)
                {
                    // imgeListの並び順を番号として設定
                    if( x != (int)item.GetValue(Grid.ColumnProperty) || y !=(int)item.GetValue(Grid.RowProperty))
                    {
                        continue;
                    }

                    for (int j = 0; j < ImageList.Items.Count; j++)
                    {
                        if ((ImageList.Items[j] as Image).Source == item.Source)
                        {
                            data.noMap[i] = j;
                            break;
                        }
                    }
                    break;
                }
            }

            //--- ﾃﾞｰﾀの出力
            data.Write();
            MessageBox.Show("jsonを出力しました");
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            // フォルダの表示はデフォルトで追加されていない
            // ソリューションエクスプローラー > 参照 > (右クリック) > 参照の追加

            // OKボタン
            if(fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // 選択されているパス表示
                //FolderPath.Text = fbd.SelectedPath;
            }

        }

        /// <summary>
        /// ボタン入力時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // 存在するか確認
            if (!File.Exists("MapData.json"))
                return;

            string moveFile = ToolSetting.m_instance.folderPath + "\\MapData.json";
            if (File.Exists(moveFile))
                File.Delete(moveFile);

            // MapData.jsonをFolderPathの場所へ移動
            File.Move("MapData.json", ToolSetting.m_instance.folderPath + "\\MapData.json");

            // 現在ディレクトリ保存
            string toolDir = Directory.GetCurrentDirectory();

            Directory.SetCurrentDirectory(ToolSetting.m_instance.folderPath);

            // FolderPathの中にあるJsonParser.exeを起動
            Process exe = Process.Start(ToolSetting.m_instance.folderPath + "\\JsonParser.exe");

            // jsonParsar.exeが終了するまで待機
            exe.WaitForExit();

            Directory.SetCurrentDirectory(toolDir);

        }

        /// <summary>
        /// グリッド列修正
        /// </summary>
        /// <param name="size"></param>
        private void GridColumnResize(int size)
        {
            int newColumn = size;
            int oldColumn = myGrid.ColumnDefinitions.Count;

            // 行番号表示分増やす
            newColumn = newColumn == 0 ? 0 : newColumn + 1;

            // 枠の数が減る
            if (newColumn < oldColumn)
            {
                myGrid.ColumnDefinitions.RemoveRange(newColumn, oldColumn - newColumn);

                for (int i = 0; i < myGrid.Children.Count; i++)
                {
                    int v = (int)myGrid.Children[i].GetValue(Grid.ColumnProperty);
                    if (v >= newColumn)
                    {
                        myGrid.Children.RemoveAt(i);
                        --i;
                    }
                }
            }

            // 枠の数を増やす
            if (newColumn > oldColumn)
            {
                for (int i = oldColumn; i < newColumn; ++i)
                {
                    ColumnDefinition cd = new ColumnDefinition();
                    if(ViewOperation.m_instance.m_bViewMode)
                        cd.Width = new GridLength(1.0f, GridUnitType.Star);
                    else
                        cd.Width = new GridLength(ToolSetting.m_instance.m_nGridSize, GridUnitType.Pixel);
                    myGrid.ColumnDefinitions.Add(cd);
                }

                // 必要な数のImageを追加
                int rowNum = myGrid.RowDefinitions.Count;
                for (int i = oldColumn; i < newColumn; ++i)
                {
                    for (int j = 0; j < rowNum; j++)
                    {   // 添付プロパティの設定
                        Image img = new Image();
                        img.SetValue(Grid.ColumnProperty, i);
                        img.SetValue(Grid.RowProperty, j);
                        myGrid.Children.Add(img);
                    }
                }
            }
        }

        /// <summary>
        /// グリッド行修正追加
        /// </summary>
        /// <param name="size"></param>
        private void GridRowResize(int size)
        {
            int newRow = size;
            int oldRow = myGrid.RowDefinitions.Count;

            // 枠の数が減る
            if (newRow < oldRow)
            {
                myGrid.RowDefinitions.RemoveRange(newRow, oldRow - newRow);

                for (int i = 0; i < myGrid.Children.Count; i++)
                {
                    int v = (int)myGrid.Children[i].GetValue(Grid.RowProperty);
                    if (v >= newRow)
                    {
                        myGrid.Children.RemoveAt(i);
                        --i;
                    }
                }
            }

            // 枠の数を増やす
            if (newRow > oldRow)
            {
                for (int i = oldRow; i < newRow; ++i)
                {
                    RowDefinition cd = new RowDefinition();
                    if(ViewOperation.m_instance.m_bViewMode)
                        cd.Height = new GridLength(1.0f, GridUnitType.Star);
                    else
                        cd.Height = new GridLength(ToolSetting.m_instance.m_nGridSize, GridUnitType.Pixel);
                    myGrid.RowDefinitions.Add(cd);
                }

                // 必要な数のImageを追加
                int columnNum = myGrid.ColumnDefinitions.Count;
                for (int i = oldRow; i < newRow; ++i)
                {
                    for (int j = 0; j < columnNum; j++)
                    {   // 添付プロパティの設定
                        if (j == 0)
                        {
                            TextBox text = new TextBox();
                            text.SetValue(Grid.ColumnProperty, j);
                            text.SetValue(Grid.RowProperty, i);
                            text.Text = "行" + i.ToString();
                            myGrid.Children.Add(text);
                            continue;
                        }
                        Image img = new Image();
                        img.SetValue(Grid.ColumnProperty, j);
                        img.SetValue(Grid.RowProperty, i);
                        myGrid.Children.Add(img);
                    }
                }


                //for (int i = size; 0 < i; --i)
                //for (int i = 0; i < size; ++i)
                //{
                //    int rowIndex1 = i + 1; // 入れ替える1つ目の行のIndex
                //    int rowIndex2 = i; // 入れ替える2つ目の行のIndex

                //    foreach (UIElement child in myGrid.Children)
                //    {
                //        int row = (int)child.GetValue(Grid.RowProperty);
                //        if (row == rowIndex1)
                //        {
                //            child.SetValue(Grid.RowProperty, rowIndex2); // 1つ目の行の要素の行を2つ目の行に変更
                //        }
                //        else if (row == rowIndex2)
                //        {
                //            child.SetValue(Grid.RowProperty, rowIndex1); // 2つ目の行の要素の行を1つ目の行に変更
                //        }

                //        TextBox text = child as TextBox;
                //        if (text == null)
                //            continue;
                //        row = (int)child.GetValue(Grid.RowProperty);
                //        text.Text = row.ToString();

                //    }
                //}
                

            }

        }

        private void CSVWrite(object sender, RoutedEventArgs e)
        {
            csvData data = new csvData(ToolSetting.m_instance.csvFile);

            // パネルの大きさ
            int width = ToolSetting.m_instance.m_nPanelW;
            int height = ToolSetting.m_instance.m_nPanelH;

            // 必要な分のリストを確保
            data.m_aData = new List<string>();
            // 一行文格納用
            var line = new List<string>();

            // 格納
            for (int i = 0; i < width * height; i++)
            {
                // グリッド上の位置
                int x = i % width;
                int y = i / width;

                // グリッドの中から該当のデータを探索
                foreach (UIElement item in myGrid.Children)
                {
                    Image img = item as Image;
                    if (img == null)
                        continue;

                    // imgeListの並び順を番号として設定
                    if (x != (int)img.GetValue(Grid.ColumnProperty) || y != (int)img.GetValue(Grid.RowProperty))
                    {
                        continue;
                    }

                    // 一致するイメージソース探索
                    int imageIdx = 0;
                    for (int j = 0; j < ImageList.Items.Count; j++)
                    {
                        if ((ImageList.Items[j] as Image).Source == img.Source)
                        {
                            // 保存し、離脱
                            imageIdx = j;
                            break;
                        }
                    }
                    // 見つかっていなければ0格納
                    line.Add(string.Concat(',', imageIdx));

                    // 行の終わりか確認
                    if ((i + 1) % width == 0)
                    {
                        string input = string.Empty;
                        foreach (var idx in line)
                        {
                            input += idx;
                        }
                        // 最初のカンマのみ削除
                        input = input.Remove(0, 1);
                        // 配列に追加
                        data.m_aData.Add(input);
                        // 次のためのクリア
                        line.Clear();
                    }

                    break;
                }
            }

            //--- ﾃﾞｰﾀの出力
            if(data.Write())
            {
                MessageBox.Show("csvを出力しました");
            }
            else
                MessageBox.Show("ファイルがないか、データがありません");
        }

        private void DialogOpen(object sender, RoutedEventArgs e)
        {
            DispDialog dd = new DispDialog();
            var path = dd.OpenDialog();
            if (path != null)
            {
                ToolSetting.m_instance.csvFile = path;
                // xamlにも反映
                CSVPathName.Text = path;
            }
        }

        private void Image_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Image image = sender as Image;
            for (int i = 0; i < ImageList.Items.Count; i++)
            {
                Image item = ImageList.Items[i] as Image;
                if (item.Source == image.Source)
                {
                    ViewOperation.m_instance.m_nSelect = i;
                    MySelectIndex.Content = i.ToString();
                    break;
                }
            }
            
        }

        private void MyGrid_MouseMove(object sender, MouseEventArgs e)
        {
            // まず、ドラッグボタンが押されているか判定
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            if (ViewOperation.m_instance.m_nSelect == -1)
                return;
            
            // senderがドロップされるGridか確認
            Grid grid = sender as Grid;
            if (grid == null)
                return;

            // ドロップされた位置を計算
            int colum = grid.ColumnDefinitions.Count;   // 方向の区切り線
            int row = grid.RowDefinitions.Count;        // 方向の区切り線
            double width = grid.RenderSize.Width;       // 横方向のサイズ
            double height = grid.RenderSize.Height;     // 縦方向のサイズ

            Point pos = e.GetPosition(grid); // gridコントロール内の位置を取得
            if (!ViewOperation.m_instance.m_bViewMode)
            {
                width = gridScroll.ViewportWidth;       // 横方向のサイズ
                height = gridScroll.ViewportHeight;     // 縦方向のサイズ
                colum = (int)gridScroll.ViewportWidth / ToolSetting.m_instance.m_nGridSize;   // 方向の区切り線
                row = (int)gridScroll.ViewportHeight / ToolSetting.m_instance.m_nGridSize;        // 方向の区切り線
            }

            // 1マス辺りのサイズを計算
            double cellWidth = width / colum;
            double cellHeight = height / row;

            // 1マス当たりのサイズから、マウスの位置がマスのどのインデックスになるか計算
            int cellX = (int)(pos.X / cellWidth);
            int cellY = (int)(pos.Y / cellHeight);

            Image image = null;
            // グリッドの該当箇所のcontrolを取得
            foreach (UIElement element in grid.Children)
            {
                int x = (int)element.GetValue(Grid.ColumnProperty);
                int y = (int)element.GetValue(Grid.RowProperty);
                if (cellX == x && cellY == y)
                {
                    image = element as Image;
                    break;
                }
            }

            //Image image = grid.Children[cellX * row + cellY] as Image;    // 2次元のインデックスを1次元に変換
            if (image == null)
                return;

            // ドロップされてきたアイテムを該当箇所に入れる
            Image dropImage = ImageList.Items[ViewOperation.m_instance.m_nSelect] as Image;
            if (dropImage == null)  // 確認
                return;

            // ドロップイメージをグリッドのイメージに適用
            image.Source = dropImage.Source;

        }

        private void Scroll_Box(object sender, RoutedEventArgs e)
        {
            MyGrid_SizeChanged(null, null);
        }

        private void CSVLoad(object sender, RoutedEventArgs e)
        {
            DispDialog dd = new DispDialog();
            var path = dd.OpenDialog();
            if (path == null)
            {
                MessageBox.Show("path error");
                return;
            }

            csvData csvData = new csvData(path);
            List<List<string>> data = csvData.Load(path);

            ToolSetting.m_instance.m_nPanelH = data.Count;
            ToolSetting.m_instance.m_nPanelW = data[0].Count();
            // jsonが読みこまれているのでサイズ調整
            GridRowResize(ToolSetting.m_instance.m_nPanelH);
            GridColumnResize(ToolSetting.m_instance.m_nPanelW);

            foreach (UIElement child in myGrid.Children)
            {
                int row = (int)child.GetValue(Grid.RowProperty);
                int column = (int)child.GetValue(Grid.ColumnProperty);
                column -= 1;

                Image image = child as Image;
                if(image != null)
                {
                    int idx = int.Parse(data[row][column].ToString());
                    Image texture = ImageList.Items[idx] as Image;
                    image.Source = texture.Source;
                }

                TextBox text = child as TextBox;
                if (text == null)
                    continue;
                row = (int)child.GetValue(Grid.RowProperty);
                text.Text = row.ToString();
            }

        }

    }

}
