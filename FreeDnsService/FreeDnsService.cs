using FreeDnsCore;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace FreeDnsService
{
    public partial class FreeDnsService : ServiceBase
    {
        private FreeDns _freeDns;
        private StringCollection _hostnames;
        private System.Timers.Timer refreshTimer;

        public FreeDnsService()
        {
            this.ServiceName = Properties.Settings.Default.ServiceName;

            if (string.IsNullOrWhiteSpace(Properties.Settings.Default.ApiKey))
            {
                Properties.Settings.Default.ApiKey = FreeDns.GetApiKey(Properties.Settings.Default.Username, Properties.Settings.Default.Password);
                Properties.Settings.Default.Save();
            }
            _freeDns = new FreeDns(Properties.Settings.Default.ApiKey);
            
            _hostnames = Properties.Settings.Default.Hostnames;

            this.refreshTimer = new System.Timers.Timer();
            this.refreshTimer.Interval = Properties.Settings.Default.RefreshMillis;
            this.ServiceName = Properties.Settings.Default.ServiceName;
        }

        protected override void OnStart(string[] args)
        {
            var sb = new StringBuilder("FreeDns service started for domain names:");
            foreach (var name in _hostnames)
            {
                sb.Append(Environment.NewLine);
                sb.Append(name);
            }
            EventLog.WriteEntry(sb.ToString());
            refreshTimer.Elapsed += refreshDns;
            refreshTimer.Start();
        }

        private void refreshDns(object sender, EventArgs e)
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
                    EventLog.WriteEntry("FreeDns updated for " + record.host);
                });

                if (recordsToUpdate.Count() == 0)
                {
                    EventLog.WriteEntry("Found no records to update!  Records for account:" +
                        allAccountRecords.Items.Select(item => item.host).Aggregate((agg, host) => (agg == null ? "" : agg) + Environment.NewLine + host));
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Unhandled exception during refresh DNS: " + ex.Message + " \n" + ex.StackTrace);
            }
        }

        protected override void OnStop()
        {
            EventLog.WriteEntry("FreeDns service stopped");
            refreshTimer.Stop();
            refreshTimer.Elapsed -= refreshDns;
        }
    }
}
