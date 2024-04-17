using Microsoft.VisualBasic;
using System.Runtime.Intrinsics.Arm;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace PushPush
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow:Window
    {
        private readonly string[] icons = {"./Resources/empty.png",
                                           "./Resources/target.png",
                                           "./Resources/box.png",
                                           "./Resources/inbox.png",
                                           "./Resources/brick.png"};

        private int StageNum = 1;
        private readonly string stageFolder;
        private GamePlay gamePlay;

        public MainWindow()
        {
            InitializeComponent();
            stageFolder = System.Environment.CurrentDirectory + "stage\\";  // 게임스테이지가 들어있는 폴더 설정 
            makePlayGround(5, 5);

            gamePlay = new GamePlay();

            loadStageNum();     // config 파일에 저장된 스테이지 번호를 불러온다. 
            gameStart();        // 게임을 시작한다. 
        }
        //----------------------------------------------------------------------------------------
        private void gameStart()
        {
            var result = gamePlay.field.loadStage(string.Format("{0}level-{1}.txt", stageFolder, StageNum));

            if(result == false)
            {
//                this.KeyPreview = false;
                MessageBox.Show("All games have been completed.", "Information", MessageBoxButton.OK, MessageBoxImage.Error); 
            }
            else
            {
                displayInitStage();
                gamePlay.Start();
//                this.KeyPreview = true;
            }
        }
        //----------------------------------------------------------------------------------------
        private void displayInitStage()
        {
            int index = 0;

            for(int y = 0 ; y < panelGameFiled.RowDefinitions.Count ; y++)
            {
                for(int x = 0 ; x < panelGameFiled.ColumnDefinitions.Count  ; x++)
                {
                    Uri resourceUri = new Uri(icons[gamePlay.field.fieldArray[x, y]], UriKind.Relative);

                    Image image = (Image)panelGameFiled.Children[index ++];
                    image.Source = new BitmapImage(resourceUri);                    
                }
            }

 //           ((PictureBox)panelGameFiled.GetControlFromPosition(gamePlay.field.worker.Position.X, gamePlay.field.worker.Position.Y)!).Image = Properties.Resources.down;

            textStage.Text = string.Format("Stage : Level-{0}", StageNum);
            textPresentStep.Text = "Steps : 0";

//            gamePlay.loadHighScore(labelLevel.Text);        // 현재 스테이지의 최고 점수를 불러와서 화면에 보여준다.
            textHighStep.Text = gamePlay.HighSteps.ToString("Steps : 0");
            textHighTime.Text = string.Format("Time : {0:D2}:{1:D2}", gamePlay.HighTimes / 60, gamePlay.HighTimes % 60);  
        }
        //----------------------------------------------------------------------------------------.
        private void loadStageNum()
        {
            Utilities.IniFile iniFile = new Utilities.IniFile("Config.ini");
            StageNum = iniFile.GetInt32("Sokoban", "Stage Number", 1);
        }
        //----------------------------------------------------------------------------------------
        private void makePlayGround(int RowCount, int ColCount)
        {
            gridDevide(RowCount, ColCount);

            for(int row = 0 ; row < RowCount ; row++)
            {
                for(int col = 0 ; col < ColCount ; col++)
                {
                    Uri resourceUri = new Uri("./Resources/Box.png", UriKind.Relative);

                    Image image = new Image()  { Source = new BitmapImage(resourceUri)} ;
                    Grid.SetColumn(image, col);
                    Grid.SetRow(image, row);

                    panelGameFiled.Children.Add(image);
                }
            }
        }
        //----------------------------------------------------------------------------------------
        private void gridDevide(int RowCount, int ColCount)
        {
            borderGround.Width = this.Width - 210;

            panelGameFiled.Width = Constants.IconSize * ColCount;
            panelGameFiled.Height = Constants.IconSize * RowCount;

            for(int loop=0 ; loop < RowCount ; loop++)
            {
                panelGameFiled.RowDefinitions.Add(new RowDefinition());
            }

            for(int loop=0 ; loop < ColCount ; loop++)
            {
                panelGameFiled.ColumnDefinitions.Add(new ColumnDefinition());
            }
        }
        //----------------------------------------------------------------------------------------
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            borderGround.Width = this.Width - 210;            
        }
        //----------------------------------------------------------------------------------------
    }
}