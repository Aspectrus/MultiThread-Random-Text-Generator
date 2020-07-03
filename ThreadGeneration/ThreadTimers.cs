using ADODB;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Threading.Timer;

namespace ThreadGeneration
{
    public sealed class TimerParams
    {
        public ListView listView;
        public string timerID;
    }

    public class ThreadTimers
    {
        private Dictionary<string, Timer> _threads;
        int NumberOfThreads;
        private string fileName;
        private int MinLineLengh = int.Parse(ConfigurationManager.AppSettings["MinLineLengh"]);
        private int MaxLineLengh = int.Parse(ConfigurationManager.AppSettings["MaxLineLengh"]);
        private int MinInterval = int.Parse(ConfigurationManager.AppSettings["MinInterval"]);
        private int MaxInterval = int.Parse(ConfigurationManager.AppSettings["MaxInterval"]);
        private int FirstASCIICharacter = int.Parse(ConfigurationManager.AppSettings["FirstASCIICharacter"]);
        private int LastASCIICharacter = int.Parse(ConfigurationManager.AppSettings["LastASCIICharacter"]);
        AccessDatabase accessdatabase;
        public ThreadTimers(int NumberOfThreads, string fileName)
        {
            _threads = new Dictionary<string, Timer>();
            this.NumberOfThreads = NumberOfThreads;
            this.fileName = fileName;
            accessdatabase = new AccessDatabase(fileName);           
        }

        public void LaunchThreads(ListView listView1)
        {
            for (int i = 1; i <= NumberOfThreads; i++)
            {
                string timerID = i.ToString();
                Timer timer = new Timer(new TimerCallback(TimerEventProcessor), new TimerParams { listView = listView1, timerID = timerID }, 0, StaticRandom.Rand(MinInterval, MaxInterval));
                _threads.Add(timerID, timer);

            }
        }

        public void KillAllThreadTimers()
        {
            foreach (var timer in _threads.Values)
            {
                timer.Dispose();
            }
            _threads.Clear();
        }

        public void TimerEventProcessor(object timerParameters)
        {
            TimerParams timerparams = (TimerParams)timerParameters;
           
            int NumberOfGeneratedSymbols = StaticRandom.Rand(MinLineLengh, MaxLineLengh);
            string ThreadGeneratedData = "";
            for (int i = 0; i < NumberOfGeneratedSymbols; i++)
            {
                ThreadGeneratedData += (char)(StaticRandom.Rand(FirstASCIICharacter, LastASCIICharacter));
            }
            AddStringToView(timerparams, ThreadGeneratedData);
            accessdatabase.AddDataToMdbFile(timerparams, ThreadGeneratedData);
            int RandomInterval = StaticRandom.Rand(MinInterval, MaxInterval);
            if(_threads.Count>0) _threads[timerparams.timerID].Change(RandomInterval, 0);
        }
        private void AddStringToView(TimerParams timerParameters, string ThreadString)
        {
            var item = new ListViewItem(new[] { timerParameters.timerID, ThreadString });
            if (timerParameters.listView.InvokeRequired)
            {

                timerParameters.listView.Invoke(new MethodInvoker(delegate
                {
                    if (timerParameters.listView.Items.Count > 20) timerParameters.listView.Items.RemoveAt(0);
                    timerParameters.listView.Items.Add(item);
                    item.Checked = true;

                }));
            }
            else
            {
                if (timerParameters.listView.Items.Count > 20) timerParameters.listView.Items.RemoveAt(0);
                timerParameters.listView.Items.Add(item);
                item.Checked = true;
            }
        }

    }


}
