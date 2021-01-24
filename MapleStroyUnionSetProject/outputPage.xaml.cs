using System;
using System.Collections.Generic;
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

namespace MapleStroyUnionSetProject {
    /// <summary>
    /// outputPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class outputPage : Page {
        public outputPage() {
            InitializeComponent();
            ChangeOuptutGrid();
        }

        public static class OutputUIData {
            public static SolidColorBrush[] brushes = { new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)), new SolidColorBrush(Color.FromArgb(255, 122, 230, 146))};
            public static Color[] ColorMap = {
                Color.FromArgb(255,255,127,39),
                Color.FromArgb(255,34,177,76),
                Color.FromArgb(255,0,162,232),
                Color.FromArgb(255,255,0,0),

                Color.FromArgb(255,185,122,87),
                Color.FromArgb(255,255,174,201),
                Color.FromArgb(255,163,73,164),
                Color.FromArgb(255,0,128,128),
                Color.FromArgb(255,128,128,64),

                Color.FromArgb(255,128,128,128),
                Color.FromArgb(255,0,0,255),
                Color.FromArgb(255,255,255,0),
                Color.FromArgb(255,128,255,255),
                Color.FromArgb(255,200,191,231)
            };
        }
        public void ChangeOuptutGrid() {
            const int width = 22, height = 20;
            for (int i = 0; i < width; ++i) {
                ColumnDefinition gridCol = new ColumnDefinition();
                gridCol.Width = new GridLength(20);
                outputGrid.ColumnDefinitions.Add(gridCol);
            }

            for (int i = 0; i < height; ++i) {
                RowDefinition gridRow = new RowDefinition();
                gridRow.Height = new GridLength(20);
                outputGrid.RowDefinitions.Add(gridRow);
            }

            for (int i = 0; i < width * height; ++i) {
                Canvas c = new Canvas();
                c.Width = c.Height = 20;

                Rectangle r = new Rectangle();
                //r.Fill = Brushes.White;
                r.Stroke = new SolidColorBrush(Color.FromArgb(255, 236, 236, 236));
                r.StrokeDashArray = new DoubleCollection() { 3.0 };
                r.StrokeThickness = 1;
                r.Width = r.Height = 20;

                c.Children.Add(r);



                int count = UnionCalculator.GetCountBit(SingletonUnionOutputArray.btnArray[i]);
                int label = UnionCalculator.GetLabelBit(SingletonUnionOutputArray.btnArray[i]);
                Button btn = new Button();
                //Content = btn;
                btn.Width = btn.Height = 20;
                btn.Name = "button" + i.ToString();
                btn.BorderBrush = Brushes.Black;
                btn.BorderThickness = GetThicknessPoint(i);
                btn.Background = Brushes.White;
                btn.Background = label >= 2 ? new SolidColorBrush(OutputUIData.ColorMap[label - 2]) : OutputUIData.brushes[label]; // label은 2 ~ 15임
                //MessageBox.Show(btn.Name);
                btn.Content = c;
                Grid.SetRow(btn, i / 22);
                Grid.SetColumn(btn, i % 22);
                outputGrid.Children.Add(btn);

                //SingletonButtonArray.btnArray[i] = false;

            }
        }

        Thickness GetThicknessPoint(int point) {
            int left = 0, right = 0, top = 0, bottom = 0;
            const int thick = 1;

            // y=x 계단
            if (point % 23 == 1) {
                if (point <= 1 + 23 * 9) {
                    bottom = left = thick;
                    if (point % 22 == 10 || point % 22 == 11) {
                        bottom = 0;
                    }
                } else {
                    top = right = thick;
                    if (point % 22 == 10 || point % 22 == 11) {
                        top = 0;
                    }
                }


            }

            //y=-x 계단
            if (point % 21 == 20) {
                if (point <= 20 + 21 * 9) {
                    bottom = right = thick;
                    if (point % 22 == 10 || point % 22 == 11) {
                        bottom = 0;
                    }
                } else {
                    top = left = thick;
                    if (point % 22 == 10 || point % 22 == 11) {
                        top = 0;
                    }
                }
            }

            if (point % 22 == 10) {
                right = thick;
            }

            if (point / 22 == 9) {
                bottom = thick;
            }


            if (point % 22 == 5 &&
                point / 22 >= 5 && point / 22 <= 14) {
                left = thick;
            }

            if (point % 22 == 16 &&
                point / 22 >= 5 && point / 22 <= 14) {
                right = thick;
            }

            if (point % 22 > 5 && point % 22 < 16) {
                if (point / 22 == 4) {
                    bottom = thick;
                } else if (point / 22 == 15) {
                    top = thick;
                }
            }


            return new Thickness(left, top, right, bottom);




        }
    }


}
