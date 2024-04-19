using System;
using System.Reflection.Metadata.Ecma335;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.ComponentModel;

namespace PushPush
{
    internal class Field
    {
        public int[,] fieldArray = new int[10, 10];
        public Worker worker = new Worker(new Point(0,0));
        //----------------------------------------------------------------------------------------
        public Field()
        {

        }
        //----------------------------------------------------------------------------------------
        public bool loadStage(string StageFile, out int ColCount, out int RowCount)
        {
            bool reuslt = true;
            ColCount = 0; RowCount = 0;

            try
            {
                using(StreamReader sr = new StreamReader(StageFile))
                {
                    string[] count = sr.ReadLine()!.Split(',');
                    string[] position = sr.ReadLine()!.Split(',');

                    worker.Position.X = int.Parse(position[0]);   // 작업자의 X 위치
                    worker.Position.Y = int.Parse(position[1]);   // 작업자의 Y 위치

                    ColCount = int.Parse(count[0]);
                    RowCount = int.Parse(count[1]);

                    fieldArray = (int[,])ResizeArray(fieldArray, new int[] {ColCount, RowCount}) ;

                    int index = 0;

                    while(!sr.EndOfStream)
                    {
                        string? line = sr.ReadLine();

                        if(line is not null)
                        {
                            string[] data = line.Split(',');
                            int x = 0;

                            foreach(var value in data)
                            {
                                fieldArray[x++, index] = int.Parse(value);
                            }

                            index++;
                        }
                    }
                }
            }
            catch
            {
                reuslt = false;
            }

            return reuslt;
        }
        //----------------------------------------------------------------------------------------
        private static Array ResizeArray(Array arr, int[] newSizes)
        {
            if(newSizes.Length != arr.Rank)
            {
                throw new ArgumentException("arr must have the same number of dimensions " +
                                            "as there are elements in newSizes", "newSizes");
            }

            var temp = Array.CreateInstance(arr.GetType().GetElementType()!, newSizes);
            int length = arr.Length <= temp.Length ? arr.Length : temp.Length;
            Array.ConstrainedCopy(arr, 0, temp, 0, length);

            return temp;
        }
    }
    /*********************************************************************************************/
    internal class Worker
    {
        public Point Position = new Point(0,0);
        public Direction MoveDirection { set; get; }

        public Worker(Point position) { this.Position = position; }
    }
    /*********************************************************************************************/
    internal class UndoInform
    {
        public Direction direction;
        public bool pushInform;

        public UndoInform(Direction direction, bool pushInform)
        {
            this.direction = direction;
            this.pushInform = pushInform;
        }
    }
    /*********************************************************************************************/
}
