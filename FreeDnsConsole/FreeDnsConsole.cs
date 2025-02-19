using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using FreeDnsCore;

namespace FreeDnsConsole
{
    class FreeDnsConsole
    {
        private FreeDns _freeDns;
        private StringCollection _hostnames;
        private System.Timers.Timer refreshTimer;
        public FreeDnsConsole()
        {
            if (string.IsNullOrWhiteSpace(Properties.Settings.Default.ApiKey))
            {
                Properties.Settings.Default.ApiKey = FreeDns.GetApiKey(Properties.Settings.Default.Username, Properties.Settings.Default.Password);
                Properties.Settings.Default.Save();
            }
            _freeDns = new FreeDns(Properties.Settings.Default.ApiKey);

            _hostnames = Properties.Settings.Default.Hostnames;

            this.refreshTimer = new System.Timers.Timer();
            this.refreshTimer.Interval = Properties.Settings.Default.RefreshMillis;
        }

        public void Start()
        {
            var sb = new StringBuilder("FreeDns service started for domain names:");
            foreach (var name in _hostnames)
            {
                sb.Append(Environment.NewLine);
                sb.Append(name);
            }
            Console.WriteLine(sb.ToString());
            refreshTimer.Elapsed += refreshDns;
            refreshTimer.Enabled = true;
            refreshTimer.Start();

            // Manually trigger the event handler immediately
            refreshDns(null, null);

        }

        public void refreshDns(object sender, EventArgs e)
        {
            try
            {
                var allAccountRecords = _freeDns.GetRecords(Properties.Settings.Default.ConnectionTimeout);

                var recordsToUpdate =
                    from record in allAccountRecords.Items
                    where _hostnames.Contains(record.host)
                    select record;

                recordsToUpdate.AsParallel().ForAll(record => {
                    _freeDns.UpdateRecord(record.url, Properties.Settings.Default.ConnectionTimeout);
                    Console.WriteLine("FreeDns updated for " + record.host);
                });

                if (recordsToUpdate.Count() == 0)
                {
                    Console.WriteLine("Found no records to update!  Records for account:" +
                        allAccountRecords.Items.Select(item => item.host).Aggregate((agg, host) => (agg == null ? "" : agg) + Environment.NewLine + host));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unhandled exception during refresh DNS: " + ex.Message + " \n" + ex.StackTrace);
            }
        }

        public void Stop()
        {
            Console.WriteLine("FreeDns console stopped");
            refreshTimer.Stop();
            refreshTimer.Elapsed -= refreshDns;
        }
    }
}
