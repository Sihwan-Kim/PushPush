using System;

namespace PushPush
{
    public class Constants
    {
        public const int Road = 0x00;
        public const int Store = 0x01;
        public const int Box = 0x02;
        public const int FullStore = 0x03;
        public const int Wall = 0x04;

        public const int WorkerTop = 0;
        public const int WorkerDown = 1;
        public const int WorkerLeft = 2;
        public const int WorkerRight = 3;

        public const int MapColumnCnt = 10;
        public const int MapRowCnt = 10;

        public const int IconSize = 32;
    }

    enum Direction { TOP, BOTTOM, LEFT, RIGHT };

}
