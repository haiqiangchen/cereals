using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace dev_20180423_UpperComputer
{
    public partial class UpperComputer : Form
    {
        public UpperComputer()
        {
            InitializeComponent();

        }
        List<DateTime> timeList = new List<DateTime>();
        List<string> riceHeavy = new List<string>();
        private StringBuilder builder = new StringBuilder();//避免在事件处理方法中反复的创建，定义到外面。
        StringBuilder mess = new StringBuilder();
        double previous = 0.0;
        double current = 0.0;
        private void Form1_Load(object sender, EventArgs e)
        {
                // 设置曲线的样式
                Series series = chart1.Series[0];
                // 画样条曲线（Spline）
                series.ChartType = SeriesChartType.Spline;
                // 线宽2个像素
                series.BorderWidth = 2;
                // 线的颜色：红色
                series.Color = System.Drawing.Color.Red;
                // 图示上的文字
                series.LegendText = "谷物重量图";
                /*
                * 这里的处理方法是以时间为横坐标，谷物的重量为纵坐标 
                */
                    chart1.ChartAreas["ChartArea1"].AxisX.LabelStyle.Format = "ss";
                    series.Points.AddXY(System.DateTime.Now, "");
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                Int32 n = serialPort1.BytesToRead;//先记录下来，避免某种原因，人为的原因，操作几次之间时间长，缓存不一致  
                byte[] buf = new byte[n];//声明一个临时数组存储当前来的串口数据  
                serialPort1.Read(buf, 0, n);//读取缓冲数据  
                builder.Clear();//清除字符串构造器的内容  
                                //这里所做的是清理相关的缓存，使画图不会有诸多的重复
                riceHeavy.Clear();
                timeList.Clear();

                //因为要访问ui资源，所以需要使用invoke方式同步ui。  
                this.Invoke((EventHandler)(delegate
                {

                    // 设置曲线的样式
                    Series series = chart1.Series[0];
                    // 画样条曲线（Spline）
                    series.ChartType = SeriesChartType.Spline;
                    // 线宽2个像素
                    series.BorderWidth = 2;
                    // 线的颜色：红色
                    series.Color = System.Drawing.Color.Red;
                    // 图示上的文字
                    series.LegendText = "谷物重量图";
                    builder.Append(Encoding.ASCII.GetString(buf));
                    current = Convert.ToDouble(builder.ToString().Split('\n')[0]);
                    /***********************************/
                    double riceRate = RiceRate(previous, current);//计算流速
                    this.textBox2.Text = riceRate.ToString();
                    previous = current;
                    /***********************************/
                   string data = builder.ToString();
                       

                    DataFile(data);
                    if (Convert.ToDouble(data.Split('\n')[0]) != 0.000)
                    {
                        riceHeavy.Add(data);
                        timeList.Add(System.DateTime.Now);
                    }

                    this.listBox1.Items.Add(data + "kg");
                    /*
                    * 这里的处理方法是以时间为横坐标，谷物的重量为纵坐标 
                    */
                    int x = 0;
                    foreach (string rice in riceHeavy)
                    {
                        DateTime time = timeList[x];
                        series.Points.AddXY(time, Convert.ToDouble(rice.Split('\n')[0]));
                        x++;
                    }
                }));
                Thread.Sleep(500);
            }
            catch (Exception) {
                MessageBox.Show("缓存有毛病，这个问题你按确定就可以解决");
                return;
            }
        }
        /***
         * 数据处理成文件
         * **/
        public void DataFile(string message) {            
            mess.Clear();
            mess = mess.Append(message).Append("\r\n");
            byte[] myByte = System.Text.Encoding.UTF8.GetBytes(mess.ToString());  //转换为字节
            using (FileStream fsWrite = new FileStream(@"..\..\log.txt", FileMode.Append))
            {
                fsWrite.Write(myByte, 0, myByte.Length);
            };
        }
        /*
         *功能：显示稻谷的流量
		 *param:previous("上一个谷物重量")
		 *param:current("当前的谷物重量")
         ***/
        public double RiceRate(double previous,double current) {
            double Difference = Convert.ToDouble(current) - Convert.ToDouble(previous);
            double Rate = Difference / 0.5;
            return Rate;
        }
        /*
         *串口配置的参数
             */
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text=="") {
                MessageBox.Show("串口不能为空");
                return;
            }
            if (!(this.serialPort1.IsOpen))
            {
                this.button1.Text = "关闭串口";
                serialPort1.ReceivedBytesThreshold = 1;
                this.serialPort1.PortName = this.textBox1.Text;
                this.serialPort1.BaudRate = 4800;
                this.serialPort1.Open();
                this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(serialPort1_DataReceived);
            }
            else {
                this.button1.Text = "打开串口";
                this.serialPort1.Close();
                return;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            riceHeavy.Clear();
            timeList.Clear();

            //因为要访问ui资源，所以需要使用invoke方式同步ui。  
            this.Invoke((EventHandler)(delegate
            {

                // 设置曲线的样式
                Series series = chart1.Series[0];
                // 画样条曲线（Spline）
                series.ChartType = SeriesChartType.Spline;
                // 线宽2个像素
                series.BorderWidth = 2;
                // 线的颜色：红色
                series.Color = System.Drawing.Color.Red;
                // 图示上的文字
                series.LegendText = "谷物重量图";
                Random random = new Random();
                string data = random.Next(1,10).ToString();

                if (Convert.ToDouble(data.Split('\n')[0]) != 0.000)
                {
                    riceHeavy.Add(data);
                    timeList.Add(System.DateTime.Now);
                }

                this.listBox1.Items.Add(data + "kg");
                /*
                * 这里的处理方法是以时间为横坐标，谷物的重量为纵坐标 
                */
                int x = 0;
                foreach (string rice in riceHeavy)
                {
                    DateTime time = timeList[x];
                    series.Points.AddXY(time, Convert.ToDouble(rice.Split('\n')[0]));
                    x++;
                }
            }));
            Thread.Sleep(500);
        }
        public string data(string data) {
            return data;
        }
    }
}
