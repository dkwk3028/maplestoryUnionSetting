using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace MapleStroyUnionSetProject {

    public static class SingletonUnionOutputArray {

        private const int btnSize = 440;
        public static int[] btnArray = new int[btnSize];
        public static int count = 0;

        public static void Clear() {
            count = 0;
            btnArray = new int[btnSize];
        }
    }
    public static class SingletonButtonArray {

        private const int btnSize = 440;
        public static bool[] btnArray = new bool[btnSize];
        public static int count = btnSize;

        static SingletonButtonArray() {
            for (int i=0; i<btnSize; ++i) {
                btnArray[i] = true;
            }
        }
        public static void printBtnArray() {
            for(int i=0; i<btnSize; ++i) {
                if (i > 0 && i % 22 == 0) {
                    Console.Write("\n");
                }
                Console.Write("{0}", btnArray[i] ? 1 : 0);
            }
            Console.Write("\n");
        }

        public static void Clear() {
            count = 0;
            btnArray = new bool[btnSize];
        }
    }

    public static class SingletonBlockArray {

        private const int arrSize = 15;
        public static int[] blockArray = new int[arrSize];
        public static int count = 0;
        public static int realCount = 0;
    }
    

    struct CalcResult {
        public bool suc;
        public bool error;

        public void Clear() {
            suc = error = false;
        }
    }
    class UnionBlock {
        public int width, height;
        public int[,] shape;
        public int count, startIndex;

        public UnionBlock(int _w, int _h, int[,] _shape) {
            width = _w;
            height = _h;

            shape = new int[height,width];
            for(int i=0; i<_h; ++i) {
                for (int j=0; j<_w; ++j) {
                    shape[i, j] = _shape[i, j];
                }
            }
            count = startIndex = 0;
        }
    }
    
    struct Point {
        public int r, c;
        public int cnt;
        public Point(int _r, int _c, int _cnt) {
            c = _c;
            r = _r;
            cnt = _cnt;
        }
    }
    public class UnionCalculator {

        private static int mycnt = 0;
        private const int stepSize = 22;
        private const int stepHeight = 20;
        private const int btnArraySize = 440;
        private const int blockArraySize = 15;
        private List<List<List<UnionBlock>>> unionBlockList;
        private Point[] accessPoints;
        private List<Point> loopPoints;
        private int[,] LabelMap;
        private int[,] CountMap;
        private static int calcCount = 0;
        private CalcResult calcResult;
 
        public UnionCalculator() {

            loopPoints = new List<Point>();
            CountMap = new int[5, 5];
            UnionBlock[,] unionBlocks;
            unionBlocks = new UnionBlock[5, 5];
            
            
            unionBlocks[0,0] = new UnionBlock(1, 1, new int[1, 1] { { 1 } });
            unionBlocks[1,0] = new UnionBlock(2, 1, new int[1, 2] { { 1,1 }});
            unionBlocks[2,0] = new UnionBlock(3, 1, new int[1, 3] { { 1,1,1 }});
            unionBlocks[2,1] = new UnionBlock(2, 2, new int[2, 2] { { 1,1 },{ 1,0 } });

          
            unionBlocks[3,0] = new UnionBlock(2, 2, new int[2, 2] { { 1,1},{ 1, 1 } });
            unionBlocks[3,1] = new UnionBlock(3, 2, new int[2, 3] { { 1, 1, 1 }, { 0, 1, 0 } });
            unionBlocks[3,2] = new UnionBlock(4, 1, new int[1, 4] { { 1, 1, 1, 1 } });
            unionBlocks[3,3] = new UnionBlock(3, 2, new int[2, 3] { { 1, 1, 1 }, { 0, 0, 1 } });
            unionBlocks[3,4] = new UnionBlock(2, 3, new int[3, 2] { { 0,1},{ 1, 1 },{ 1, 0 } });

            unionBlocks[4,0] = new UnionBlock(3, 2, new int[2, 3] { { 1, 1, 1 }, { 1, 1, 0 } });
            unionBlocks[4,1] = new UnionBlock(3, 3, new int[3, 3] { { 0,1,0},{ 1, 1,1 },{ 0, 1,0 } });
            unionBlocks[4,2] = new UnionBlock(5, 1, new int[1, 5] { { 1, 1, 1, 1, 1 } });
            unionBlocks[4,3] = new UnionBlock(3, 3, new int[3, 3] { { 0,0,1},{ 1,1, 1 },{ 0,0,1 } });
            unionBlocks[4,4] = new UnionBlock(2, 4, new int[4, 2] { { 0,1},{ 0, 1 },{ 1, 1 },{ 1, 0 } });

            accessPoints = new Point[blockArraySize - 1];
            accessPoints[0] = new Point(0, 0, 1);
            accessPoints[1] = new Point(1, 0, 2);
            accessPoints[2] = new Point(2, 0, 2); //...
            accessPoints[3] = new Point(2, 1, 4);

            for(int i=4; i<blockArraySize-1; ++i) {
                if (i < 9) {
                    int cnt = 0;
                    if (i == 4) {
                        cnt = 1;
                    }else if (i == 6 || i == 8) {
                        cnt = 2;
                    } else {
                        cnt = 4;
                    }
                    accessPoints[i] = new Point(3, i - 4, cnt);
                } else {
                    int cnt = 0;
                    if (i % 9 == 1) {
                        cnt = 1;
                    }else if (i % 9 == 2) {
                        cnt = 2;
                    } else {
                        cnt = 4;
                    }
                    accessPoints[i] = new Point(4, i % 9, cnt);
                }
            }

            unionBlockList = new List<List<List<UnionBlock>>>();
            for(int i=0; i<5; ++i) {
                unionBlockList.Add(new List<List<UnionBlock>>());
            }
            foreach (Point pos in accessPoints) {
                unionBlockList[pos.r].Add(new List<UnionBlock>());
                unionBlockList[pos.r][pos.c].Add(unionBlocks[pos.r, pos.c]);
                Console.WriteLine("{0}, {1}", pos.r, pos.c);
                for (int i=1; i<pos.cnt; ++i) {
                    UnionBlock addBlock;
                    UnionBlock subjectBlock = unionBlockList[pos.r][pos.c][i - 1];
                    if (pos.cnt == 2) {
                        subjectBlock = GetRotateClockwiseUnionBlock(subjectBlock);
                        addBlock = subjectBlock;
                        i = pos.cnt;
                    } else {
                        addBlock = GetRotateClockwiseUnionBlock(subjectBlock);
                    }

                    unionBlockList[pos.r][pos.c].Add(addBlock);
                }
            }

            CreateLabelMap();
            calcResult = new CalcResult();
        }

        private void CreateLabelMap() {
            LabelMap = new int[5, 5];
            int label = 2;
            foreach (var pos in accessPoints) {
                LabelMap[pos.r, pos.c] = label++;
            }
        }
        private UnionBlock GetRotateClockwiseUnionBlock(UnionBlock unionBlock) {
            
            int new_w = unionBlock.height, new_h = unionBlock.width;
            int[,] new_shape = new int[new_h, new_w];

            //변환 공식 : (c,r)로 주어졌을때, (-r,c) 가 된다. 연산 후 원래사분면으로 옮겨준다.

            int x_min = 0;

            for(int i=0; i < unionBlock.height; ++i) {
                for(int j=0; j < unionBlock.width; ++j) {
                    if (-i < x_min) {
                        x_min = -i;
                    }
                }
            }

            int transfromX = x_min < 0 ? -x_min : 0;
            for (int i = 0; i < unionBlock.height; ++i) {
                for (int j = 0; j < unionBlock.width; ++j) {
                    new_shape[j, -i + transfromX] = unionBlock.shape[i, j];
                }
            }

            UnionBlock unionBlock1 = new UnionBlock(new_w, new_h, new_shape);

            return unionBlock1;
            
        }
        public void CalcUnion() {

            CountMap = new int[5, 5];

            int[] btnArray = new int[btnArraySize];
            int[] blockArray = new int[blockArraySize];
            for (int i=0; i<btnArraySize; ++i) {
                btnArray[i] = SingletonButtonArray.btnArray[i] ? 1 : 0;
                if (SingletonButtonArray.btnArray[i]) {
                    loopPoints.Add(new Point(i / 22, i % 22, 0));
                }
            }
            for (int i=0; i<blockArraySize; ++i) {
                blockArray[i] = SingletonBlockArray.blockArray[i];
                switch (GetBlockCountByIndex(i)) {
                    case 1:
                        unionBlockList[0][0][0].count = blockArray[i];
                        unionBlockList[0][0][0].startIndex = 0;
                        
                        break;
                    case 2:
                        unionBlockList[1][0][0].count = blockArray[i];
                        unionBlockList[1][0][0].startIndex = 0;
                        break;
                    case 3:
                        unionBlockList[2][i == 3 ? 0 : 1][0].count = blockArray[i];
                        unionBlockList[2][i == 3 ? 0 : 1][0].startIndex = 0;
                        break;
                    case 4:
                        unionBlockList[3][i % 5][0].count = blockArray[i];
                        if (i % 5 == 4) {
                            unionBlockList[3][i % 5][0].startIndex = 1;
                        } else {
                            unionBlockList[3][i % 5][0].startIndex = 0;
                        }
                        break;
                    case 5:
                        unionBlockList[4][i % 5][0].count = blockArray[i];

                        if (i % 5 == 0 || i % 5 == 2) {
                            unionBlockList[4][i % 5][0].startIndex = 0;
                        }else if (i % 5 == 1 || i % 5 == 4) {
                            unionBlockList[4][i % 5][0].startIndex = 1;
                        } else {
                            unionBlockList[4][i % 5][0].startIndex = 2;
                        }
                        break;
                }

            }
            for(int i=0; i<5; ++i) {
                for(int j=0; j<unionBlockList[i].Count; ++j) {
                    for(int k=0; k<unionBlockList[i][j].Count; ++k) {
                        
                        int start = 0;
                        for(int y=0; y<1; ++y) {
                            for(int x=0; x< unionBlockList[i][j][k].width; ++x) {
                                if (unionBlockList[i][j][k].shape[y,x] == 1) {
                                    start = x;
                                    break;
                                }
                            }
                        }
                        unionBlockList[i][j][k].startIndex = start;
                    }
                }
            }
            calcCount = 0;
            calcResult.Clear();
            RecursiveCalc(0,0, SingletonButtonArray.count, SingletonBlockArray.realCount, ref btnArray);
            if (calcResult.error) {
                MessageBox.Show("너무 복잡해서 찾지 못했습니다! (5000만번 이상 연산 실행)");
                return;
            }
            if (calcResult.suc) {
                MessageBox.Show("찾았습니다. 출력보기를 눌러보세요");
            } else {
                MessageBox.Show("찾지 못했습니다.");
            }
            //MessageBox.Show(calcCount.ToString());

        }


        public void SetToNumberBtnArray(int now, int num,int posLoop,int r, int c, ref int[] btnArray) {
            //
          //  Console.Write("");
            if (num == 1) {
                CountMap[r, c] -= 1;
            } else {
                CountMap[r, c] += 1;
            }
            for (int j = 0; j < unionBlockList[r][c][posLoop].height; ++j) {
                for (int k = 0; k < unionBlockList[r][c][posLoop].width; ++k) {
                    if (unionBlockList[r][c][posLoop].shape[j, k] == 1) {

                        btnArray[now + j * stepSize + (k - unionBlockList[r][c][posLoop].startIndex)] = CalcToBitBtnArray(num, CountMap[r,c]);
                    }
                }
            }
         //   Console.Write("");

        }

        public static int CalcToBitBtnArray(int value, int count) {
            // 23bit ~ 31bit는 개수를 표현, 1bit ~ 4bit는 Label을 표현하겠음.
            int n = value != 1 ? (count) << 22 : 0;
            n += value;
            return n;
        }

        public static int GetCountBit(int num) {
            // ‭2143289344‬ is 0111 1111 1100 0000 0000 0000 0000 0000
            int bit = 2143289344;
            return (num & bit) >> 22;
        }

        public static int GetLabelBit(int num) {
            return num & 15;
        }
        private void RecursiveCalc(int depth, int nowIndex, int remainOccup, int remainBlock, ref int[] btnArray) {
            calcCount++;
            if (calcCount > 50000000) {
                calcResult.error = true;
                return;
            }
            if (calcResult.error) {
                return;
            }
            //Console.WriteLine("now is {0}", now);
            if (remainOccup == 0 || remainBlock == 0) {
                if (remainBlock == 0) {
                    Console.WriteLine("찾았다 !!!!!!");
                    for (int i = 0; i < btnArraySize; ++i) {
                        SingletonUnionOutputArray.btnArray[i] = btnArray[i];
                    }
                    calcResult.suc = true;
                    return;
                    //return true;
                } else {
                    calcResult.suc = false;
                    return;
                    //return false;
                }
            }
            if (remainOccup < SingletonBlockArray.count) {
                //Console.WriteLine("없음!!!");
                calcResult.suc = false;
                return;
                //return false;
            }
            //Console.WriteLine("{0}", depth);
            int btnPos = 0;
            for (int loop = nowIndex; loop < loopPoints.Count; ++loop) {
                int y = loopPoints[loop].r, x = loopPoints[loop].c;
                btnPos = y * stepSize + x;
                if (btnArray[btnPos] == 1) {
                    for (int i = 13; i >= 0; --i) {
                        if (unionBlockList[accessPoints[i].r][accessPoints[i].c][0].count != 0) {
                            //Console.WriteLine("입성 now is {0}", i);
                            for (int posLoop = 0; posLoop < accessPoints[i].cnt; ++posLoop) {
                                bool b_success = true;
                                //한계점설정
                                if ((x - unionBlockList[accessPoints[i].r][accessPoints[i].c][posLoop].startIndex < 0) ||
                                    (x - unionBlockList[accessPoints[i].r][accessPoints[i].c][posLoop].startIndex) + unionBlockList[accessPoints[i].r][accessPoints[i].c][posLoop].width > stepSize ||
                                    y + unionBlockList[accessPoints[i].r][accessPoints[i].c][posLoop].height > stepHeight
                                    ) {
                                    continue;
                                }
                                for (int j = 0; j < unionBlockList[accessPoints[i].r][accessPoints[i].c][posLoop].height; ++j) {
                                    for (int k = 0; k < unionBlockList[accessPoints[i].r][accessPoints[i].c][posLoop].width; ++k) {
                                        /*Console.WriteLine("{0}, {1} and value is {2}, {3} and loop is {4}", (x + k), (y + j),
                                            unionBlockList[accessPoints[i].r][accessPoints[i].c][posLoop].shape[j, k], btnArray[btnPos + j * stepSize + (k - unionBlockList[accessPoints[i].r][accessPoints[i].c][posLoop].startIndex)], posLoop);
                                        //*/
                                        if (unionBlockList[accessPoints[i].r][accessPoints[i].c][posLoop].shape[j, k] == 1 && btnArray[btnPos + j * stepSize + (k - unionBlockList[accessPoints[i].r][accessPoints[i].c][posLoop].startIndex)] != 1) {
                                            b_success = false;
                                            j = unionBlockList[accessPoints[i].r][accessPoints[i].c][posLoop].height;
                                            break;
                                        }
                                    }
                                }

                                if (b_success) {
                                    
                                    SetToNumberBtnArray(btnPos, LabelMap[accessPoints[i].r, accessPoints[i].c] , posLoop, accessPoints[i].r, accessPoints[i].c, ref btnArray);
                                    //Console.WriteLine(GetLabelBit(btnArray[btnPos]));
                                    unionBlockList[accessPoints[i].r][accessPoints[i].c][0].count -= 1;
                                    SingletonBlockArray.count -= accessPoints[i].r + 1;
                                    //Console.WriteLine("{0} is ", SingletonBlockArray.Instance.GetCount());

                                    RecursiveCalc(depth + 1,++nowIndex, remainOccup - (accessPoints[i].r + 1), remainBlock - 1, ref btnArray);
                                    SetToNumberBtnArray(btnPos, 1, posLoop, accessPoints[i].r, accessPoints[i].c, ref btnArray);
                                    unionBlockList[accessPoints[i].r][accessPoints[i].c][0].count += 1;
                                    SingletonBlockArray.count += accessPoints[i].r + 1;
                                    if (calcResult.error) {
                                        return;
                                    }

                                    if (calcResult.suc) {
                                        return;
                                    } else {
                                        break;
                                    }
                                }

                            }
                        }
                    }
                    --remainOccup;
                }
            }
            calcResult.suc = false;
            return;
        }

        private int GetBlockCountByIndex(int index) {
            int result = 0;

            if (index < 5) {
                switch (index) {
                    case 0:
                        result = 1;
                        break;
                    case 2:
                        result = 2;
                        break;
                    case 3:
                        result = 3;
                        break;
                    case 4:
                        result = 3;
                        break;
                    default:
                        result = 0;
                        break;
                }
            }else if (index < 10) {
                result = 4;
            } else {
                result = 5;
            }

            return result;
        }

        
    }

}
