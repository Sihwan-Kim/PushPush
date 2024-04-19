using Microsoft.VisualBasic;
using System;
using System.Runtime.Intrinsics.Arm;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Input;
using System.Windows.Media;
using System.Reflection;
using PushPush.Model;

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

        private readonly string[] worker = {"./Resources/back.png",
                                            "./Resources/front.png",
                                            "./Resources/left.png",
                                            "./Resources/right.png"};

        private int StageNum = 1;
        private readonly string stageFolder;
        private GamePlay gamePlay;
        private GameInformation gameInformation;

        public MainWindow()
        {
            InitializeComponent();
            stageFolder = System.Environment.CurrentDirectory + "\\stage\\";  // 게임스테이지가 들어있는 폴더 설정 

            gameInformation = new GameInformation();

            gamePlay = new GamePlay();
            gamePlay.ReturnToTime += new GamePlay.UpdateTimeInform(UpdateTime);

            loadStageNum();     // config 파일에 저장된 스테이지 번호를 불러온다. 
            gameStart();        // 게임을 시작한다. 
        }
        //----------------------------------------------------------------------------------------
        private void UpdateTime(string TimeInform)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { textPresentTime.Text = TimeInform; }));           
        }
        //----------------------------------------------------------------------------------------
        private void gameStart()
        {
            var result = gamePlay.field.loadStage(string.Format("{0}level-{1}.txt", stageFolder, StageNum), out int colCount, out int rowCount);

            if(result == false)
            {
                MessageBox.Show("All games have been completed.", "Information", MessageBoxButton.OK, MessageBoxImage.Error); 
            }
            else
            {
                displayInitStage(colCount, rowCount);
                gamePlay.Start();
            }
        }
        //----------------------------------------------------------------------------------------
        private void displayInitStage(int colCount, int rowCount)
        {
            int index = 0;

            makePlayGround(colCount, rowCount);

            for(int y = 0 ; y < panelGameFiled.RowDefinitions.Count ; y++)
            {
                for(int x = 0 ; x < panelGameFiled.ColumnDefinitions.Count  ; x++)
                {
                    Uri resourceUri = new Uri(icons[gamePlay.field.fieldArray[x, y]], UriKind.Relative);
                    ((Image)panelGameFiled.Children[index ++]).Source = new BitmapImage(resourceUri);                    
                }
            }

            Uri workerUri = new Uri(worker[1], UriKind.Relative);
            index = (gamePlay.field.worker.Position.Y * panelGameFiled.ColumnDefinitions.Count) + gamePlay.field.worker.Position.X;
            ((Image)panelGameFiled.Children[index]).Source = new BitmapImage(workerUri);  

            //textStage.Text = string.Format("Stage : Level-{0}", StageNum);
            gameInformation.StageNumber = string.Format("Stage : Level-{0}", StageNum); 
            textPresentStep.Text = "Steps : 0";

            gamePlay.loadHighScore(gameInformation.StageNumber);        // 현재 스테이지의 최고 점수를 불러와서 화면에 보여준다.
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
        private void InitPlayGround()
        {
            panelGameFiled.Children.Clear();
            panelGameFiled.RowDefinitions.Clear();
            panelGameFiled.ColumnDefinitions.Clear();
        }
        //----------------------------------------------------------------------------------------
        private void makePlayGround(int RowCount, int ColCount)
        {
            InitPlayGround();

            gridDevide(RowCount, ColCount);   // 열과 행 갯수로 그리드를 분할한다. 

            for(int row = 0 ; row < RowCount ; row++)
            {
                for(int col = 0 ; col < ColCount ; col++)
                {
                    Uri resourceUri = new Uri("./Resources/empty.png", UriKind.Relative);

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
        private MessageBoxResult WorkerMove(Direction direction)
        {
            MessageBoxResult result = MessageBoxResult.No;
            System.Drawing.Point rootPosition = gamePlay.field.worker.Position;  // worker가 처음 있던 위치 저장 

            gamePlay.moveWorker(direction);
            FieldUpdate(rootPosition, false);  // 화면을 갱신한다. 

            if(gamePlay.CheckStageClear())  // 게임이 완료 되었다.
            {
                gamePlay.Stop();
                gamePlay.saveHighScore(gameInformation.StageNumber);
                result = MessageBox.Show("This Stage Cleared, Play the next stage?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if(result == MessageBoxResult.Yes) StageNum++;  // 다음 스테이지로 넘어간다.                 
            }

            return result;
        }
        //----------------------------------------------------------------------------------------
        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            MessageBoxResult result = MessageBoxResult.No ;

            switch(e.Key)
            {
                case Key.Up: result = WorkerMove(Direction.TOP); break;
                case Key.Down: result = WorkerMove(Direction.BOTTOM); break;
                case Key.Left: result = WorkerMove(Direction.LEFT); break;
                case Key.Right: result = WorkerMove(Direction.RIGHT); break;
                case Key.U:
                    gamePlay.Undo();
                    FieldUpdate(gamePlay.field.worker.Position, true);
                    break;
                case Key.R: gameStart(); break;
            }


            if(result == MessageBoxResult.Yes) gameStart();  // 다음 스테이지의 게임 계속 진행 
        }
        //----------------------------------------------------------------------------------------
        private void FieldUpdate(System.Drawing.Point RootPosition, bool Undo)
        {
            /*********************************************************
             * 일반적인 이동에서는 최대 3개 픽셀에 변화가 생긴다.
             * Undo 상황에서는 최대 2개 픽셀에 변화가 생긴다. 
             *********************************************************/
            System.Drawing.Point pos1 = gamePlay.getChangePosition(0);
            System.Drawing.Point pos2 = gamePlay.getChangePosition(1);

            int item1 = gamePlay.field.fieldArray[pos1.X, pos1.Y];
            int item2 = gamePlay.field.fieldArray[pos2.X, pos2.Y];

            changeIcon(pos1, icons[item1]);
            changeIcon(pos2, icons[item2]);

            if(!Undo)
            {
                int item3 = gamePlay.field.fieldArray[RootPosition.X, RootPosition.Y];
                changeIcon(RootPosition, icons[item3]);
            }

            changeIcon(gamePlay.field.worker.Position,  worker[(int)gamePlay.field.worker.MoveDirection]);

            textPresentStep.Text = gamePlay.Steps.ToString("Steps : 0");
        }
        //----------------------------------------------------------------------------------------
        private void changeIcon(System.Drawing.Point Position, string IconName)
        {
            Uri workerUri = new Uri(IconName, UriKind.Relative);
            var index = (Position.Y * panelGameFiled.ColumnDefinitions.Count) + Position.X;

            ((Image)panelGameFiled.Children[index]).Source = new BitmapImage(workerUri);  
        }
        //----------------------------------------------------------------------------------------
        private void btnUndo_Click(object sender, RoutedEventArgs e)
        {
            gamePlay.Undo();
            FieldUpdate(gamePlay.field.worker.Position, true);
        }
        //----------------------------------------------------------------------------------------
        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            StageNum = 1;
            gameStart();
        }
        //----------------------------------------------------------------------------------------
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            gamePlay.Stop();
            saveStageNum();
        }
        //----------------------------------------------------------------------------------------
        public void saveStageNum()
        {
            Utilities.IniFile iniFile = new Utilities.IniFile("Config.ini");

            iniFile.WriteValue("Sokoban", "Stage Number", StageNum);
        }
        //----------------------------------------------------------------------------------------
    }
}