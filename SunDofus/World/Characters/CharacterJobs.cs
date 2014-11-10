using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Characters
{
    class CharacterJobs
    {
        public List<Jobs.Job> Jobs { get; set; }
        public JobOptions Options { get; set; }

        private Character Client;

        public CharacterJobs(Character client)
        {
            Client = client;
            Options = new JobOptions(0, 0, 0);
            Jobs = new List<Jobs.Job>();
        }

        public void Parse(string data)
        {
            foreach (var j in data.Split('|'))
            {
                var info = data.Split(';');
                var job = new Jobs.Job(int.Parse(info[0]), int.Parse(info[1]), int.Parse(info[2]));

                job.ParseSkills(info[3]);
                Jobs.Add(job);
            }
        }

        public string Save()
        {
            var data = string.Empty;

            foreach (var job in Jobs)
                data += string.Format("{0}|{1};{2};{3};{4}", data, job.ID, job.Level, job.Experience, job.SaveSkills());

            return data.Length > 0 ? data.Substring(1) : "";
        }

        public void AddNewJob(int id)
        {
            if (Jobs.Any(x => x.ID == id))
                return;

            Jobs.Add(new Jobs.Job(id, 1, 0));

            SendJobs();
            SendJobsXP();
            SendJobOptions();
        }

        public void SendJobOptions()
        {
            Client.NClient.Send(string.Concat("JO", Options.ToString()));
        }

        public void SendJobsXP()
        {
            var data = string.Empty;

            foreach (var job in Jobs)
                data = string.Format("{0}|{1};{2};{3};{4};{5}",data, job.ID, job.Level, job.GetMinExperience(), job.Experience, job.GetMaxExperience());
            
            Client.NClient.Send(string.Concat("JX", data));
        }

        public void SendJobs()
        {
            var data = string.Empty;

            foreach (var job in Jobs)
                data = string.Format("{0}|{1};{2}",data, job.ID, job.GetSkills());

            Client.NClient.Send(string.Concat("JS", data));
        }

        public bool HasJob(int id)
        {
            return Jobs.Any(x => x.ID == id);
        }
    }

    class JobOptions
    {
        public JobOptionParams Params { get; set; }
        public int MinSlots { get; set; }
        public int MaxSlots { get; set; }

        public JobOptions(JobOptionParams p, int min, int max)
        {
            Params = p;
            MinSlots = min;
            MaxSlots = max;
        }

        public override string ToString()
        {
            return string.Format("{0}|{1}|{2}", (int)Params, MinSlots, MaxSlots);
        }
    }

    enum JobOptionParams
    {
        PaidService = 1,
        FreeIfFailed = 2,
        ResourcesNeeded = 4
    }
}
