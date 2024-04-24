using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel ;

namespace PushPush.Model
{
    public class GameInformation : ObservableObject
    {
        private string? stageNumber;        // 현재 진행중인 Stage 번호
        private int presentSteps;           // 현재 진행중인 게임의 이동 횟수 
        private int presentTime;            // 현재 진행중인 게임의 진행 시간 
        private int highSteps;              // 현재 진행중인 stage의 최고기록 이동 횟수     
        private int highTime;               // 현재 진행중인 stage의 최고기록 시간     

        public int HighTime
        {
            get => highTime; 
            set => SetProperty(ref highTime, value); 
        }

        public int HighSteps
        {
            get => highSteps;
            set => SetProperty(ref highSteps, value);
        }

        public int PresentTime
        {
            get => presentTime; 
            set => SetProperty(ref presentTime, value);
        }

        public int PresentSteps
        {
            get => presentSteps; 
            set => SetProperty(ref presentSteps, value);
        }

        public string? StageNumber
        {
            get => stageNumber; 
            set => SetProperty(ref stageNumber, value);
        }
    }
}
