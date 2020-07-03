using System;
using System.Windows.Forms;
using System.IO;
using System.Configuration;

namespace ThreadGeneration
{
    public partial class ApplicationWindow : Form
    {
        private bool ThreadGenerationIsOn = false;
        ThreadTimers threadtimers;
        AccessDatabase accessdatabase;
        private string fileName = "threadData.mdb";
        private int MinThreadCount = int.Parse(ConfigurationManager.AppSettings["MinThreadCount"]);
        private int MaxThreadCount = int.Parse(ConfigurationManager.AppSettings["MaxThreadCount"]);
        public ApplicationWindow()
        {
            InitializeComponent();
            historyListView.Columns[0].Text = "Thread ID";
            historyListView.Columns[1].Text = "Generated string";
            historyListView.Columns[0].Width = (int)(historyListView.Width * 0.30);
            historyListView.Columns[1].Width = (int)(historyListView.Width * 0.69);
          
        }

        private void StartButton_Click(object sender, EventArgs e)
        {

            if (!int.TryParse(ThreadNumberInput.Text, out int NumberOfThreads))
            {
                MessageBox.Show("Incorrect input");
                return;
            } 
            if (NumberOfThreads > MaxThreadCount || NumberOfThreads < MinThreadCount) MessageBox.Show("Incorrect number of threads");

            else
            {
                if (ThreadGenerationIsOn)
                {
                    threadtimers.KillAllThreadTimers();
                    threadtimers = null;
                    accessdatabase = null;
                    StartStopButton.Text = "Start";
                    ThreadGenerationIsOn = false;
                    ThreadNumberInput.ReadOnly = false;
                }

                else
                {                   
                    accessdatabase = new AccessDatabase();
                    if (!accessdatabase.CreateNewAccessDatabase(fileName)) MessageBox.Show("mdb file could not be created");
                    threadtimers = new ThreadTimers(NumberOfThreads, fileName);
                    StartStopButton.Text = "Stop";
                    ThreadGenerationIsOn = true;
                    threadtimers.LaunchThreads(historyListView);
                    
                    ThreadNumberInput.ReadOnly = true;
                }
            }
            
        }
        

        private void ApplicationWindow_ResizeEnd(object sender, EventArgs e)
        {
            historyListView.Columns[0].Width = (int)(historyListView.Width * 0.30);
            historyListView.Columns[1].Width = (int)(historyListView.Width * 0.69);
        }
    }


}
