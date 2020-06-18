using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace DataAccess
{
    public class TracksRepository : ConnectionClass
    {
        public TracksRepository() : base(){ }
        
        public IQueryable<Track> GetTracks() {
            return Entity.Tracks;
        }

        public Track GetTrack(int TrackId) {
            return Entity.Tracks.SingleOrDefault(x => x.Id == TrackId);
        }

        public void AddTrack(Track t ){
            Entity.Tracks.Add(t);
            Entity.SaveChanges();
        }


        public void RemoveTrack(Track t)
        {
            Entity.Tracks.Remove(t);
            Entity.SaveChanges();
        }
    }
}
