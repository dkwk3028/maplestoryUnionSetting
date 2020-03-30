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
    /// mainPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class mainPage : Page {
        public mainPage() {
            InitializeComponent();
            init();
        }

        private void init() {
            InitUnionGridUI();
            InitBlockEvent();
        }

        private void InitBlockEvent() {
            int num = 1;

            while (num <= 5) {
                for (int i = 1; i <= 5; ++i) {
                    string name = "block" + num.ToString() + "_";
                    if (num < 3) {
                        name += "1";
                        ++num;
                        if (num == 2) {
                            ++i;
                        }
                    } else if (num == 3) {
                        name += (i - 3).ToString();
                    } else {
                        name += (i).ToString();
                    }

                    var textBox = this.FindName(name) as TextBox;
                    textBox.TextChanged += BlockTextBox_Changed;
                }
                ++num;
            }

        }

        private void InitUnionGridUI() {
            //가로 22 세로 20
            const int width = 22, height = 20;
            for (int i = 0; i < width; ++i) {
                ColumnDefinition gridCol = new ColumnDefinition();
                gridCol.Width = new GridLength(20);
                unionGrid.ColumnDefinitions.Add(gridCol);
            }

            for (int i = 0; i < height; ++i) {
                RowDefinition gridRow = new RowDefinition();
                gridRow.Height = new GridLength(20);
                unionGrid.RowDefinitions.Add(gridRow);
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



                Button btn = new Button();
                //Content = btn;
                btn.Width = btn.Height = 20;
                btn.Name = "button" + i.ToString();
                btn.BorderBrush = Brushes.Black;

                btn.BorderThickness = GetThicknessPoint(i);
                btn.Background = Brushes.White;
                //btn.Background = new SolidColorBrush(Color.FromArgb(255, 122, 230, 146));
                //MessageBox.Show(btn.Name);
                btn.Content = c;
                btn.Click += Button_Toggle_Click;
                Grid.SetRow(btn, i / 22);
                Grid.SetColumn(btn, i % 22);
                unionGrid.Children.Add(btn);

                SingletonButtonArray.btnArray[i] = false;

            }
        }

        private void BlockTextBox_Changed(object sender, RoutedEventArgs e) {
            var textBox = sender as TextBox;
            int value = 0;

            var name = textBox.Name;
            int firstNum = Int32.Parse(name.Substring(5, 1));
            int secondNum = Int32.Parse(name.Substring(7, 1));

            int index = 0;
            if (firstNum <= 3) {
                switch (firstNum) {
                    case 2:
                        index += 2;
                        break;
                    case 3:
                        index += 2 + secondNum;
                        break;
                }
            } else {
                index += 5 * (firstNum - 3) + (secondNum - 1);
            }

            //예외처리
            if (!(Int32.TryParse(textBox.Text, out value))) {
                MessageBox.Show("숫자만 입력해주세요 !");
                textBox.Text = "0";
                SingletonBlockArray.blockArray[index] = 0;
                return;
            }

            SingletonBlockArray.blockArray[index] = value;

            int count = 0;
            int rCount = 0;
            for (int i = 0; i < 15; ++i) {
                int multiplyNum = 1;
                if (i < 5) {
                    if (i == 2) {
                        multiplyNum = 2;
                    } else if (i == 3 || i == 4) {
                        multiplyNum = 3;
                    }
                } else {
                    multiplyNum = i < 10 ? 4 : 5;
                }
                count += SingletonBlockArray.blockArray[i] * multiplyNum;
                rCount += SingletonBlockArray.blockArray[i];

            }
            SingletonBlockArray.count = count;
            SingletonBlockArray.realCount = rCount;
            holdCountLabel.Content = "보유칸 : " + count.ToString();
        }
        private void Button_Toggle_Click(object sender, RoutedEventArgs e) {
            Button btn = (Button)sender;
            int index = Int32.Parse(btn.Name.Substring(6));

            bool b_green = SingletonButtonArray.btnArray[index];

            if (!b_green) {
                btn.Background = new SolidColorBrush(Color.FromArgb(255, 122, 230, 146));
                SingletonButtonArray.btnArray[index] = !b_green;
                SingletonButtonArray.count += 1;
                UpdateCountLable();
            } else {
                btn.Background = Brushes.White;
                SingletonButtonArray.btnArray[index] = !b_green;
                SingletonButtonArray.count -= 1;
                UpdateCountLable();
            }
        }

        private void UpdateCountLable() {
            occupCountLabel.Content = "점령칸 : " + SingletonButtonArray.count.ToString();
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

        private void calcButton_Click(object sender, RoutedEventArgs e) {
            UnionCalculator unionCalculator = new UnionCalculator();


            int occupCount = SingletonButtonArray.count;
            int holdCount = SingletonBlockArray.count;
            if (occupCount < holdCount) {
                MessageBox.Show("보유칸에 비해서 점령칸이 작습니다!");
                return;
            } else if (holdCount == 0 || occupCount == 0) {
                MessageBox.Show("보유칸 또는 점령칸이 0칸 입니다. 설정해주세요!");
                return;
            }

            unionCalculator.CalcUnion();
            Console.WriteLine("연산 끝!");
            outputBtn.IsEnabled = true;
        }


        private void outputBtn_Click(object sender, RoutedEventArgs e) {
            this.NavigationService.Navigate(new Uri("/outputPage.xaml", UriKind.Relative));
        }
    }
}
